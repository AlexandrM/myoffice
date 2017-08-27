using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Threading.Tasks;
using Web.MyOffice.Res;


namespace Web.MyOffice.Controllers
{
    [Authorize]
    //[RequireHttps]
    [ViewEngineAdv("References/")]
    public class MemberController : ControllerAdv<DB>
    {
        public ActionResult Index(int? page, string searchTB_Filter, bool? isSelect)
        {
            IQueryable<Member> list = db.Members
                .Where(x => x.UserId == UserId);

            if (!String.IsNullOrEmpty(searchTB_Filter))
            {
                string[] filters = searchTB_Filter.Split(' ');
                foreach (var filter in filters)
                    list = list.ApplyFilterPatern(filter,
                        x => x.Email
                        , x => x.FirstName
                        , x => x.FullName
                        , x => x.LastName
                        , x => x.MiddleName
                        );
            }

            list = list.OrderBy(x => x.FullName).ThenBy(x => x.LastName).ThenBy(x => x.FirstName);
            var r = list.ToList();
            var item = r.FirstOrDefault(x => x.Id == UserId);
            if (item != null)
            {
                r.Remove(item);
                r.Insert(0, item);
            }
            var model = SearchablePagedList<Member>.Create(r, page, 25, searchTB_Filter);
            if (Request.IsAjaxRequest())
                return new JsonResult
                {
                    Data = new
                    {
                        html = this.RenderPartialView("IndexList", model)
                    }
                };
            else
                return View("Index", model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member model)
        {
            if (ModelState.IsValid)
            {
                //Member
                Member member = null;
                //Seek member
                var eMember = db.Members.FirstOrDefault(x => x.Email == model.Email & x.Id == x.MainMemberId & x.Id == x.UserId);
                //Create if exits
                member = eMember ?? Member.NewMember(model.Email);
                if (eMember == null)
                {
                    db.Members.Add(member);
                    db.SaveChanges();
                }

                model.Id = Guid.NewGuid();
                model.MainMemberId = member.Id;
                model.UserId = UserId;
                db.Members.Add(model);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public JsonResult MemberExists(string email)
        {
            var member = db.Members.FirstOrDefault(x => x.Email == email & x.Id == x.UserId & x.Id == x.MainMemberId);

            return new JsonResult
            {
                Data = new
                {
                    result = member != null
                }
            };
        }

        public ActionResult Edit(Guid id)
        {
            var model = db.Members.FirstOrDefault(x => x.Id == id & x.UserId == UserId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Member model)
        {
            if (ModelState.IsValid)
            {
                AttachModel(model);
                model.UserId = UserId;
                db.Entry(model).Property("Email").IsModified = false;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(Guid id)
        {
            if (id != UserId)
            { 
                var model = db.Members.FirstOrDefault(x => x.Id == id & x.UserId == UserId);
                db.Members.Remove(model);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult MyProfile()
        {
            var model = db.Members.FirstOrDefault(x => x.Id == UserId & x.Id == UserId & x.MainMemberId == UserId);
            ViewBag.User = UserManager.FindByIdAsync(UserIdS).Result;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(Member model)
        {
            if (ModelState.IsValid)
            {
                AttachModel(model);
                db.Entry(model).Property("Email").IsModified = false;
                db.SaveChanges();

                return RedirectToAction("MyProfile");
            }

            return View(model);
        }

        public ActionResult List()
        {
            return View();
        }
    }
}
