using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

using ASE.EF;
using ASE.MVC;

using Web.MyOffice.Data;
using MyBank.Models;

namespace Web.MyOffice.Controllers.API
{
    public class ExpendituresController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage CategoryList()
        {
            using (db)
            {
                var model = new {
                    Categories = db.CategoryItems
                        .AsNoTracking()
                        .Where(x => x.Budget.OwnerId == UserId)
                        .OrderBy(x => x.Name)
                        .ToList(),

                    Budgets = db.Budgets
                    .AsNoTracking()
                    .Where(x => x.OwnerId == UserId)
                    .OrderBy(x => x.Name)
                    .ToList(),
                };

                model.Categories.ForEach(x =>
                {
                    x.Items = db.Items.Any(z => z.CategoryId == x.Id) ? new List<Item> { new Item { } } : new List<Item>();
                });

                return ResponseObject2Json(model);
            }
        }

        [HttpGet]
        public HttpResponseMessage ItemsList(Guid categoryId)
        {
            using (db)
            {
                var model = new
                {
                    Items = db.Items
                        .AsNoTracking()
                        .Where(x => x.Category.Budget.OwnerId == UserId && x.CategoryId == categoryId)
                        .OrderBy(x => x.Name)
                        .ToList(),
                };

                model.Items.ForEach(x =>
                {
                    x.Motions = db.Motions.Any(z => z.ItemId == x.Id) ? new List<Motion> { new Motion() } : new List<Motion>();
                });

                return ResponseObject2Json(model);
            }
        }

        [HttpPost]
        public HttpResponseMessage CategoryPost(CategoryItem newCategory)
        {
            using (db)
            {
                var postCategories = db.CategoryItems.FirstOrDefault(category => category.Id == newCategory.Id);
                var budget = db.Budgets.FirstOrDefault(x => x.OwnerId == UserId && x.Id == newCategory.BudgetId);

                if (postCategories != null)
                {
                    postCategories.Name = newCategory.Name;
                    postCategories.Description = newCategory.Description;
                    db.Entry(postCategories).State = EntityState.Modified;
                }
                else
                {
                    var category = db.CategoryItems.Create();
                    category.Name = newCategory.Name;
                    category.Description = newCategory.Description;
                    category.BudgetId = budget.Id;
                    db.Entry(newCategory).State = EntityState.Added;
                }
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [HttpDelete]
        public HttpResponseMessage CategoryDelete(Guid categoryId)
        {
            using (db)
            {
                var deleteCategory = db.CategoryItems.FirstOrDefault(x => x.Id == categoryId & x.Budget.OwnerId == UserId);
                if (!db.Items.Any(x => x.CategoryId == deleteCategory.Id))
                {
                    db.Entry(deleteCategory).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }

            return ResponseObject2Json(HttpStatusCode.Moved);
        }
        
        [HttpPut]
        public HttpResponseMessage ItemsCategoryPut(Item newItem)
        {
            Item insertItem = db.Items.FirstOrDefault(x => x.Id == newItem.Id && x.Category.Budget.OwnerId == UserId);
            var budget = db.Budgets.FirstOrDefault(x => x.Id == newItem.BudgetId && x.OwnerId == UserId);

            if (insertItem == null)
            {
                insertItem = db.Items.Create();
                insertItem.Id = Guid.NewGuid();
                db.Items.Add(insertItem);
            }

            insertItem.Name = newItem.Name;
            insertItem.Category = db.CategoryItems.FirstOrDefault(x => x.Id == newItem.CategoryId && x.Budget.OwnerId == UserId);
            insertItem.Deleted = newItem.Deleted;
            insertItem.Description = newItem.Description;
            insertItem.BudgetId = budget.Id;
            db.SaveChanges();

            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [HttpDelete]
        public HttpResponseMessage ItemDelete(Guid itemId, bool delete)
        {
            using (db)
            {
                var deleteItem = db.Items.FirstOrDefault(x => x.Id == itemId && x.Category.Budget.OwnerId == UserId);
                if ((deleteItem.Deleted) && (!db.Motions.Any(x => x.ItemId == deleteItem.Id)) && (delete)) {
                    db.Entry(deleteItem).State = EntityState.Deleted;
                }
                else
                {
                    deleteItem.Deleted = !deleteItem.Deleted;
                }
                db.SaveChanges();
            }

            return ResponseObject2Json(HttpStatusCode.Moved);
        }

        [HttpGet]
        public HttpResponseMessage MotionList(Guid itemId)
        {
            var model = new
            {
                Motions = db.Motions
                    .Where(x => x.ItemId == itemId && x.Item.Category.Budget.OwnerId == UserId)
                    .OrderByDescending(x => x.DateTime)
                    .ToList()
            };

            return ResponseObject2Json(model);
        }
    }
}