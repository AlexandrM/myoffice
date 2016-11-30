using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Rotativa.MVC;

using ASE;
using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using Web.MyOffice.Res;

namespace Web.MyOffice.Controllers.References
{

    [Authorize]
    [RequireHttps]
    public class MemberDebtController : ControllerAdv<DB>
    {
        public ActionResult Index(int? page, string searchTB_Filter)
        {
            var DateTo = DefaultDateTo(DateTime.Now);

            var q1 =
                from payments in db.MemberDayReports.Where(x => x.UserId == UserId & x.DateTime < DateTo)
                group payments by new { payments.Member } into g
                select new
                {
                    Member = g.Key.Member,
                    Amount = g.Sum(x => x.Value * x.Amount)
                };

            var q2 =
                from payments in db.MemberPayments.Where(x => x.UserId == UserId & x.DateTime < DateTo)
                group payments by new { payments.Member } into g
                select new {
                    Member = g.Key.Member,
                    Amount = g.Sum(x => x.Amount)
                };

            IQueryable<MemberDebt> list =
                from report in q1
                join payments in q2
                on new { MemberId = report.Member.Id } equals new { MemberId = payments.Member.Id } into paymentsJ
                from payments in paymentsJ.DefaultIfEmpty()
                group new { report, payments } by new { report.Member } into g
                select new MemberDebt
                {
                    Member = g.Key.Member,
                    Debt = g.Sum(x => x.report.Amount - ((decimal?)x.payments.Amount ?? 0))
                };

            list = list.Where(x => x.Debt != 0);

            if (!String.IsNullOrEmpty(searchTB_Filter))
            {
                string[] filters = searchTB_Filter.Split(' ');
                foreach (var filter in filters)
                    list = list.ApplyFilterPatern(filter,
                        x => x.Member.FirstName
                        , x => x.Member.FullName
                        , x => x.Member.LastName
                        , x => x.Member.MiddleName
                        );
            }

            list = list.OrderBy(x => x.Member.FullName).ThenBy(x => x.Member.LastName).ThenBy(x => x.Member.FirstName).ThenBy(x => x.Currency.ShortName);
            var model = SearchablePagedList<MemberDebt>.Create(list, page, 25, searchTB_Filter);
            FillViewBag();
            if (Request.IsAjaxRequest())
                return new JsonResult
                {
                    Data = new
                    {
                        html = this.RenderPartialView("IndexList", model)
                    }
                };

            return View(model);
        }

        public void FillViewBag()
        {
        }

