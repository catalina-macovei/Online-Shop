using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(IWebHostEnvironment environment, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _env = environment;
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Conditii de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Collaborator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        // se afiseaza lista tuturor produselor impreuna cu categoria
        // HttpGet implicit aici 
        
        public IActionResult Index()
        {
            // Alegem sa afisam 6 produse pe pagina
            int _perPage = 6;

            var products = db.Products.Where(product => product.IsActive).Include("Category").Include("User");

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            
            int totalItems = products.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Products = paginatedProducts;

            return View();
        }


        // HttpGet implicit
        // Afisare unui produs in functie de id
        // si impreuna cu categoria din care fac parte
        //[Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Show(int id)
        {
            Product product = db.Products.Include("Category")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                        .Where(product => product.Id == id)
                                        .First();
            SetAccessRights();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;

            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();

                //recalculam rating-ul produsului comentat
                Product prod = db.Products.Find(comment.ProductId);

                //selectam ratingurile comentariilor produsului
                var ratings = from c in db.Comments
                              where (c.ProductId == prod.Id && c.Rating != null)
                              select c.Rating;
                //recalculam media
                var result = 0.00;

                if (ratings.Count() != 0)
                {
                    result = ratings.Average().Value;
                }

                prod.Rating = (int)Math.Round(result);

                db.SaveChanges();


                return Redirect("/Products/Show/" + comment.ProductId);
            }
            else
            {
                Product prod = db.Products.Include("Category")
                                          .Include("Comments")
                                          .Where(prod => prod.Id == comment.ProductId)
                                          .First();
                SetAccessRights();

                return View(prod);
            }
        }


        // HttpGet implicit
        // Afisare formular de completare detalii produs
        // Aici se va selecta si categoria din care face parte
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult New()
        {
            Product product = new Product();

            product.Categories = GetAllCategories();

            return View(product);
        }


        // Adaugare Produs in BD
        [Authorize(Roles = "Collaborator,Admin")]
        [HttpPost]
        public async Task<IActionResult> New(Product product, IFormFile file)
        {

            //product.Categories = GetAllCategories();

            // iau userId utilizator care adauga produs in BD
            
            product.UserId = _userManager.GetUserId(User);

            var res = await SaveImage(file);

            if (res == null)
            {
                ModelState.AddModelError("PhotoSrc", "Please load a jpg, jpeg, png, and gif file type.");
            } 
            else
            {
                product.PhotoSrc = res;
            }

            

            if (ModelState.IsValid)
            {
                var isAdmin = User.IsInRole("Admin");

                product.IsActive = isAdmin;

                db.Products.Add(product);
                db.SaveChanges();

                TempData["message"] = isAdmin ? "Product has been added!" : "Reviewing your product!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Index");
            }
            else
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        // Log or debug the error messages
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        // Log these error messages for debugging purposes
                    }
                }

                product.Categories = GetAllCategories(); 
                return View(product);
            }
        }

        // HttpGet implicit
        // Editare produs din BD 
        // Categoria selectata din dropdown
        // Afisare formular impreuna cu datele aferente produsului
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult Edit(int id)
        {
            Product product = db.Products.Include("Category")
                                .Where(product => product.Id == id)
                                .First();

            product.Categories = GetAllCategories();

            if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "You're unable to modify a product you didn't add!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        // HttpPost
        // Adaugare produs modificat in baza de date
        [Authorize(Roles = "Collaborator,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile file)
        {
            Product product = db.Products.Find(id);
            requestProduct.Categories = GetAllCategories();

            product.UserId = _userManager.GetUserId(User);

            var res = await SaveImage(file);

            if (res == null)
            {
                ModelState.AddModelError("PhotoSrc", "Please load a jpg, jpeg, png or gif file type.");
            }
            else
            {
                product.PhotoSrc = res;
            }


            if (ModelState.IsValid)
            {
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    product.Title = requestProduct.Title;
                    product.Description = requestProduct.Description;
                    product.Price = requestProduct.Price;
                    product.Stock = requestProduct.Stock;
                    product.CategoryId = requestProduct.CategoryId;

                    if (file != null && file.Length > 0)
                    {
                        product.PhotoSrc = await SaveImage(file);

                    }
                    TempData["message"] = "Product has been modified!";
                    TempData["messageType"] = "alert-success";

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You're unable to modify a product you didn't add!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }

            }
            else
            {
                return View(requestProduct);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateProductIsActive(int Id)
        {
            Product product = db.Products.Find(Id);
            if (product == null)
            {
                return RedirectToAction("InReview");
            }
            product.IsActive = true; // poate fi isActiev
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Collaborator,Admin")]
        public IActionResult Delete(int id)
        {
            Product requestProduct = db.Products.Include("Comments")
                                         .Where(product => product.Id == id)
                                         .First();
            if (requestProduct.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Products.Remove(requestProduct);
                db.SaveChanges();

                TempData["message"] = "Product has been deleted!";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "You're unable to delete a product you didn't add!";
                TempData["messageType"] = "alert-danger";
            }

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


        private async Task<string?> SaveImage(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine("img", "products");
            var webRootPath = _env.WebRootPath;

            var uploadsFolderPath = Path.Combine(webRootPath, uploadsFolder);

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var relativeFilePath = Path.Combine(uploadsFolder, uniqueFileName).Replace(Path.DirectorySeparatorChar, '/');
            return $"/{relativeFilePath}";
        }

        [Authorize(Roles="Admin")]
        public IActionResult InReview()
        {
            // Alegem sa afisam 6 produse pe pagina
            int _perPage = 6;

            var products = db.Products.Where(product => !product.IsActive).Include("Category").Include("User");

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            int totalItems = products.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Products = paginatedProducts;

            return View();
        }

        public IActionResult InProgress()
        {
            // Alegem sa afisam 6 produse pe pagina
            int _perPage = 6;
     
            var products = db.Products.Where(product => !product.IsActive && _userManager.GetUserId(User) == product.UserId).Include("Category").Include("User");

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            int totalItems = products.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Products = paginatedProducts;

            return View();
        }
    }
}
