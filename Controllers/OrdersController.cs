using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Runtime.ConstrainedExecution;

namespace OnlineShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private void SetAccessRights()
        {

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        public IActionResult Index()
        {
            var orders = db.Orders.Where(o => o.UserId == _userManager.GetUserId(User)).OrderByDescending(o => o.Date);
            ViewBag.Orders = orders;
            return View();
        }

        public IActionResult Show(int id)
        {
            var username = (from o in db.Orders
                            join u in db.ApplicationUsers on o.UserId equals u.Id
                            select u.UserName).First();
            ViewBag.Username = username;

            ViewBag.Userid = (from o in db.Orders
                              where o.Id == id
                             select o.UserId).First();

            var oProducts = from op in db.Ordered_Products
                            join o in db.Orders on op.OrderId equals o.Id
                            join p in db.Products on op.ProductId equals p.Id
                            join c in db.Categories on p.CategoryId equals c.Id
                            where op.OrderId == id
                            select new
                            {
                                o.UserId,
                                o.Id,
                                op.ProductId,
                                p.PhotoSrc,
                                p.Title,
                                c.CategoryName                                
                            };

            ViewBag.OrderedProducts = oProducts;

            Order order = db.Orders.Where(o => o.Id == id).First();
            ViewBag.Total = order.TotalPrice;

            SetAccessRights();

            return View();
        }

        public IActionResult New()
        {
            var cproducts = db.Carts.Include("Product").Where(c => c.UserId == _userManager.GetUserId(User));
            ViewBag.Products = cproducts;

            double total = 0;
            foreach (var product in cproducts)
            {
                total += product.Quantity * product.Product.Price;
            }
            ViewBag.Total = total;
            return View();
        }

        [HttpPost]
        public IActionResult New(Order order)
        {
            //adaugam order in baza de date
            order.Date = DateTime.Now;
            order.UserId = _userManager.GetUserId(User);

            if(ModelState.IsValid)
            {
                db.Orders.Add(order);

                db.SaveChanges();

                TempData["message"] = "Your order has been registered";
                TempData["messageType"] = "alert-success";

                //pentru fiecare produs comandat actualizam stocul
                var cproducts = db.Carts.Include("Product").Where(c => c.UserId == _userManager.GetUserId(User));

                List<int?> unavailableProductIds = new List<int?>();

                foreach (var cpr in cproducts)
                {
                    Ordered_Product op = new Ordered_Product();
                    op.ProductId = cpr.ProductId;
                    op.OrderId = order.Id;
                    op.Quantity = cpr.Quantity;

                    db.Ordered_Products.Add(op);


                    Product p = db.Products.Find(cpr.ProductId);
                    p.Stock -= cpr.Quantity;

                    //daca au fost comandate toate bucatile din acel produs, preluam id ul produsului
                    if(p.Stock == 0)
                    {
                        unavailableProductIds.Add(cpr.ProductId);
                    }
                }

                db.SaveChanges();

                //stergem din cart-urile tuturor utilizatorilor produsele care au devenit indisponibile
                foreach (var unavailableProductId in unavailableProductIds)
                {
                    var unavailableProducts = db.Carts.Where(c => c.ProductId == unavailableProductId);

                    foreach (var unavailableProduct in unavailableProducts)
                    {
                        db.Remove(unavailableProduct);
                    }
                    
                }

                //commit
                db.SaveChanges();

                // stergem tot din cart
                var entries = db.Carts.Where(c => c.UserId == _userManager.GetUserId(User));
                foreach (var e in entries)
                {
                    db.Carts.Remove(e);
                }
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            else
            {
                var cproducts = db.Carts.Include("Product").Where(c => c.UserId == _userManager.GetUserId(User));
                ViewBag.Products = cproducts;

                double total = 0;
                foreach (var product in cproducts)
                {
                    total += product.Quantity * product.Product.Price;
                }
                ViewBag.Total = total;

                return View(order);
            }

            
        }
    }
}
