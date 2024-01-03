using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var carts = db.Carts.Include("User")
                                   .Include("Product")
                                   .Include("Product.Category")
                                   .Include("Product.User")
                                   .Where(cart => cart.UserId == _userManager.GetUserId(User));
        
            if(carts.Any())
            {
                ViewBag.Empty = false;
                ViewBag.Carts = carts;
                double total = 0.00;
                foreach(var cart in carts)
                {
                    total += cart.Quantity * cart.Product.Price;
                }

                ViewBag.Total = total;

            }
            else
            {
                ViewBag.Empty = true;
                ViewBag.Total = 0;
            }

            


            return View();
        }

        [HttpPost]
        public IActionResult AddToCart([FromForm] Cart cart)
        {
            cart.UserId = _userManager.GetUserId(User);

            var alreadyInCart = db.Carts.Where(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId);

            if (alreadyInCart.Any())
            {
                TempData["message"] = "You cannot add a product to the cart if it is already there";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Products/Show/" + cart.ProductId);
            }
            else
            {
                var product = db.Products.Where(p => p.Id == cart.ProductId).First();
                if (cart.Quantity > product.Stock)
                {
                    TempData["message"] = "There aren't enough products in stock";
                    TempData["messageType"] = "alert-danger";

                    return Redirect("/Products/Show/" + cart.ProductId);
                }
                else
                {
                    if (ModelState.IsValid)
                    {

                        //add the object received to database
                        db.Carts.Add(cart);

                        //commit
                        db.SaveChanges();

                        TempData["message"] = "The product has been added to your cart";
                        TempData["messageType"] = "alert-success";

                        return Redirect("/Products/Show/" + cart.ProductId);
                    }

                    else
                    {
                        return View(cart);
                    }
                }
                
            }
            
        }

        [HttpPost]
        public IActionResult EmptyCart()
        {
            var entries = db.Carts.Where(c => c.UserId == _userManager.GetUserId(User));
            foreach (var e in entries)
            {
                db.Carts.Remove(e);
            }

            db.SaveChanges();

            TempData["message"] = "Your cart has been emptied";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

        public IActionResult DeleteProduct(int id)
        {
            Cart cart = db.Carts.Where(c => c.UserId == _userManager.GetUserId(User) && c.ProductId == id).First();
            db.Carts.Remove(cart);
            db.SaveChanges();

            TempData["message"] = "The product has been deleted from your cart";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditQuantity(Cart requestCart)
        {
            Cart cart = db.Carts.Where(c => c.UserId == _userManager.GetUserId(User) && c.ProductId == requestCart.ProductId).First();

            var product = db.Products.Where(p => p.Id == requestCart.ProductId).First();

            if (requestCart.Quantity > product.Stock)
            {
                TempData["message"] = "There aren't enough products in stock";
                TempData["messageType"] = "alert-danger";

                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    cart.Quantity = requestCart.Quantity;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return RedirectToAction("Index");
                }
            }

        }
    }
}
