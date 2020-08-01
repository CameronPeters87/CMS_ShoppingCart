using CMS_ShoppingCart.Models;
using CMS_ShoppingCart.Models.Entities;
using CMS_ShoppingCart.Models.ViewModels;
using CMS_ShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CMS_ShoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            /*
             * With linq 
             * var categories = (from c in db.Categories
             *                  orderby c.Sorting
             *                  select new CategoryVM(c)).ToList();
             */
            var categories = db.Categories
               .ToArray()
               .OrderBy(c => c.Sorting)
               .Select(c => new CategoryVM(c))
               .ToList();

            
            return View(categories);
        }
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            if (db.Categories.Any(c => c.Name.Equals(catName)))
                return "That title is taken is taken";

            CategoryDTO dto = new CategoryDTO();

            dto.Name = catName;
            dto.Slug = catName.Replace(" ", "-");
            dto.Sorting = 100;

            db.Categories.Add(dto);
            db.SaveChanges();

            id = dto.Id.ToString();

            return id;
        }
        // Post: Admin/Shop/ReorderCategories?id
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            int count = 1;

            CategoryDTO dto;

            foreach (var catId in id)
            {
                dto = db.Categories.Find(catId);
                dto.Sorting = count;
                db.SaveChanges();
                count++;
            }
        }
        // Post: Admin/Shop/DeleteCategory?id
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryDTO category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Categories");
        }
        // Get: Admin/Shop/AddProduct
        public async Task<ActionResult> AddProduct()
        {
            var model = new ProductVM
            {
                Categories = new SelectList(db.Categories.ToList(), "Id", "Name")
            };
            return View(model);
        }
    }
}