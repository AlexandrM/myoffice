using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;

using ASE;
using ASE.MVC;

using Web.MyOffice.Data;
using MyBank.Models;

namespace Web.MyOffice.Controllers.API
{
    public class BudgetAccountsController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage GetCategoryAccountsList()
        {
            dynamic model = new
            {
                BudgetUsers = db.BudgetUsers
                    .Include(x => x.Budget)
                    .Include(x => x.Budget.CategoryAccounts)
                    .Include(x => x.Budget.CategoryAccounts.Select(z => z.Accounts))
                    .Where(x => x.UserId == UserId || x.Budget.OwnerId == UserId)
                    .ToList(),

                Currencies = db.Currencies.Where(x => x.UserId == UserId).ToList()
            };
            return ResponseObject2Json(model);
        }

        [HttpPut]
        public HttpResponseMessage NewAccountPut(Account newAccount)
        {
            using (db)
            {
                if (db.Accounts.Find(newAccount.Id) != null)
                {
                    db.Entry(newAccount).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(newAccount).State = EntityState.Added;
                }
                db.SaveChanges();
            }

            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        public class EditCategoryList {
            public List<CategoryAccount> NewCategories { set; get; }
            public List<CategoryAccount> EditCategories { set; get; }
            public List<CategoryAccount> DelCategories { set; get; }
        }

        [HttpPost]
        public HttpResponseMessage CategoryListPost(EditCategoryList categoryList)
        {
            using (db)
            {
                if (categoryList != null)
                {
                    if (categoryList.DelCategories != null && categoryList.DelCategories.Count > 0)
                    {
                        foreach (var delCategories in categoryList.DelCategories)
                        {
                            db.Entry(delCategories).State = EntityState.Deleted;
                        }
                        db.SaveChanges();
                    }
                    if (categoryList.EditCategories != null && categoryList.EditCategories.Count > 0)
                    {
                        foreach (var editCategories in categoryList.EditCategories)
                        {
                            db.Entry(editCategories).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    if (categoryList.NewCategories != null && categoryList.NewCategories.Count > 0)
                    {
                        foreach (var newCategories in categoryList.NewCategories)
                        {
                            db.Entry(newCategories).State = EntityState.Added;
                        }
                        db.SaveChanges();
                    }
                    return ResponseObject2Json(HttpStatusCode.Accepted);
                }
                else
                {
                    return ResponseObject2Json(HttpStatusCode.NotModified);
                }
            } 
        }

        [HttpDelete]
        public HttpResponseMessage AccountDelete(Guid accountId)
        {
            var delAcc = db.Accounts.Include(acc=>acc.Motions).FirstOrDefault(x => x.Id == accountId && x.Budget.OwnerId == UserId);
            if (delAcc != null )
            {
                if (delAcc.Motions.Count == 0)
                {
                    db.Entry(delAcc).State = EntityState.Deleted;
                }
                else
                {
                    delAcc.Deleted = true;
                    db.Entry(delAcc).State = EntityState.Modified;
                }
                db.SaveChanges();
                return ResponseObject2Json(true);
            }
            else
            {
                return ResponseObject2Json(false);
            } 
        }

        [HttpGet]
        public HttpResponseMessage AccountMotionsGet(Guid accountId)
        {
            return ResponseObject2Json(db.Motions.Any(motion=>motion.AccountId == accountId));
        }
    }
}