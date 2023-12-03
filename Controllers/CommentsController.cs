using Humanizer;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommentsController(ApplicationDbContext context)
        {
            db = context;
        }

        // Stergerea unui comentariu asociat unui articol din baza de date
        [HttpPost]
        public IActionResult Delete(int id)
        {
            //find comment object to be deleted
            Comment comm = db.Comments.Find(id);

            //delete comment
            db.Comments.Remove(comm);

            //commit
            db.SaveChanges();

            return Redirect("/Products/Show/" + comm.ProductId);
        }

        public IActionResult Edit(int id)
        {
            //find in the database the comment object to be edited
            Comment comm = db.Comments.Find(id);

            //transmit to view the object
            return View(comm);
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            //find the comment object to be edited
            Comment comm = db.Comments.Find(id);

            //apply the changes
            if (ModelState.IsValid)
            {

                comm.Content = requestComment.Content;

                db.SaveChanges();

                return Redirect("/Products/Show/" + comm.ProductId);
            }
            else
            {
                return View(comm);
            }

        }

    }
}
