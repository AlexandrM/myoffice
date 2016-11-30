using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

using Microsoft.AspNet.Identity;

using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class GlobalDAL: IDisposable
    {
        #region GlobalDAL

        public void Dispose()
        {
            GlobalDAL.GlobalDALCount--;
            //ASE.Log.WriteLine("-GlobalDAL " + GlobalDALCount);
        }

        private static int GlobalDALCount = 0;
        public static GlobalDAL CreateInstance
        {
            get
            {
                GlobalDALCount++;
                //ASE.Log.WriteLine("+GlobalDAL " + GlobalDALCount);
                return new GlobalDAL();
            }
        }

        private DB db = new DB();

        #endregion GlobalDAL

        #region Owner

        public Budget CurrentOwner
        {
            get
            {
                Guid budgetId = Guid.Empty;
                if (HttpContext.Current.Request.Cookies["BudgetId"] != null)
                    Guid.TryParse(HttpContext.Current.Request.Cookies["BudgetId"].Value, out budgetId);

                var budget = db.BudgetUsers
                    .Where(x => (x.Budget.OwnerId == CurrentOwnerId) | (x.UserId == CurrentOwnerId & x.BudgetId == budgetId))
                    .Select(x => x.Budget).Include(x => x.Items)
                    .FirstOrDefault();

                if (budget == null) 
                    budget = db.Budgets
                        .Include(x => x.Items)
                        .FirstOrDefault(x => x.Id == budgetId & x.OwnerId == CurrentOwnerId);

                return budget;
            }
        }

        public Guid CurrentOwnerId
        {
            get
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                return id;
            }
        }

        public Member CurrentUser
        {
            get
            {
                Guid id = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                return db.Members.Find(id);
            }
        }

        public Member GetOwnerByName(string name)
        {
            return db.Members.FirstOrDefault(x => x.Email == name);
        }

        public Member GetOwnerById(Guid id)
        {
            return db.Members.FirstOrDefault(x => x.Id == id);
        }

        #endregion Owner

        #region Account 

        public Account AccountGet(Guid? accountId)
        {
            return db.Accounts
                .Include(x => x.Motions)
                .Include(x => x.Currency)
                .FirstOrDefault(x => x.Id == accountId && x.Budget.Id == CurrentOwner.Id);
        }

        public Account AccountSaveAdd(Account model)
        {
            return AccountSaveAdd(model.Id, model.Name, model.CurrencyId, model.CategoryId, model.Deleted, model.Description, model.CreditLimit, model.ShowInRest);
        }

        public Account AccountSaveAdd(Guid accountId, string name, Currency currency, CategoryAccount category, bool deleted, string description, decimal creditLimit, bool showInRest)
        {
            return AccountSaveAdd(accountId, name, currency.Id, category.Id, deleted, description, creditLimit, showInRest);
        }

        public Account AccountSaveAdd(Guid accountId, string name, Currency currency, CategoryAccount category, bool deleted, string description, decimal creditLimit)
        {
            return AccountSaveAdd(accountId, name, currency.Id, category.Id, deleted, description, creditLimit, true);
        }

        public Account AccountSaveAdd(Guid accountId, string name, Guid currencyId, Guid categoryId, bool deleted, string description, decimal creditLimit, bool showInRest)
        {
            Account item = AccountGet(accountId);
            if (item == null)
            {
                item = db.Accounts.Create();
                item.Budget = CurrentOwner;
                db.Accounts.Add(item);
            }
            item.Name = name;
            item.Currency = db.Currencies.FirstOrDefault(x => x.Id == currencyId);
            item.Category = db.CategoryAccounts.FirstOrDefault(x => x.Id == categoryId);
            item.Budget = CurrentOwner;
            item.Deleted = deleted;
            item.Description = description;
            item.CreditLimit = creditLimit;
            item.ShowInRest = showInRest;
            db.SaveChanges();

            return item;
        }

        public Account AccountDeleteRemove(Guid accountId, bool delete)
        {
            Account item = AccountGet(accountId);
            if ((!delete) || ((delete) & (!item.Deleted)))
            {
                item.Deleted = delete;
            }
            else
            {
                Motion motion = db.Motions.FirstOrDefault(x => x.Account.Id == item.Id);
                if (motion != null)
                    return null;

                db.Accounts.Remove(item);
            }
            db.SaveChanges();

            return item;
        }

        public List<Account> Accounts
        {
            get
            {
                return db.Accounts.Where(x => x.Budget.Id == CurrentOwner.Id).OrderBy(y => y.Name).ToList();
            }
        }

        public List<Account> AccountsFor(Guid? currencyid, Guid? categoryid)
        {
            var q = from x in db.Accounts where x.Budget.Id == CurrentOwner.Id select x;
            if (currencyid.HasValue)
                q = q.Where(x => x.Currency.Id == currencyid);
            if (categoryid.HasValue)
                q = q.Where(x => x.Category.Id == categoryid);

            return q
                .Include(x => x.Motions)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public List<SelectListItem> AccountsSelectList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();

                foreach (Account account in Accounts)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = account.Name;
                    item.Value = account.Id.ToString();
                    list.Add(item);
                }

                return list;
            }
        }

        #endregion Account

        #region Item

        public Item ItemGet(string name, bool create = true)
        {
            Item item = CurrentOwner.Items.FirstOrDefault(x => x.Name == name && x.Budget.Id == CurrentOwner.Id);
            if ((item == null) && (create))
            {
                item = db.Items.Create();
                item.BudgetId = CurrentOwner.Id;
                item.Name = name;
                item.Category = CategoryItemSaveAdd(Guid.NewGuid(), "-", "-");
                db.Items.Add(item);
                db.SaveChanges();
            }

            return item;
        }

        public Item ItemGet(Guid? id, bool create = true)
        {
            Item item = CurrentOwner.Items.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            if ((item == null) && (create))
            {
                item = db.Items.Create();
                item.Budget = CurrentOwner;
                item.Name = "";
                item.Category = CategoryItemSaveAdd(Guid.NewGuid(), "-", "-");
                db.Items.Add(item);
                db.SaveChanges();
            }

            return item;
        }

        public List<Item> FindItemsByName(string term)
        {
            return db.Items.Where(x => x.Budget.Id == CurrentOwner.Id & x.Name.Contains(term)).ToList();
        }

        public List<Item> Items
        {
            get
            {
                return db.Items
                    .Include(x => x.Category)
                    .Include(x => x.Motions)
                    .Where(x => x.Budget.Id == CurrentOwner.Id).OrderBy(x => x.Category.Name).OrderBy(x => x.Name).ToList();
            }
        }

        public List<Item> ItemsForCategory(Guid? categoryid)
        {
            var q = from x in db.Items where x.Budget.Id == CurrentOwner.Id select x;
            if (categoryid.HasValue)
                q = q.Where(x => x.Category.Id == categoryid);

            return q.OrderBy(x => x.Category.Name).ThenBy(x => x.Name).ToList();
        }

        public Item ItemSaveAdd(Item model)
        {
            return ItemSaveAdd(model.Id, model.Name, model.CategoryId, model.Deleted, model.Description);
        }

        public Item ItemSaveAdd(Guid id, string name, CategoryItem category, bool deleted, string description)
        {
            return ItemSaveAdd(id, name, category.Id, deleted, description);
        }

        public Item ItemSaveAdd(Guid id, string name, Guid categoryId, bool deleted, string description)
        {
            Item item = CurrentOwner.Items.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            if (item == null)
            {
                item = db.Items.Create();
                item.Budget = CurrentOwner;
                db.Items.Add(item);
            }
            item.Name = name;
            item.Category = db.CategoryItems.FirstOrDefault(x => x.Id == categoryId && x.Budget.Id == CurrentOwner.Id);
            item.Deleted = deleted;
            item.Description = description;
            db.SaveChanges();

            return item;
        }

        public Item ItemSaveAdd(Guid id, string name, string category, bool deleted)
        {
            Item item = ItemGet(id, false);
            if (item == null)
            {
                item = db.Items.Create();
                db.Items.Add(item);
                db.SaveChanges();
            }
            CategoryItem categoryItem = db.CategoryItems.FirstOrDefault(x => x.Name == category && x.Budget.Id == CurrentOwner.Id);
            if (categoryItem == null)
            {
                categoryItem = db.CategoryItems.Create();
                categoryItem.Budget = CurrentOwner;
                categoryItem.Name = category;
                db.CategoryItems.Add(categoryItem);
                db.SaveChanges();
            }
            item.Name = name;
            item.Category = categoryItem;
            item.Budget = CurrentOwner;
            item.Deleted = deleted;
            db.SaveChanges();

            return item;
        }

        public Item ItemDeleteRemove(Guid id)
        {
            Item item = ItemGet(id, false);
            Motion motion = db.Motions.FirstOrDefault(x => x.Item.Id == item.Id);
            if (motion != null)
                return null;

            db.Items.Remove(item);
            db.SaveChanges();
            return null;
        }

        public void ItemMerge(Guid main, Guid[] list)
        {
            Item mainItem = db.Items.FirstOrDefault(x => x.Budget.Id == CurrentOwner.Id && x.Id == main);
            if (mainItem == null)
                return;

            foreach (Guid item in list)
            {
                if (item == main)
                    continue;

                List<Motion> motions = db.Motions.Where(x => x.Account.Budget.Id == CurrentOwner.Id && x.Item.Id == item).ToList();
                foreach (Motion motion in motions)
                    motion.Item = mainItem;

                db.SaveChanges();
            }
        }

        public List<Motion> MotionsByItem(Guid item)
        {
            return db.Motions.Where(x => x.Account.Budget.Id == CurrentOwner.Id && x.Item.Id == item).ToList();
        }

        public List<Motion> MotionsByItem(Item item)
        {
            return db.Motions
                .Include(x => x.Account)
                .Where(x => x.Account.Budget.Id == CurrentOwner.Id && x.Item.Id == item.Id).OrderByDescending(x => x.DateTime).ToList();
        }
        

        #endregion Item

        #region Currency

        public Currency CurrencyGet(Guid id)
        {
            return db.Currencies.FirstOrDefault(x => x.Id == id && x.Owner.Id == CurrentOwner.Id);
        }

        public void CurrencyDelete(Guid id)
        {
            Currency item = db.Currencies.FirstOrDefault(x => x.Id == id && x.Owner.Id == CurrentOwner.Id);
            db.Currencies.Remove(item);
            db.SaveChanges();
        }

        public void CurrencyRateDelete(int id)
        {
            CurrencyRate item = db.CurrencyRates.FirstOrDefault(x => x.Currency.Owner.Id == CurrentOwner.Id && x.Id == id);
            db.CurrencyRates.Remove(item);
            db.SaveChanges();
        }

        public void CurrencySetPrimary(Guid id)
        {
            /*foreach (Currency currency in db.Currencies.Where(x => x.Budget.Id == CurrentOwner.Id && x.Primary).ToList())
            {
                currency.Primary = false;
                db.SaveChanges();
            }

            Currency item = db.Currencies.FirstOrDefault(x => x.Budget.Id == CurrentOwner.Id && x.Id == id);
            item.Primary = true;
            db.SaveChanges();*/
        }

        public CurrencyRate CurrencyRateGet(long id)
        {
            return db.CurrencyRates.FirstOrDefault(x => x.Currency.Owner.Id == CurrentOwner.Id && x.Id == id);
        }

        public List<CurrencyRate> CurrencyRateList(Guid id)
        {
            return db.CurrencyRates.Where(x => x.Currency.Owner.Id == CurrentOwner.Id && x.Currency.Id == id).OrderByDescending(x => x.DateTime).ToList();
        }

        /*public Currency CurrencyAddUpdate(Currency model)
        {
            return CurrencyAddUpdate(model.Id, model.Name, model.ShortName);
        }

        public Currency CurrencyAddUpdate(Guid id, string name, string shortName)
        {
            return CurrencyAddUpdate(id, name, shortName, false);
        }

        public Currency CurrencyAddUpdate(Guid id, string name, string shortName, bool primary)
        {
            Currency item = db.Currencies.FirstOrDefault(x => x.Id == id && x.Owner.Id == CurrentOwner.Id);
            if (item == null)
            {
                item = db.Currencies.Create();
                item.Owner = CurrentOwner;
                db.Currencies.Add(item);
            }
            item.Name = name;
            item.ShortName = shortName;
            //item.Primary = primary;
            db.SaveChanges(); ;

            return item;
        }*/

        public List<Currency> Currencies
        {
            get
            {
                return db.Currencies.Where(x => x.UserId == CurrentOwner.OwnerId).OrderBy(x => x.Name).ToList();
            }
        }

        public List<SelectListItem> CurrenciesSelectList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                SelectListItem item;

                /*item = new SelectListItem();
                item.Text = "---";
                item.Value = "0";
                list.Add(item);*/

                foreach (Currency cur in Currencies)
                {
                    item = new SelectListItem();
                    item.Text = cur.Name;
                    item.Value = cur.Id.ToString();
                    list.Add(item);
                }

                return list.OrderBy(x => x.Text).ToList();
            }
        }

        public CurrencyRate CurrencyRateSaveAdd(CurrencyRate model)
        {
            return CurrencyRateSaveAdd(model.Id, model.CurrencyId, model.DateTime, model.Value, 0);
        }

        public CurrencyRate CurrencyRateSaveAdd(long id, Currency currency, DateTime datetime, decimal rate, decimal multiplicity)
        {
            return CurrencyRateSaveAdd(id, currency.Id, datetime, rate, multiplicity);
        }

        public CurrencyRate CurrencyRateSaveAdd(long id, Guid currencyid, DateTime datetime, decimal rate, decimal multiplicity)
        {
            Currency currency = db.Currencies.FirstOrDefault(x => x.Owner.Id == CurrentOwner.Id && x.Id == currencyid);
            CurrencyRate item = db.CurrencyRates.FirstOrDefault(x => x.Currency.Owner.Id == CurrentOwner.Id && x.Currency.Id == currency.Id & x.Id == id);
            if (item == null)
            {
                item = db.CurrencyRates.Create();
                item.Currency = currency;
                db.CurrencyRates.Add(item);
            }
            item.DateTime = datetime;
            item.Value = rate;
            //item.Multiplicity = multiplicity;
            db.SaveChanges();

            return item;
        }

        #endregion Currency

        #region CategoryAccount

        public List<CategoryAccount> CategoryAccounts
        {
            get
            {
                return db.CategoryAccounts
                    .Include(x => x.Accounts)
                    .Where(x => x.Budget.Id == CurrentOwner.Id).ToList();
            }
        }

        public CategoryAccount CategoryAccountGet(Guid? id)
        {
            return db.CategoryAccounts.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
        }

        public CategoryAccount CategoryAccountAddUpdate(Guid? id, string name, string description)
        {
            CategoryAccount item = db.CategoryAccounts.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            if (item == null)
            {
                item = db.CategoryAccounts.Create();
                item.Budget = CurrentOwner;
                db.CategoryAccounts.Add(item);
            }
            item.Name = name;
            item.Description = description;
            db.SaveChanges(); ;

            return item;
        }

        public void CategoryAccountDelete(Guid id)
        {
            CategoryAccount item = db.CategoryAccounts.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            db.CategoryAccounts.Remove(item);
            db.SaveChanges();
        }

        public List<SelectListItem> CategoryAccountsSelectList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                SelectListItem item;

                /*item = new SelectListItem();
                item.Text = "---";
                item.Value = "0";
                item.Selected = true;
                list.Add(item);*/

                foreach (CategoryAccount cur in CategoryAccounts)
                {
                    item = new SelectListItem();
                    item.Text = cur.Name;
                    item.Value = cur.Id.ToString();
                    list.Add(item);
                }

                return list.OrderBy(x => x.Text).ToList();
            }
        }

        #endregion CategoryAccount

        #region CategoryItem

        public List<CategoryItem> CategoryItems
        {
            get
            {
                return db.CategoryItems
                    .Include(x => x.Items)
                    .Where(x => x.Budget.Id == CurrentOwner.Id).OrderBy(x => x.Name).ToList();
            }
        }

        public CategoryItem CategoryItemGet(Guid? id)
        {
            return db.CategoryItems.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
        }

        public CategoryItem CategoryItemSaveAdd(CategoryItem model)
        {
            return CategoryItemSaveAdd(model.Id, model.Name, model.Description);
        }

        public CategoryItem CategoryItemSaveAdd(Guid id, string name, string description)
        {
            return CategoryItemSaveAdd(id, name, description, false);
        }

        public CategoryItem CategoryItemSaveAdd(Guid id, string name, string description, bool isInternal)
        {
            CategoryItem item = null;
            if (id != null)
                 item = db.CategoryItems.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            if ((item == null) & (!String.IsNullOrEmpty(name)))
                item = db.CategoryItems.FirstOrDefault(x => x.Name == name && x.Budget.Id == CurrentOwner.Id);
            if (item == null)
            {
                item = db.CategoryItems.Create();
                item.BudgetId = CurrentOwner.Id;
                db.CategoryItems.Add(item);
            }
            item.Name = name;
            item.Description = description;
            item.Internal = isInternal;
            db.SaveChanges();

            return item;
        }

        public void CategoryItemDelete(Guid id)
        {
            CategoryItem item = db.CategoryItems.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            db.CategoryItems.Remove(item);
            db.SaveChanges();
        }

        public void CategoryItemSetInternal(Guid id, bool set)
        {
            CategoryItem item = db.CategoryItems.FirstOrDefault(x => x.Id == id && x.Budget.Id == CurrentOwner.Id);
            item.Internal = set;
            db.SaveChanges();
        }        

        public List<SelectListItem> CategoryItemsSelectList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                SelectListItem item;

                /*item = new SelectListItem();
                item.Text = "---";
                item.Value = "0";
                item.Selected = true;
                list.Add(item);*/

                foreach (CategoryItem cur in CategoryItems)
                {
                    item = new SelectListItem();
                    item.Text = cur.Name;
                    item.Value = cur.Id.ToString();
                    list.Add(item);
                }

                return list;
            }
        }

        public List<CategoryItem> FindItemCategoryByName(string term)
        {
            return db.CategoryItems.Where(x => x.Budget.Id == CurrentOwner.Id & x.Name.Contains(term)).ToList();
        }

        /*public string MotionInfoAdd(string id, string key, string name, string comment, string gps, string lastGps, string lastNet, string audio, string photo, string video, string datetime, string sum)
        {
            MotionInfo motionInfo = null;
            if (!String.IsNullOrWhiteSpace(key))
            {
                Guid exId = Guid.Parse(key.Replace("+", ""));
                motionInfo = db.MotionInfos.FirstOrDefault(x => x.Id == exId & x.Owner.Id == CurrentOwner.Id);
            }

            if (motionInfo == null)
                motionInfo = db.MotionInfos.Create();

            motionInfo.Owner = CurrentOwner;
            motionInfo.ExternalId = id;
            motionInfo.Name = name;
            motionInfo.Comment = comment;
            motionInfo.GPS = gps;
            motionInfo.LastGPS = lastGps;
            motionInfo.LastNET = lastNet;
            motionInfo.Audio = audio;
            motionInfo.Photo = photo;
            motionInfo.Video = video;

            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(datetime, out dt))
                motionInfo.DateTime = dt;
            else
                motionInfo.DateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

            decimal dSum = 0;
            if (decimal.TryParse(sum, out dSum))
                motionInfo.Sum = dSum;
            else
                motionInfo.Sum = 0;

            if (motionInfo.Id == null)
                db.MotionInfos.Add(motionInfo);
            db.SaveChanges();

            return motionInfo.Id.ToString();
        }*/

        #endregion CategoryItem

        #region Motion

        public Motion MotionGet(Guid id)
        {
            Motion item = db.Motions.FirstOrDefault(x => x.Id == id && x.Account.Budget.Id == CurrentOwner.Id);

            return item;
        }

        public Motion AddSaveMotion(Account account, Guid motionId, DateTime dt, string item, decimal sumP, decimal sumM, string note)
        {
            return AddSaveMotion(account.Id, motionId, dt, item, sumP, sumM, note);
        }

        public Motion AddSaveMotion(Guid accountId, Guid motionId, DateTime dt, string item, decimal sumP, decimal sumM, string note)
        {
            Motion motion;
            Account account = AccountGet(accountId);

            if (motionId == Guid.Empty)
            {
                motion = db.Motions.Create();
                motion.DateTime = DateTime.Now;
                motion.Added = DateTime.Now;
                motion.AccountId = account.Id;
            }
            else
                motion = db.Motions.FirstOrDefault(x => x.Id == motionId & x.AccountId == account.Id);

            motion.Item = ItemGet(item);
            if (dt != DateTime.MinValue)
                motion.DateTime = dt;
            motion.SumP = sumP;
            motion.SumM = sumM;
            motion.Note = note;
            if (motionId == Guid.Empty)
                db.Motions.Add(motion);
            db.SaveChanges();

            return motion;
        }

        public Motion RemoveDeleteMotion(Guid motionId, bool delete, bool remove)
        {
            Motion motion = db.Motions.FirstOrDefault(x => x.Id == motionId);
            motion.Deleted = delete;

            if (remove)
                db.Motions.Remove(motion);
            db.SaveChanges();

            return motion;
        }

        public bool MotionDeleteRemove(Guid motionId, bool delete)
        {
            Motion item = db.Motions.FirstOrDefault(x => x.Account.Budget.Id == CurrentOwner.Id && x.Id == motionId);
            return MotionDeleteRemove(item, delete);
        }

        public bool MotionDeleteRemove(Motion item, bool delete)
        {
            if ((!delete) || ((delete) & (!item.Deleted)))
                item.Deleted = delete;
            else
                db.Motions.Remove(item);

            db.SaveChanges();

            return true;
        }

        #endregion Motion

        #region MotionInfo

        /*public MotionInfo MotionInfoGet(Guid id)
        {
            MotionInfo item = db.MotionInfos.FirstOrDefault(x => x.Id == id && x.Owner.Id == CurrentOwner.Id);

            return item;
        }

        public MotionInfo MotionInfoGet(Motion motion)
        {
            MotionInfo item = db.MotionInfos.FirstOrDefault(x => x.Motion.Id == motion.Id && x.Owner.Id == CurrentOwner.Id);

            return item;
        }*/

        public List<Record> MotionInfos
        {
            get
            {
                return db.BankRecords.Where(x => x.OwnerId == CurrentOwner.Id & x.Sum != 0).OrderByDescending(x => x.DateTime).Take(50).ToList();
            }
        }

        #endregion MotionInfo

        #region AccountUser

        /*public AccountUser AccountUserGet(Guid accountId, string email)
        {
            return db.AccountUsers.FirstOrDefault(x => x.Account.Id == accountId & x.Account.BudgetId == CurrentOwner.Id & x.User.Email.ToLower() == email.ToLower());
        }

        public AccountUser AccountUserGet(Guid id)
        {
            return db.AccountUsers.FirstOrDefault(x => x.Id == id & x.UserId == CurrentOwner.Id);
        }

        public AccountUser AccountUserGetByAccount(Guid id)
        {
            return db.AccountUsers.FirstOrDefault(x => x.AccountId == id & x.UserId == CurrentOwner.Id);
        }

        public AccountUser AccountUserSetCategory(Guid accountId, Guid categoryId)
        {
            var m = db.AccountUsers.FirstOrDefault(x => x.Account.Id == accountId & x.UserId == CurrentOwner.Id);
            m.CategoryId = categoryId;
            db.SaveChanges();

            return m;
        }

        public AccountUser AccountUserAdd(Guid accountId, Guid userId)
        {
            var m = new AccountUser { AccountId = accountId, UserId = userId };
            db.AccountUsers.Add(m);
            db.SaveChanges();
            return m;
        }

        public List<AccountUser> AccountUserGetList(Guid accountId)
        {
            return db.AccountUsers.Where(x => x.Account.Id == accountId & x.Account.BudgetId == CurrentOwner.Id).ToList();
        }

        public List<AccountUser> AccountUserGetList()
        {
            return db.AccountUsers.Where(x => x.UserId == CurrentOwner.Id).ToList();
        }

        public AccountUser AccountUserGetDelete(AccountUser accountUser)
        {
            db.AccountUsers.Remove(accountUser);
            db.SaveChanges();

            return accountUser;
        }*/

        #endregion AccountUser


        #region Budgets

        public List<Budget> Budgets
        {
            get
            {
                return db.Budgets.Where(x => x.OwnerId == CurrentUser.Id).ToList();
            }
        }

        public Budget BudgetGet(Guid? id)
        {
            return db.Budgets
                .Include(x => x.Users)
                .FirstOrDefault(x => x.Id == id && x.OwnerId == CurrentUser.Id);
        }

        public Budget BudgetAddUpdate(Guid? id, string name)
        {
            Budget item = db.Budgets.FirstOrDefault(x => x.Id == id && x.OwnerId == CurrentUser.Id);
            if (item == null)
            {
                item = db.Budgets.Create();
                item.OwnerId = CurrentUser.Id;
                db.Budgets.Add(item);
            }
            item.Name = name;
            db.SaveChanges();

            return item;
        }

        public void BudgetDelete(Guid id)
        {
            Budget item = db.Budgets.FirstOrDefault(x => x.Id == id && x.OwnerId == CurrentUser.Id);
            db.Budgets.Remove(item);
            db.SaveChanges();
        }

        #endregion CategoryAccount
    }
}