using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using ASE;
using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace Web.MyOffice.Controllers.References
{
    [Authorize]
    //[RequireHttps]
    public class ImplementerMotionsController : ControllerAdv<DB>
    {
        public ActionResult Index(DateTime? DateFrom, DateTime? DateTo, int? page, string searchTB_Filter, Guid memberId, Guid currencyId)
        {
            DateFrom = DefaultDateFrom(DateFrom);
            DateTo = DefaultDateTo(DateTo);
            ViewBag.DateFrom = DateFrom.Value.ToStringD();
            ViewBag.DateTo = DateTo.Value.ToStringD();

            List<MemberMotion> list = new List<MemberMotion>();


            var c = db.Currencies.Find(currencyId);
            var member = db.Members.FirstOrDefault(x => x.Id == memberId & x.UserId == UserId);
            memberId = db.Members.FirstOrDefault(x => x.UserId == member.MainMemberId & x.MainMemberId == UserId).Id;

            ViewBag.Currency = db.Currencies.FirstOrDefault(x => x.UserId == UserId & x.CurrencyType == c.CurrencyType);
            ViewBag.Member = member;


            IQueryable<MemberDayReport> list1 = db.MemberDayReports.Where(x => x.Member.MainMemberId == UserId & x.MemberId == memberId & x.DateTime >= DateFrom & x.DateTime <= DateTo);
            if (!String.IsNullOrEmpty(searchTB_Filter))
            {
                string[] filters = searchTB_Filter.Split(' ');
                foreach (var filter in filters)
                    list1 = list1.ApplyFilterPatern(filter,
                        x => x.DateTime
                        , x => x.Member.FirstName
                        , x => x.Member.FullName
                        , x => x.Member.LastName
                        , x => x.Member.MiddleName
                        );
            }
            list1 = list1.OrderByDescending(x => x.DateTime).ThenBy(x => x.Member.FullName).ThenBy(x => x.Member.LastName).ThenBy(x => x.Member.FirstName);

            IQueryable<MemberPayment> list2 = db.MemberPayments.Where(x => x.Member.MainMemberId == UserId & x.DateTime >= DateFrom & x.DateTime <= DateTo & x.MemberId == memberId);
            if (!String.IsNullOrEmpty(searchTB_Filter))
            {
                string[] filters = searchTB_Filter.Split(' ');
                foreach (var filter in filters)
                    list2 = list2.ApplyFilterPatern(filter,
                        x => x.DateTime
                        , x => x.Member.FirstName
                        , x => x.Member.FullName
                        , x => x.Member.LastName
                        , x => x.Member.MiddleName
                        );
            }
            list2 = list2.OrderByDescending(x => x.DateTime).ThenBy(x => x.Member.FullName).ThenBy(x => x.Member.LastName).ThenBy(x => x.Member.FirstName);

            list.AddRange(list1.ToList());
            list.AddRange(list2.ToList());

            decimal rest = db.MemberDayReports.Where(x => x.MemberId == memberId & x.Member.MainMemberId == UserId).Sum(x => (decimal?)x.Amount * (decimal?)x.Value) ?? 0;
            rest -= db.MemberPayments.Where(x => x.MemberId == memberId & x.Member.MainMemberId == UserId).Sum(x => (decimal?)x.Amount) ?? 0;
            ViewBag.Rest = rest;


            var debtStart = db.MemberDayReports.Where(x => x.MemberId == memberId & x.Member.MainMemberId == UserId & x.DateTime < DateFrom).Sum(x => (decimal?)x.Amount * (decimal?)x.Value) ?? 0;
            debtStart -= db.MemberPayments.Where(x => x.MemberId == memberId & x.Member.MainMemberId == UserId & x.DateTime < DateFrom).Sum(x => (decimal?)x.Amount) ?? 0;

            var debtEnd = db.MemberDayReports.Where(x => x.MemberId == memberId & x.Member.MainMemberId == UserId & x.DateTime < DateTo).Sum(x => (decimal?)x.Amount * (decimal?)x.Value) ?? 0;
            debtEnd -= db.MemberPayments.Where(x => x.MemberId == memberId & x.Member.MainMemberId == UserId & x.DateTime < DateTo).Sum(x => (decimal?)x.Amount) ?? 0;

            ViewBag.DebtStart = debtStart;
            ViewBag.OnEnd = debtEnd;

            var model = SearchablePagedList<MemberMotion>.Create(list, page, 25, searchTB_Filter);
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


        public ActionResult Delete1(Guid id)
        {
            var model = db.MemberDayReports.FirstOrDefault(x => x.Id == id & x.UserId == UserId);
            db.MemberDayReports.Remove(model);
            db.SaveChanges();

            if (Request.IsAjaxRequest())
                return new JsonResult { Data = new { Action = "Index" } };

            return RedirectToAction("Index");
        }

        public ActionResult Delete2(Guid id)
        {
            var model = db.MemberPayments.FirstOrDefault(x => x.Id == id & x.UserId == UserId);
            db.MemberPayments.Remove(model);
            db.SaveChanges();

            if (Request.IsAjaxRequest())
                return new JsonResult { Data = new { Action = "Index" } };

            return RedirectToAction("Index");
        }
    }
}
