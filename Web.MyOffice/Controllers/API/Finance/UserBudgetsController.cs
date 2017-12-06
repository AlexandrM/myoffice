using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
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
using MyBank.Models;
using Method = System.Web.Http;
using MVC = Web.MyOffice.Controllers.MyBank;

namespace Web.MyOffice.Controllers.API
{
    public class UserBudgetsController : ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage List()
        {
            var model = (new object()).ToDynamic();
            model.budgetUsers = db.BudgetUsers.Include(x => x.Budget);
            model.users = db.Members.ToList();
            return ResponseObject2Json(model);
        }

        [Method.HttpPut]
        public HttpResponseMessage NewBudgetPut(Budget newBudget)
        {
            newBudget.OwnerId = UserId;
            using (db)
            {
      
                var budgetUsers = db.BudgetUsers.Where(budgetUser => budgetUser.UserId == UserId && budgetUser.BudgetId == newBudget.Id);
                var budgetUsersExists = budgetUsers.Count() == 1;
                BudgetUser newBudgetUser = null;
                if (budgetUsersExists)
                {
                    newBudgetUser = budgetUsers.FirstOrDefault();
                    newBudgetUser.BudgetId = newBudget.Id;
                }
                else
                {
                    newBudgetUser = new BudgetUser()
                    {
                        Id = budgetUsersExists ? budgetUsers.FirstOrDefault().Id : Guid.NewGuid(),
                        BudgetId = newBudget.Id,
                        UserId = UserId,
                        Budget = newBudget
                    };
                }

                db.Entry(newBudgetUser).State = budgetUsersExists ? EntityState.Modified : EntityState.Added;
                db.SaveChanges();

                newBudget.OwnerId = UserId;
                var budgetExists = db.Budgets.Where(budget => budget.OwnerId == UserId && budget.Id == newBudget.Id).Count() == 1;
                db.Entry(newBudget).State = budgetExists ? EntityState.Modified : EntityState.Added;
                db.SaveChanges();


            }

            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        public class UpdatingUserList
        {
            public List<Member> NewUsers { set; get; }
            public List<Member> DelUsers { set; get; }
            public Guid BudgetId { set; get; } 
        }

        [Method.HttpPost]
        public HttpResponseMessage PostBudgetUserList(UpdatingUserList UpdatingList)
        {
            using (db)
            {
                var hasChanges = false;
                if (UpdatingList != null)
                {
                    if (UpdatingList.NewUsers != null && UpdatingList.NewUsers.Count > 0)
                    {
                        BudgetUser user = null;
                        foreach (var newUser in UpdatingList.NewUsers)
                        {
                            user = new BudgetUser() {BudgetId = UpdatingList.BudgetId, UserId = newUser.Id};
                            db.Entry(user).State = EntityState.Added;
                        }
                        db.SaveChanges();
                    }
                    if (UpdatingList.DelUsers != null && UpdatingList.DelUsers.Count > 0)
                    {
                        foreach (var delUser in UpdatingList.DelUsers)
                        {
                           var user = db.BudgetUsers.Where(budgetUser => budgetUser.UserId == delUser.Id && budgetUser.BudgetId == UpdatingList.BudgetId).First();
                            db.Entry(user).State = EntityState.Deleted;
                        }
                        db.SaveChanges();
                    }
                }
               
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [Method.HttpDelete]
        public HttpResponseMessage DeleteUser(Guid Id)
        {

            using (db)
            {
                var budgetUsersDel = db.BudgetUsers.Where(user => user.Id == Id).FirstOrDefault();
                db.Entry(budgetUsersDel).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }
    }
}
