using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity;
using System.Dynamic;

using ASE;
using ASE.EF;
using ASE.MVC;
using ASE.Json;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MyBank.Models;
using MVC = Web.MyOffice.Controllers.MyBank;
using Method = System.Web.Http;


namespace Web.MyOffice.Controllers.API
{
    public class BudgetAccountsController : ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage GetCategoryAccountsList()
        { 
            dynamic model = (new object()).ToDynamic();
                model.BudgetUsers = db.BudgetUsers
                    .Include(x => x.Budget)
                    .Include(x => x.Budget.CategoryAccounts)
                    .Include(x => x.Budget.CategoryAccounts.Select(z => z.Accounts))
                    .Where(userBudget=>userBudget.UserId == UserId)
                    .ToList();
                model.Currencies = db.Currencies.ToList();
            return ResponseObject2Json(model);
        }

        [Method.HttpPut]
        public HttpResponseMessage NewAccountPut(Account newAccount)
        {
            using (db)
            {
                if (db.Accounts.Where(acc=>acc.Id == newAccount.Id).Count()>0)
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

        [Method.HttpPost]
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
                    if (categoryList.NewCategories != null && categoryList.NewCategories.Count>0)
                    {
                        foreach (var newCategories in categoryList.NewCategories)
                        {
                            db.Entry(newCategories).State = EntityState.Added;
                        }
                        db.SaveChanges();
                    }
                    return ResponseObject2Json(HttpStatusCode.Accepted);
                } else return ResponseObject2Json(HttpStatusCode.NotModified);
            } 
        }

        [Method.HttpDelete]
        public HttpResponseMessage CategoryListPost(Guid accountId)
        {
            var delAccs = db.Accounts.Where(account => account.Id == accountId);
            if (delAccs!= null && delAccs.Count() ==1)
            {
                db.Entry(delAccs.First()).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Moved);
        }
    }
}