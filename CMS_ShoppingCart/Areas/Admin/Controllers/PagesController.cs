using CMS_ShoppingCart.Models;
using CMS_ShoppingCart.Models.Entities;
using CMS_ShoppingCart.Models.ViewModels;
using CMS_ShoppingCart.Models.ViewModels.Pages;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CMS_ShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        public ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pages;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                pages = db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new PageVM(x)).ToList();
            }

            return View(pages);
        }

        // GET: Admin/Pages/AddPage
        public ActionResult AddPage()
        {
            return View();
        }

        // Post: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string slug;

            PagesDTO dto = new PagesDTO();

            dto.Title = model.Title;

            // Check for and set slug
            // Slug must be lower case and replace spaces with dashes
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                slug = model.Title.Replace(" ", "-").ToLower();
            }
            else
            {
                slug = model.Slug.Replace(" ", "-").ToLower();
            }

            // Check if slug and title is unique
            if (db.Pages.Any(x => x.Title.Equals(model.Title)) ||
                db.Pages.Any(x => x.Slug.Equals(slug)))
            {
                ModelState.AddModelError("", "The slug or title already exists");
                return View(model);
            }

            dto.Slug = slug;
            dto.Body = model.Body;
            dto.HasSidebar = model.HasSidebar;
            dto.Sorting = 100;

            db.Pages.Add(dto);
            db.SaveChanges();

            // Set TempData
            // This is temporary data that is saved after another view is rendered
            TempData["SM"] = "You have added a new page";

            return RedirectToAction("AddPage");
        }

        // GET: Admin/Pages/EditPage?id
        public ActionResult EditPage(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagesDTO dto = db.Pages.Find(id);

            if (dto == null)
            {
                return HttpNotFound();
            }

            //PageVM model = new PageVM()
            //{
            //    Body = dto.Body,
            //    HasSidebar = dto.HasSidebar,
            //    Id = dto.Id,
            //    Slug = dto.Slug,
            //    Sorting = dto.Sorting,
            //    Title = dto.Title
            //});

            PageVM model = new PageVM(dto);

            return View(model);
        }
        // Post: Admin/Pages/EditPage?id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int id = model.Id;
            string slug = model.Slug;

            PagesDTO dto = db.Pages.Find(id);

            if (slug != "home")
            {
                if (string.IsNullOrWhiteSpace(slug))
                {
                    slug = model.Title.Replace(" ", "-");
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-");
                }
            }

            dto.Title = model.Title;
            dto.Slug = model.Slug;
            dto.HasSidebar = model.HasSidebar;
            dto.Sorting = model.Sorting;
            dto.Body = model.Body;

            db.SaveChanges();

            TempData["SM"] = "You have edited the page!";

            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails?id
        public ActionResult PageDetails(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagesDTO dto = db.Pages.Find(id);

            if (dto == null)
            {
                return HttpNotFound();
            }

            PageVM model = new PageVM(dto);

            return View(model);
        }

        // GET: Admin/Pages/DeletePage?id
        public ActionResult DeletePage(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagesDTO page = db.Pages.Find(id);
            db.Pages.Remove(page);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public void ReorderPages(int[] id)
        {
            int count = 1;

            PagesDTO page;

            foreach (var pageId in id)
            {
                page = db.Pages.Find(pageId);
                page.Sorting = count;
                db.SaveChanges();
                count++;
            }
        }


        // GET: Admin/Pages/EditSidebar
        public ActionResult EditSidebar()
        {
            SidebarDTO dto = db.Sidebars.Find(1); // Only one sidebar

            if (dto == null)
            {
                return HttpNotFound();
            }

            SidebarVM model = new SidebarVM(dto);

            return View(model);
        }

        // Post: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            if (ModelState.IsValid)
            {
                SidebarDTO dto = new SidebarDTO
                {
                    Id = model.Id,
                    Body = model.Body
                };

                db.Entry(dto).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SM"] = "You have edited the sidebar!";

                return RedirectToAction("EditSidebar");
            }
            return View(model);
        }
    }
}