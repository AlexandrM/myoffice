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
using MyBank.Models;
using Web.MyOffice.Models;
using System.Data.SqlClient;
using Method = System.Web.Http;

namespace Web.MyOffice.Controllers.API
{
    public class ExpendituresController : ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage ItemsCategoryGet()
        {
            using (db)
            {
                var categories = db.CategoryItems.
                    Include(category => category.Items
                    .Select(item => item.Motions
                    .Select(motion=>motion.Account))
                    ).ToList();
                var budgets = db.Budgets.ToList();
                var model = (new object()).ToDynamic();
                model.Categories = categories;
                model.Budgets = budgets;
                return ResponseObject2Json(model);
            }
        }

        [Method.HttpPut]
        public HttpResponseMessage ItemsCategoryPut(Item newItem)
        {
            Item insertItem = null;
            if (db.Items.Any(item => item.Id == newItem.Id))
            {
                insertItem = db.Items.Find(newItem.Id);
            }
            else
            {
                insertItem = db.Items.Create();
            }
            insertItem.Name = newItem.Name;
            insertItem.Category = db.CategoryItems.FirstOrDefault(x => x.Id == newItem.CategoryId);
            insertItem.Deleted = newItem.Deleted;
            insertItem.Description = newItem.Description;
            insertItem.BudgetId = newItem.BudgetId;
            using (db) { 
                if (db.Items.Any(item => item.Id == newItem.Id)) { 
                    db.Entry(insertItem).State = EntityState.Deleted;
                    db.SaveChanges();
                } 
                db.Entry(insertItem).State = EntityState.Added;
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [Method.HttpDelete]
        public HttpResponseMessage ItemDelete(Guid deleteId)
        {
            using (db)
            {
                var deleteItems = db.Items.Where(Item => Item.Id == deleteId).ToList();
                if (deleteItems.Count == 1) {
                    db.Entry(deleteItems.First()).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            return ResponseObject2Json(HttpStatusCode.Moved);
        }

        [Method.HttpGet]
        public HttpResponseMessage CategoryDelete(Guid deleteId)
        {
            using (db)
            {
                var deleteCategory = db.CategoryItems.Where(category => category.Id == deleteId).ToList();
                if (deleteCategory.Count == 1)
                {
                    db.Entry(deleteCategory.First()).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            return ResponseObject2Json(HttpStatusCode.Moved);
        }

        [Method.HttpPost]
        public HttpResponseMessage CategoryPost(CategoryItem newCategory)
        {
            using (db)
            {
                var postCategories = db.CategoryItems.Where(category => category.Id == newCategory.Id).ToList();
                if (postCategories.Count == 1)
                {
                    db.Entry(postCategories.First()).State = EntityState.Modified;
                }
                else if (postCategories.Count == 0)
                {
                    var category = db.CategoryItems.Create();
                    category.Name = newCategory.Name;
                    category.Name = newCategory.Name;
                    db.Entry(newCategory).State = EntityState.Added;
                }
                else
                {
                    return ResponseObject2Json(HttpStatusCode.NotModified);
                }
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }
    }
}