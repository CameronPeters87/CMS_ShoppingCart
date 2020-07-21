using CMS_ShoppingCart.Models;
using CMS_ShoppingCart.Models.Entities;
using CMS_ShoppingCart.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}