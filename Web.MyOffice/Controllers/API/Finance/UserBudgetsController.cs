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
    [Authorize]
    public class UserBudgetsController : ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage List()
        {
            return ResponseObject2Json(new {
                Budgets = db.Budgets
                    .Include(x => x.Users)
                    .Where(x => x.OwnerId == UserId)
                    .OrderBy(x => x.Name)
                    .ToList()
                    .Select(x => {
                        x.Users.ForEach(z =>
                        {
                            z.User = db.Members.FirstOrDefault(c => c.UserId == UserId && c.MainMemberId == z.UserId);
                        });
                        return x;
                    })
                    .ToList(),

                UserId = UserId,
            });
        }

        [Method.HttpDelete]
        public object Delete([FromUri] Guid id)
        {
            var m = db.Budgets.FirstOrDefault(x => x.Id == id && x.OwnerId == UserId);
            if (db.Accounts.Any(x => x.Category.BudgetId == m.Id))
            {
                return new
                {
                    ok = false,
                    message = $"Budget {m.Name} have a accounts. Remove accounts first.",
                };
            }

            db.Budgets.Remove(m);
            db.SaveChanges();

            return new
            {
                ok = true,
                message = $"",
            };
        }

        [Method.HttpPut]
        public HttpResponseMessage NewBudgetPut(Budget newBudget)
        {
            newBudget.OwnerId = UserId;
            using (db)
            {
                if (newBudget.Id == Guid.Empty)
                {
                    newBudget.Id = Guid.NewGuid();
                    db.Budgets.Add(newBudget);
                    db.SaveChanges();
                }
                else
                {
                    var m = db.Budgets.FirstOrDefault(x => x.Id == newBudget.Id && x.OwnerId == UserId);
                    m.Name = newBudget.Name;
                    db.SaveChanges();
                }
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [Method.HttpGet]
        [Route("api/UserBudgets/findUser")]
        public object FindUser([FromUri] string email)
        {
            var m = db.Members.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (m != null)
            {
                return new
                {
                    userId = m.Id,
                    ok = true,
                    message = "",
                };
            }
            else
            {
                return new
                {
                    ok = false,
                    message = $"User with email {email} not found! Add in members first!",
                };
            }
        }

        [Method.HttpPost]
        [Route("api/UserBudgets/addUser")]
        public object AddUser(BudgetUser user)
        {
            var b = db.Budgets.FirstOrDefault(x => x.Id == user.BudgetId );
            var bu = db.BudgetUsers
                .Include(x => x.User)
                .FirstOrDefault(x => x.BudgetId == b.Id && x.UserId == user.UserId);

            if (bu != null)
            {
                return new
                {
                    ok = false,
                    message = $"User with email {bu.User.Email} already added!",
                };
            }

            db.BudgetUsers.Add(user);
            db.SaveChanges();

            return new
            {
                ok = true,
                message = "",
            };
        }

        [Method.HttpDelete]
        [Route("api/UserBudgets/deleteUser")]
        public object deleteUser([FromUri] Guid userId)
        {
            var bu = db.BudgetUsers
                .FirstOrDefault(x => x.Budget.OwnerId == UserId && x.UserId == userId);

            db.BudgetUsers.Remove(bu);
            db.SaveChanges();

            return new
            {
                ok = true,
                message = "",
            };
        }
    }
}
