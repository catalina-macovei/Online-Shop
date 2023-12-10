using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            //transmit received message to view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            //obtain categories from database
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;

            //transmit categories to view
            ViewBag.Categories = categories;

            return View();
        }

        public ActionResult Show(int id)
        {
            //find the category chosen
            Category category = db.Categories.Find(id);

            //transmit the object to view
            return View(category);
        }

        public ActionResult New()
        {
            // build a new object of type category
            Category cat = new Category();

            //send it to view
            return View(cat);
        }

        [HttpPost]
        public ActionResult New(Category cat)
        {
            if (ModelState.IsValid)
            {
                
                //add the object received to database
                db.Categories.Add(cat);

                //commit
                db.SaveChanges();

                TempData["message"] = "The category has been added";

                return RedirectToAction("Index");
            }

            else
            {
                return View(cat);
            }
        }

        public ActionResult Edit(int id)
        {
            //find the category object to be edited
            Category category = db.Categories.Find(id);

            //transmit it to view
            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(int id, Category requestCategory)
        {
            //find the category object to be edited
            Category category = db.Categories.Find(id);

            if (ModelState.IsValid)
            {

                //change its attributes accordingly
                category.CategoryName = requestCategory.CategoryName;

                //commit
                db.SaveChanges();

                TempData["message"] = "The category has been edited";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //find the category object to be deleted
            Category category = db.Categories.Include("Products")
                                             .Include("Products.Comments")
                                             .Where(c => c.Id == id)
                                             .First();

            //delete it from the database
            db.Categories.Remove(category);

            //commit
            db.SaveChanges();

            TempData["message"] = "The category has been deleted";
            return RedirectToAction("Index");
        }
    }
}
