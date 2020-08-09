using CMS_ShoppingCart.Models;
using CMS_ShoppingCart.Models.Entities;
using CMS_ShoppingCart.Models.ViewModels;
using CMS_ShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
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
                Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name")
            };
            return View(model);
        }

        // Post: Admin/Shop/AddProduct
        [HttpPost]
        public async Task<ActionResult> AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name");
                return View(model);
            }
            
            // Make sure product name is unique
            if(db.Products.Any(x => x.Name == model.Name))
            {
                model.Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name");
                ModelState.AddModelError("", "That product Name is taken");
                return View(model);
            }

            CategoryDTO category = await db.Categories.FirstOrDefaultAsync(c => c.Id == model.CategoryId);

            db.Products.Add(new ProductDTO
            {
                Name = model.Name,
                Slug = model.Name.Replace(" ", "-"),
                Description = model.Description,
                CategoryId = model.CategoryId,
                CategoryName = category.Name,
                Category = category,
                Price = model.Price
            });

            await db.SaveChangesAsync();

            // Get the product id
            int productId = await db.Products.OrderByDescending(p => p.Id)
                .Select(p => p.Id).FirstOrDefaultAsync();

            TempData["SM"] = "You have Added a product";

            #region Product Image

            /*
             * Create necessary directories
             */
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads",
                Server.MapPath(@"\")));

            // pathString1 = "\Images\Uploads\Products";
            var pathString1 = Path.Combine(originalDirectory.ToString(),
                "Products");

            // Need specific product holder
            var pathString2 = Path.Combine(originalDirectory.ToString(),
                "Products\\" + productId.ToString());
            // Need main image thumb
            var pathString3 = Path.Combine(originalDirectory.ToString(),
                "Products\\" + productId.ToString() + "\\Thumbs");
            // For Gallery Images
            var pathString4 = Path.Combine(originalDirectory.ToString(),
                "Products\\" + productId.ToString() + "\\Gallery");
            // For Gallery thumbs
            var pathString5 = Path.Combine(originalDirectory.ToString(),
                "Products\\" + productId.ToString() + "\\Gallery\\Thumbs" + productId.ToString());

            // Check if these Directories exists. If not, create them
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);


            // Check if file was uploaded
            if(file != null && file.ContentLength > 0)
            {

                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if(ext != "image/jpg" && 
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/png" &&
                    ext != "image/gif")
                {
                    model.Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name");
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                    return View(model);
                }

                // Init image name
                string imgName = file.FileName;

                // Save image name to dto
                var product = await db.Products.FindAsync(productId);
                product.ImageName = imgName;

                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Set original or thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imgName);
                var path2 = string.Format("{0}\\{1}", pathString3, imgName);

                // Save original
                file.SaveAs(path);

                // Create and Save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion

            return RedirectToAction("AddProduct");
        }
    }
}