        public ActionResult Details(Guid memberId, Guid currencyId, bool? send)
        {
            var start = DefaultDateTo(DateTime.Now);

            var debt = db.MemberDayReports.Where(x => x.UserId == UserId & x.MemberId == memberId & x.DateTime < start).Sum(x => (decimal?)(x.Amount * x.Value)) ?? 0;
            debt -= db.MemberPayments.Where(x => x.UserId == UserId & x.MemberId == memberId & x.DateTime < start).Sum(x => (decimal?)x.Amount) ?? 0;

            ViewBag.Debt = debt;

            List<MemberDayReport> result = new List<MemberDayReport>();
            Guid lastId = Guid.Empty;
            while (true)
            {
                if (debt <= 0)
                    break;

                var list = db.MemberDayReports.Where(x => x.UserId == UserId & x.MemberId == memberId & x.DateTime <= start & x.Id != lastId)
                    .OrderByDescending(x => x.DateTime).Take(1).ToList();
                
                if (list.Count() == 0)
                    break;

                foreach(var item in list)
                {
                    lastId = item.Id;
                    start = item.DateTime;

                    result.Add(item);

                    debt -= item.Value * item.Amount;
                    if (debt <= 0)
                        break;
                }
            }

            ViewBag.Currency = db.Currencies.Find(currencyId);
            var model = ViewBag.Member = db.Members.Find(memberId);

            if (send.HasValue)
            {
                ViewBag.RenderToPdf = true;

                var pdf = new Rotativa.MVC.ViewAsPdf("Details", "_RenderToPdf", result);
                pdf.RotativaOptions.PageOrientation = Rotativa.Core.Options.Orientation.Landscape;
                pdf.RotativaOptions.PageMargins.Left = 15;

                using (var ms = new MemoryStream())
                {
                    var arr = pdf.BuildPdf(this.ControllerContext);
                    ms.Write(arr, 0, arr.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    EMail.SendEmail(
                        model.Email,
                        model.Email,
                        System.Configuration.ConfigurationManager.AppSettings["ASE.SiteName"] + " - " + db.IAm().Email + " - " + S.Debt,
                        "",
                        new System.Net.Mail.Attachment(ms, S.Details + ".pdf"));
                }

                return RedirectToAction("Details", new { memberId = memberId, currencyId = currencyId });
            }

            return View(result);
        }

        public ActionResult Import()
        {
            foreach(var currency in db.Currencies.ToList())
            {
                var lr = db.CurrencyRates.Where(x => x.CurrencyId == currency.Id).OrderByDescending(x => x.DateTime).Take(1).FirstOrDefault();
                if (lr == null)
                {
                    currency.MyCurrency = true;
                    currency.Value = 1;
                }
                else
                {
                    currency.Value = lr.Value;
                }
                db.SaveChanges();
            }
            foreach(var report in db.MemberDayReports.
                Include(x => x.Member)
                .ToList())
            {
                var project = db.ProjectMembers.Where(x => x.MemberId == report.Member.Id & x.MemberType == ProjectMemberType.Customer).Select(x => x.Project).FirstOrDefault();

                if (project == null)
                {
                    MemberRate rate = db.MemberRates.Include(x => x.Currency).Where(x => x.MemberId == report.MemberId).OrderByDescending(x => x.DateTime).Take(1).FirstOrDefault();

                    project = new Project
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = report.UserId,
                        DateTime = new DateTime(2015, 01, 01),
                        Description = report.Member.FullName,
                        Name = report.Member.FullName,
                        State = ProjectState.InProgress,
                    };

                    if (rate != null)
                    {
                        project.RateCurrencyType = rate.Currency.CurrencyType;
                        project.RateValue = rate.Value;
                        project.RateType = rate.RateType;
                    }
                    db.Projects.Add(project);
                    db.SaveChanges();

                    db.ProjectMembers.Add(new ProjectMember
                    {
                        MemberId = report.UserId,
                        ProjectId = project.Id,
                        MemberType = ProjectMemberType.Implementer
                    });
                    db.ProjectMembers.Add(new ProjectMember
                    {
                        MemberId = report.Member.Id,
                        ProjectId = project.Id,
                        MemberType = ProjectMemberType.Customer
                    });
                    db.SaveChanges();
                }
                report.ProjectId = project.Id;
                db.SaveChanges();
            }

            foreach (var payment in db.MemberPayments.ToList())
            {
                var project = db.ProjectMembers.Where(x => x.MemberId == payment.Member.Id & x.MemberType == ProjectMemberType.Customer).Select(x => x.Project).FirstOrDefault();

                payment.ProjectId = project.Id;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        /*public ActionResult DetailsToEmail(Guid memberId, Guid currencyId)
        {
            var pdf = new ActionAsPdf("Details", new { memberId = memberId, currencyId = currencyId, send = true });
            var model = db.Members.Find(memberId);

            using(var ms = new MemoryStream())
            {
                var arr = pdf.BuildPdf(this.ControllerContext);
                ms.Write(arr, 0, arr.Length);
                ms.Seek(0, SeekOrigin.Begin);

                EMail.SendEmail(
                    model.Email,
                    model.Email,
                    System.Configuration.ConfigurationManager.AppSettings["ASE.SiteName"] + " - " + db.IAm().Email + " - " + S.Debt,
                    "",
                    new System.Net.Mail.Attachment(ms, S.Details + ".pdf"));
            }
            return RedirectToAction("Details", new { memberId = memberId, currencyId = currencyId });
        }*/
    }
}
