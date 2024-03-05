using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abno.Data;
using Abno.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Abno.Common;
using Microsoft.AspNetCore.Identity;

namespace Abno.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> userManager;

        public RoleManager<IdentityRole> RoleManager { get; }
        public SignInManager<User> SignInManager { get; }

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }
        
        // GET: Products
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var viewModel = new ProductsViewModel
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                Products = await _context.Product
                    .OrderByDescending(product => product.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(),
                Product = new Product()
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            var subscriber = await _context.UserProducts.Where(p => p.ProductId == id).ToListAsync();

            if (product == null)
            {
                return NotFound();
            }

            ViewData["product"] = product;

            var subscriberCount = subscriber.Count();
            ViewBag.SubscriberCount = subscriberCount;
            return View();
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Product
                    .Where(p => p.Name == product.Name);
                if (existingProduct != null)
                {
                    ModelState.AddModelError("", "Product already exists");
                    TempData[Constants.Error] = "Product already exists";
                    return RedirectToAction("Index", product);
                }

                if (image != null && image.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Products", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    product.Image = $"/Products/{fileName}";
                }
                product.CreatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData[Constants.Success] = "Product created";
                return RedirectToAction("Index");
            }
            TempData[Constants.Error] = "An error occured check your input";
            return RedirectToAction("Index", product);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                IsAvailable = product.IsAvailable,
                Image = product.Image,
                Link = product.Link,
                credentials = new Credentials()
            };

            return View(viewModel);
        }


        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, EditProductViewModel viewModel, IFormFile image)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Product.FindAsync(id);

                    product.Name = viewModel.Name;
                    product.Description = viewModel.Description;
                    product.IsAvailable = viewModel.IsAvailable;
                    product.Link = viewModel.Link;

                    if (image != null && image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Products", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        product.Image = $"/Products/{fileName}";
                    }

                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Success] = "Updated successfully";

                    return RedirectToAction("Details", new { id = product.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            TempData[Constants.Error] = "An error occured";
            return View(viewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["product"] = product;
            return View();
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Product.FindAsync(id);
                if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
                {
                    return Unauthorized();
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                TempData[Constants.Success] = "Deleted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData[Constants.Error] = "An error occurred";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        // GET: Credentials/Create
        public IActionResult AddCredential(int productId)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new CredentialsViewModel
            {
                ProductId = productId
            };
            return View(viewModel);
        }

        // POST: Credentials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCredential(CredentialsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == model.ProductId);
                if (product == null)
                {
                    return NotFound();
                }
                var hashed = userManager.PasswordHasher.HashPassword(null, model.Password);
                var existingUser = _context.Credentials
                    .Where(u => u.UserName == model.UserName)
                    .FirstOrDefault();
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "User already registered");
                    return View(model);
                }

                var credentials = new Credentials
                {
                    ProductId = model.ProductId,
                    UserName = model.UserName,
                    Password = hashed
                };

                _context.Credentials.Add(credentials);
                await _context.SaveChangesAsync();

                TempData[Constants.Success] = "Credentials created successfully";
                return RedirectToAction("Details", new { id = product.Id });
            }

            return View(model);
        }
    }
}
