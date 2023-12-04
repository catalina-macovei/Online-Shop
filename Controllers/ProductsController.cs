using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            webHostEnvironment = environment;
            db = context;
        }


        // se afiseaza lista tuturor produselor impreuna cu categoria
        // HttpGet implicit aici 
        public IActionResult Index()
        {
            var products = db.Products.Include("Category");

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }


        // HttpGet implicit
        // Afisare unui produs in functie de id
        // si impreuna cu categoria din care fac parte
        public IActionResult Show(int id)
        {
            Product product = db.Products.Include("Category").Include("Comments")
                                .Where(product => product.Id == id).First();
            return View(product);
        }

        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Products/Show/" +
                comment.ProductId);
            }
            else
            {
                Product prod =
                db.Products.Include("Category").Include("Comments")
                .Where(prod => prod.Id == comment.ProductId)
                .First();
                //return Redirect("/Articles/Show/" + comm.ArticleId);
                return View(prod);
            }
    }


        // HttpGet implicit
        // Afisare formular de completare detalii produs
        // Aici se va selecta si categoria din care face parte
        public IActionResult New()
        {
            Product product = new Product();
            product.Categories = GetAllCategories();

            return View(product);
        }


        // Adaugare Produs in BD
        [HttpPost]
        public IActionResult New(Product product, IFormFile file)
        {
            
            //product.Categories = GetAllCategories();

            try
            {
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);

                    var uploadsFolder = Path.Combine("img", "products");
                    var webRootPath = webHostEnvironment.WebRootPath;

                    var uploadsFolderPath = Path.Combine(webRootPath, uploadsFolder);
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    var uniqueFileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    var relativeFilePath = Path.Combine(uploadsFolder, uniqueFileName).Replace(Path.DirectorySeparatorChar, '/');

                    product.PhotoSrc = $"/{relativeFilePath}";

                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                
                 throw new Exception("Image is required");
            }
            catch (Exception ex)
            {
                return View(product);
            }
        }

        // HttpGet implicit
        // Editare produs din BD 
        // Categoria selectata din dropdown
        // Afisare formular impreuna cu datele aferente produsului
        public IActionResult Edit(int id)
        {
            Product product = db.Products.Include("Category")
                                .Where(product => product.Id == id).First();

            product.Categories = GetAllCategories();

            return View(product);
        }

        // HttpPost
        // Adaugare produs modificat in baza de date
        [HttpPost]
        public IActionResult Edit(int id, Product requestProduct)
        {
            Product product = db.Products.Find(id);
            requestProduct.Categories = GetAllCategories();

            try 
            { 
                product.Title = requestProduct.Title;
                product.Description = requestProduct.Description;
                product.Price = requestProduct.Price;
                product.Stock = requestProduct.Stock;
                product.CategoryId = requestProduct.CategoryId;

                db.SaveChanges();

                TempData["message"] = "Produsul a fost editat!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(requestProduct);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Product requestProduct = db.Products.Find(id);

            db.Products.Remove(requestProduct);
            db.SaveChanges();

            TempData["message"] = "Produsul a fost sters!";

            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories) {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                });
            }
  
            // returnam lista de categorii
            return selectList;
        }
    }
}
