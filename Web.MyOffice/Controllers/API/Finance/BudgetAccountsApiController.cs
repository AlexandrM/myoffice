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
            [HttpGet]
            public HttpResponseMessage AccountMotionsGet(Guid accountId)
            {
                return ResponseObject2Json(db.Motions.Any(motion => motion.AccountId == accountId));
            }

            [HttpPost]
            public HttpResponseMessage NewAccountPut(Account newAccount)
            {
                using (db)
                {
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
                var delAcc = db.Accounts.Include(acc => acc.Motions).FirstOrDefault(x => x.Id == accountId && x.Budget.OwnerId == UserId);
                if (delAcc != null)
                {
                    if (delAcc.Motions.Count == 0)
                    {
                        db.Entry(delAcc).State = EntityState.Deleted;
                    }
                    else
                    {
                        delAcc.Deleted = true;
                        db.Entry(delAcc).State = EntityState.Modified;
                        var accMotions = db.Motions.Where(motion=>motion.AccountId == delAcc.Id);
                        foreach (var motion in accMotions)
                        {
                            motion.Deleted = true;
                            db.Entry(motion).State = EntityState.Modified;
                        }
                    }
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
            public HttpResponseMessage CategoryAccountsListGet()
            {
                var budgetUsers = db.BudgetUsers
                    .Where(x => x.UserId == UserId || x.Budget.OwnerId == UserId)
                    .Include(x => x.Budget)
                    .Include(x => x.Budget.CategoryAccounts)
                    .Include(x => x.Budget.CategoryAccounts.Select(z => z.Accounts))
                    .ToList();
                var categoryAccounts = budgetUsers.Select(budgetuser => budgetuser.Budget.CategoryAccounts).Distinct().ToList();

                var budgets = budgetUsers.Select(budgetuser => budgetuser.Budget).Distinct().ToList();
                dynamic model = new
                {
                    Categories = CategoriesFilter(categoryAccounts).OrderBy(cat=>cat.Name),
                    Budgets = budgets.OrderBy(budget=>budget.Name),
                    Currencies = db.Currencies.Where(x => x.UserId == UserId).ToList().OrderBy(currency=>currency.Name)
                };
                return ResponseObject2Json(model);
            }

        private List<CategoryAccount> CategoriesFilter(List<List<CategoryAccount>> categoryAccounts)
        {
            var categories = new List<CategoryAccount>();
            foreach (var catAccs in categoryAccounts)
            {
                categories.AddRange(catAccs.Distinct());
            }
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Accounts.Count(acc=>acc.Deleted) == categories[i].Accounts.Count && categories[i].Accounts.Count != 0)
                {
                    categories.Remove(categories[i]);
                    continue;
                }
                for (int j = 0; j < categories[i].Accounts.Count; j++)
                {
                    if (categories[i].Accounts[j].Deleted)
                    {
                        categories[i].Accounts.Remove(categories[i].Accounts[j]);
                    }
                }

            }
            return categories;
        }

        [HttpGet]
        public HttpResponseMessage CategoryDelete(Guid deleteId, bool totalDelete)
        {
            var deleteCategory = db.CategoryAccounts
                                .Include(cat => cat.Accounts)
                                .FirstOrDefault(cat => cat.Id == deleteId);
            if (deleteCategory != null)
            {
                if (totalDelete)
                {
                    db.Entry(deleteCategory).State = EntityState.Deleted;
                }
                else
                {
                    if (deleteCategory.Accounts.Count == 0)
                    {
                        db.Entry(deleteCategory).State = EntityState.Deleted;
                    }
                    else
                    {
                        db.Entry(deleteCategory).State = EntityState.Modified;
                        var accs = db.Accounts.Include(acc => acc.Motions)
                            .Where(acc => acc.CategoryId == deleteCategory.Id).ToList();
                        foreach (var acc in accs)
                        {
                            acc.Deleted = true;
                            db.Entry(acc).State = EntityState.Modified;
                            foreach (var motion in acc.Motions)
                            {
                                motion.Deleted = true;
                            }
                            db.Entry(acc).State = EntityState.Modified;
                        }
                    }
                }
            }
            else
            {
                return ResponseObject2Json(false);
            }
            db.SaveChanges();
            return ResponseObject2Json(true);
        }
    
        [HttpPut]
            public HttpResponseMessage CategoryUpdate(CategoryAccount newCategory)
            {
                using (db)
                {
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