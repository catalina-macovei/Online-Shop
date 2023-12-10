using Humanizer;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Stergerea unui comentariu asociat unui articol din baza de date
        [Authorize(Roles = "User,Collaborator,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            //find comment object to be deleted
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                //delete comment
                db.Comments.Remove(comm);

                //commit
                db.SaveChanges();

                return Redirect("/Products/Show/" + comm.ProductId);
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this comment";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Articles");
            }
        }

        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Edit(int id)
        {
            //find in the database the comment object to be edited
            Comment comm = db.Comments.Find(id);
            
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                //transmit to view the object
                return View(comm);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this comment";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            //find the comment object to be edited
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
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
            else
            {
                TempData["message"] = "You are not allowed to edit this comment";
                return RedirectToAction("Index");
            }

        }

    }
}
