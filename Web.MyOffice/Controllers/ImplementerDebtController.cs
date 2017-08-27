using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

using ASE;
using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using Web.MyOffice.Res;


namespace Web.MyOffice.Controllers.References
{

    [Authorize]
    //[RequireHttps]
    public class ImplementerDebtController : ControllerAdv<DB>
    {
        /*public ActionResult Index(int? page, string searchTB_Filter)
        {
            var DateTo = DefaultDateTo(DateTime.Now);

            var q1 =
                from payments in db.MemberDayReports.Where(x => x.Member.MainMemberId == UserId & x.DateTime < DateTo)
                join members in db.Members.Where(x => x.UserId == UserId) on payments.UserId equals members.MainMemberId
                group payments by new { members, payments.Currency } into g
                //group payments by new { payments.Member, payments.Currency } into g
                select new
                {
                    Member = g.Key.members,
                    Currency = g.Key.Currency,
                    Amount = g.Sum(x => x.Value * x.Amount)
                };

            var q2 =
                from payments in db.MemberPayments.Where(x => x.Member.MainMemberId == UserId & x.DateTime < DateTo)
                join members in db.Members.Where(x => x.UserId == UserId) on payments.UserId equals members.MainMemberId
                group payments by new { members, payments.Currency } into g
                //group payments by new { payments.Member, payments.Currency } into g
                select new {
                    Member = g.Key.members,
                    Currency = g.Key.Currency,
                    Amount = g.Sum(x => x.Amount)
                };

            IQueryable<MemberDebt> list =
                from report in q1
                join payments in q2
                on new { MemberId = report.Member.Id, CurrencyId = report.Currency.Id } equals new { MemberId = payments.Member.Id, CurrencyId = payments.Currency.Id } into paymentsJ
                from payments in paymentsJ.DefaultIfEmpty()
                group new { report, payments } by new { report.Member, report.Currency } into g
                select new MemberDebt
                {
                    Member = g.Key.Member,
                    Currency = g.Key.Currency,
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

            Currency c = ViewBag.CurrencyI = db.Currencies.Find(currencyId);
            ViewBag.Currency = db.Currencies.FirstOrDefault(x => x.UserId == UserId & x.CurrencyType == c.CurrencyType);
            
            var member = db.Members.FirstOrDefault(x => x.Id == memberId & x.UserId == UserId);
            memberId = db.Members.FirstOrDefault(x => x.UserId == member.MainMemberId & x.MainMemberId == UserId).Id;
            ViewBag.Member = member;

            var debt = db.MemberDayReports.Where(x => x.Member.MainMemberId == UserId & x.MemberId == memberId & x.CurrencyId == currencyId & x.DateTime < start).Sum(x => (decimal?)x.Amount * (decimal?)x.Value) ?? 0;
            debt -= db.MemberPayments.Where(x => x.Member.MainMemberId == UserId & x.MemberId == memberId & x.CurrencyId == currencyId & x.DateTime < start).Sum(x => (decimal?)x.Amount) ?? 0;

            ViewBag.Debt = debt;

            List<MemberDayReport> result = new List<MemberDayReport>();
            Guid lastId = Guid.Empty;
            while (true)
            {
                var list = db.MemberDayReports.Where(x => x.Member.MainMemberId == UserId & x.MemberId == memberId & x.CurrencyId == currencyId & x.DateTime <= start & x.Id != lastId)
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

                if (debt <= 0)
                    break;
            }

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
                        db.IAm().Email,
                        db.IAm().Email,
                        System.Configuration.ConfigurationManager.AppSettings["ASE.SiteName"] + " - " + member.Email + " - " + S.Debt,
                        "",
                        new System.Net.Mail.Attachment(ms, S.Details + ".pdf"));
                }

                return RedirectToAction("Details", new { memberId = member.Id, currencyId = currencyId });
            }

            return View(result);
        }*/
    }
}
