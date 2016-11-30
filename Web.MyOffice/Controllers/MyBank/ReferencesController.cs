using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Drawing;

using ASE.MVC;

using Web.MyOffice.Models;
using Web.MyOffice.Data;

using MyBank.Models;

namespace MyBank.Controllers
{
    [RequireHttps]
    [Authorize]
    [ViewEngineAdv("MyBank/")]
    public class ReferencesController : ControllerAdv<DB>
    {

        private GlobalDAL DAL = GlobalDAL.CreateInstance;

        #region References

        public ActionResult Index()
        {
            if (DAL.CurrentOwner.Id == UserId)
            {
                if (DAL.Currencies.Count == 0)
                    return RedirectToAction("Currencies");
                if (DAL.CategoryAccounts.Count == 0)
                    return RedirectToAction("CategoryAccounts");
                if (DAL.Accounts.Count == 0)
                    return RedirectToAction("Accounts");
            }

            return RedirectToAction("Items");
        }

        public JsonResult AccountList(Guid? currencyid, Guid? categoryid)
        {
            ViewData["currencyid"] = currencyid;
            ViewData["categoryid"] = categoryid;

            string html = this.RenderPartialView("AccountList",
                DAL.AccountsFor(currencyid, categoryid));

            return new JsonResult
            {
                Data = new
                {
                    title = @R.R.Accounts,
                    html = html
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ItemList(Guid? categoryid)
        {
            ViewData["categoryid"] = categoryid;

            string html = this.RenderPartialView("ItemList",
                DAL.ItemsForCategory(categoryid));

            return new JsonResult
            {
                Data = new
                {
                    title = @R.R.Items,
                    html = html
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion References

        #region Currency

        public ActionResult Currencies()
        {
            return View(DAL.Currencies);
        }

        public JsonResult CurrencyEdit(Guid id)
        {
            Currency item = DAL.CurrencyGet(id);
            if (item == null)
                item = new Currency();

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("CurrencyEdit", item),
                    title = R.R.Currency + ": " + item.Name
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /*[HttpPost]
        public ActionResult CurrencyEdit(Currency model)
        {
            DAL.CurrencyAddUpdate(model);

            return RedirectToAction("Currencies");
        }*/

        public JsonResult CurrencyDelete(Guid id)
        {
            string error = "";
            try
            {
                DAL.CurrencyDelete(id);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /*public JsonResult CurrencySave(Guid id, string name, string shortName)
        {
            Currency curency = DAL.CurrencyAddUpdate(id, name, shortName);

            return new JsonResult
            {
                Data = new
                {
                    id = curency.Id,
                    ok = true
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }*/

        public ActionResult CurrencySetPrimary(Guid id)
        {
            DAL.CurrencySetPrimary(id);

            return RedirectToAction("Currencies");
        }

        public ActionResult CurrencyRates(Guid id)
        {
            ViewBag.Currency = DAL.CurrencyGet(id);
            return View(DAL.CurrencyRateList(id));
        }

        public JsonResult CurrencyRateEdit(int id, Guid currencyid)
        {
            CurrencyRate item = DAL.CurrencyRateGet(id);
            if (item == null)
                item = new CurrencyRate {
                    Currency = DAL.CurrencyGet(currencyid),
                    DateTime = DateTime.Now,
                    Value = 1
                };

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("CurrencyRateEdit", item),
                    title = R.R.Rate
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult CurrencyRateEdit(CurrencyRate model)
        {
            DAL.CurrencyRateSaveAdd(model);

            return RedirectToAction("CurrencyRates", new { id = model.CurrencyId });
        }

        public JsonResult CurrencyRateDelete(int id)
        {
            string error = "";
            try
            {
                DAL.CurrencyRateDelete(id);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CurrencyRateSave(int id, Guid currencyid, DateTime datetime, decimal rate, decimal multiplicity)
        {
            DAL.CurrencyRateSaveAdd(id, currencyid, datetime, rate, multiplicity);

            return new JsonResult
            {
                Data = new
                {
                    accountId = id,
                    result = true
                }
            };
        }        

        #endregion Currency

        #region CategoryAccount

        public ActionResult CategoryAccounts()
        {
            return View(DAL.CategoryAccounts);
        }

        public JsonResult CategoryAccountEdit(Guid? id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("CategoryAccountEdit", DAL.CategoryAccountGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult CategoryAccountEdit(Guid? id, string name, string description)
        {
            CategoryAccount category = DAL.CategoryAccountAddUpdate(id, name, description);

            return RedirectToAction("CategoryAccounts");
        }


        public JsonResult CategoryAccountDelete(Guid id)
        {
            string error = "";
            try
            {
                DAL.CategoryAccountDelete(id);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion CategoryAccount

        #region CategoryItem

        public ActionResult CategoryItems()
        {
            return View(DAL.CategoryItems);
        }

        public JsonResult CategoryItemEdit(Guid? id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("CategoryItemEdit", DAL.CategoryItemGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult CategoryItemEdit(CategoryItem model)
        {
            DAL.CategoryItemSaveAdd(model);

            return RedirectToAction("CategoryItems");
        }

        /*public JsonResult CategoryItemSave(Guid id, string name, string description)
        {
            CategoryItem item = DAL.CategoryItemSaveAdd(id, name, description);

            return new JsonResult
            {
                Data = new
                {
                    id = item.Id,
                    ok = true
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }*/


        public JsonResult CategoryItemDelete(Guid id)
        {
            string error = "";
            try
            {
                DAL.CategoryItemDelete(id);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CategoryItemSetInternal(Guid id, bool set)
        {
            string error = "";
            DAL.CategoryItemSetInternal(id, set);

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CategoryItemsList(Guid id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("CategoryItemsList", DAL.ItemsForCategory(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion ItemCategory

        #region Item

        public ActionResult Items(Guid? id)
        {
            RouteData.Values.Remove("id");
            ViewBag.Active = id.HasValue ? id.Value : Guid.Empty;
            return View(DAL.Items);
        }

        public JsonResult ItemEdit(Guid? id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("ItemEdit", DAL.ItemGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult ItemEdit(Item model)
        {
            DAL.ItemSaveAdd(model);

            return RedirectToAction("Items", new { id = model.CategoryId });
        }

        public JsonResult ItemSave(Guid id, string name, string category, bool deleted)
        {
            DAL.ItemSaveAdd(id, name, category, deleted);

            return new JsonResult
            {
                Data = new
                {
                    accountId = id,
                    result = true
                }
            };
        }

        public JsonResult ItemDelete(Guid id, Guid categoryId)
        {
            string error = "";
            if (DAL.ItemDeleteRemove(id) == null)
            {
                //id = 0;
            }

            return new JsonResult
            {
                Data = new
                {
                    accountId = id,
                    error = error,
                    url = Url.Action("Items", new { id = categoryId})
                }
            };
        }

        public JsonResult ItemMerge(Guid main, Guid[] list)
        {
            string error = "";
            try
            {
                DAL.ItemMerge(main, list);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error
                }
            };
        }

        public JsonResult ItemMotionList(Guid id)
        {
            string error = "";
            Item itm = DAL.ItemGet(id);
            List<Motion> list = DAL.MotionsByItem(itm);

            ViewData["itemid"] = id;
            string html = this.RenderPartialView("MotionsList", list);

            return new JsonResult
            {
                Data = new
                {
                    html = html,
                    title = @R.R.Item + ": " + itm.Name,
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult MotionEdit(Guid item)
        {
            string error = "";
            Motion motion = DAL.MotionGet(item);

            string html = this.RenderPartialView("MotionsEdit", motion);

            return new JsonResult
            {
                Data = new
                {
                    html = html,
                    name = String.Format("{0} {1} {2}", motion.Account.Name, motion.DateTime, (motion.SumM == 0 ? motion.SumP : motion.SumM * -1)),
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ItemListItem(Guid item)
        {
            string error = "";
            Item itm = DAL.ItemGet(item);

            string html = this.RenderPartialView("ItemListItem", itm);

            return new JsonResult
            {
                Data = new
                {
                    html = html,
                    id = itm.Id,
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion Item

        #region Account

        public ActionResult Accounts(Guid? categoryId)
        {
            ViewBag.CategoryId = categoryId;

            return View(DAL.Accounts);
        }

        public JsonResult AccountEdit(Guid? id)
        {
            ModelState.Clear();
            var acc = DAL.AccountGet(id);

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("AccountEdit", acc),
                    title = R.R.AddNewAccount
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult AccountEdit(Account model)
        {
            if (DAL.Currencies.Count == 0)
                return RedirectToAction("Currencies");
            if (DAL.CategoryAccounts.Count == 0)
                return RedirectToAction("CategoryAccounts");

            DAL.AccountSaveAdd(model);

            return RedirectToAction("Accounts", new { categoryId = model.CategoryId });
        }

        public JsonResult AccountSave(Guid id, string name, Guid currencyId, Guid categoryId, bool deleted, string description, decimal creditLimit, bool showInRest)
        {
            DAL.AccountSaveAdd(id, name, currencyId, categoryId, deleted, description, creditLimit, showInRest);

            return new JsonResult
            {
                Data = new
                {
                    accountId = id,
                    result = true
                }
            };
        }

        public JsonResult AccountDelete(Guid id)
        {
            string error = "";
            bool delete = db.Motions.Where(x => x.AccountId == id).Count() == 0;
            try
            {
                DAL.AccountDeleteRemove(id, delete);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    accountId = id,
                    error = error
                }
            };
        }

        #endregion Account

        #region Motions

        public JsonResult MotionDetele(Guid id, bool delete)
        {
            string error = "";
            DAL.MotionDeleteRemove(id, delete);

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }       

        #endregion Motions

        public JsonResult AddMerge(Guid id)
        {
            string error = "";
            Item item = DAL.ItemGet(id);

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("AddMerge", item),
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult AddMerge(Guid[] items, Guid leave)
        {
            DAL.ItemMerge(leave, items);

            return RedirectToAction("Items");
        }

        public JsonResult AccountUserEdit(Guid? id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("AccountUserEdit", DAL.AccountGet(id)),
                    title = R.R.Users
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult AccountUserEditFind(string email)
        {
            var user = DAL.GetOwnerByName(email);
            if (user == null)
                return new HttpNotFoundResult();

            return new JsonResult
            {
                Data = new
                {
                    user = user.Email
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /*
        [HttpPost]
        public ActionResult AccountUserEdit(Guid id, string[] users)
        {
            var acc = DAL.AccountGet(id);
            if (acc == null)
                return new HttpNotFoundResult();

            foreach(var user in users)
            {
                if (String.IsNullOrEmpty(user))
                    continue;

                var usr = DAL.GetOwnerByName(user);
                if (usr == null)
                    continue;

                var item = DAL.AccountUserGet(id, user);
                if (item == null)
                    DAL.AccountUserAdd(acc.Id, usr.Id);
            }

            var list = DAL.AccountUserGetList(acc.Id);
            foreach(var item in list)
            {
                if (users.FirstOrDefault(x => x.ToLower() == item.User.Email.ToLower()) == null)
                    DAL.AccountUserGetDelete(item);
            }

            return RedirectToAction("Accounts", new { categoryId = acc.CategoryId });
        }
        */


        #region Budgets

        public ActionResult Budgets()
        {
            return View(DAL.Budgets);
        }

        public JsonResult BudgetEdit(Guid? id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("BudgetEdit", DAL.BudgetGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult BudgetEdit(Guid? id, string name)
        {
            Budget category = DAL.BudgetAddUpdate(id, name);

            return RedirectToAction("Budgets");
        }

        public JsonResult BudgetUsersEdit(Guid? id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("BudgetUsersEdit", DAL.BudgetGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public JsonResult BudgetUsersAdd(Guid id, string email)
        {
            var error = "";
            var user = db.Members.FirstOrDefault(x => x.Email.ToLower() == email.ToLower() & x.Id == x.UserId & x.Id == x.MainMemberId);
            if (user == null)
            {
                error = String.Format(R.R.UserNotFound, email);
            }
            else
            {
                var budget = db.Budgets.Find(id);
                if (db.BudgetUsers.FirstOrDefault(x => x.UserId == user.Id) == null)
                {
                    db.BudgetUsers.Add(new BudgetUser
                    {
                        Id = Guid.NewGuid(),
                        BudgetId = budget.Id,
                        UserId = user.Id
                    });
                    db.SaveChanges();
                }
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error,
                    html = this.RenderPartialView("BudgetUsersEditList", DAL.BudgetGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        
        [HttpPost]
        public JsonResult BudgetUsersDelete(Guid id)
        {
            var item = db.BudgetUsers.FirstOrDefault(x => x.Id == id);

            db.BudgetUsers.Remove(item);
            db.SaveChanges();

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("BudgetUsersEditList", DAL.BudgetGet(item.BudgetId))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult BudgetDelete(Guid id)
        {
            string error = "";
            try
            {
                DAL.CategoryAccountDelete(id);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return new JsonResult
            {
                Data = new
                {
                    error = error
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion CategoryAccount

        public ActionResult SelectBudget(Guid id)
        {
            if (Response.Cookies["BudgetId"] == null)
                Response.Cookies.Add(new HttpCookie("BudgetId", id.ToString()));
            else
                Response.Cookies["BudgetId"].Value = id.ToString();

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
