using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;

using ASE;
using ASE.MVC;
using System.Collections.Generic;

using Web.MyOffice.Data;
using MyBank.Models;

namespace Web.MyOffice.Controllers.API
{
    public class BudgetAccountsController : ControllerApiAdv<DB>
    {
        #region Accounts

        [HttpPost]
        public HttpResponseMessage NewAccountPut(Account newAccount)
        {
            using (db)
            {
                var category = db.CategoryAccounts.FirstOrDefault(x => x.Budget.OwnerId == UserId & x.Id == newAccount.CategoryId);
                newAccount.CategoryId = category.Id;

                if (db.Accounts.Any(acc=> acc.Id == newAccount.Id))
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

        [HttpDelete]
        public HttpResponseMessage AccountDelete(Guid accountId)
        {
            var delAcc = db.Accounts.FirstOrDefault(x => x.Id == accountId && x.Category.Budget.OwnerId == UserId);
            if (!db.Motions.Any(x => x.AccountId == delAcc.Id))
            {
                db.Entry(delAcc).State = EntityState.Deleted;
                db.SaveChanges();
                return ResponseObject2Json(true);
            }
            else
            {
                return ResponseObject2Json(false);
            }
        }
        #endregion

        #region Category

        [HttpGet]
        public HttpResponseMessage GetUsersBudgets()
        {
            var categories = db.CategoryAccounts
                .AsNoTracking()
                .Include(x => x.Accounts)
                .Where(x => x.Budget.OwnerId == UserId)
                .OrderBy(x => x.Name)
                .ToList();

            categories.ForEach(x => x.Accounts = x.Accounts.OrderBy(z => z.Name).ToList());
            categories.ForEach(x => {
                x.Accounts.ForEach(z =>
                {
                    z.Motions = db.Motions.Any(c => c.AccountId == z.Id) ? new List<Motion> { new Motion () } : new List<Motion>();
                });
            });

            var model = new
            {
                Categories = categories,

                Currencies = db.Currencies
                    .AsNoTracking()
                    .Where(x => x.UserId == UserId)
                    .OrderBy(x => x.Name)
                    .ToList(),

                Budgets = db.Budgets
                    .AsNoTracking()
                    .Where(x => x.OwnerId == UserId)
                    .OrderBy(x => x.Name)
                    .ToList(),
            };

            return ResponseObject2Json(model);
        }

        [HttpDelete]
        public HttpResponseMessage CategoryDelete(Guid deleteId, bool totalDelete)
        {
            var deleteCategory = db.CategoryAccounts
                                .Include(x => x.Accounts)
                                .FirstOrDefault(x => x.Id == deleteId & x.Budget.OwnerId == UserId);

            if (!deleteCategory.Accounts.Any())
            {
                db.Entry(deleteCategory).State = EntityState.Deleted;
                db.SaveChanges();
            }
            
            return ResponseObject2Json(true);
        }
    
        [HttpPut]
        public HttpResponseMessage CategoryUpdate(CategoryAccount newCategory)
        {
            using (db)
            {
                newCategory.BudgetId = db.Budgets.FirstOrDefault(x => x.OwnerId == UserId & x.Id == newCategory.BudgetId).Id;
                if (db.CategoryAccounts.Any(cat => cat.Id == newCategory.Id))
                {
                    db.Entry(newCategory).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(newCategory).State = EntityState.Added;
                }
                db.SaveChanges();
                return ResponseObject2Json(true);
            }
        }

        #endregion
    }
}