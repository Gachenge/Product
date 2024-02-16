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

namespace Abno.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private UserRole _userRole;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; 
        }
        
        private void getUserRole()
        {
            var roleValue = User.FindFirstValue(ClaimTypes.Role);
            if (Enum.TryParse<UserRole>(roleValue, out var userRole))
            {
                _userRole = userRole;
            }
        }


        // GET: Products
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10;
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
                return Json(new { redirectTo = Url.Action(nameof(Index)) });
            }
            return View(product);
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
                Image = product.Image
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

            getUserRole();

            if (_userRole != UserRole.Admin)
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
                    var redirectUrl = Url.Action(nameof(Details), new { id = product.Id });

                    return Json(new { redirectTo = redirectUrl });
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
                getUserRole();

                if (_userRole != UserRole.Admin)
                {
                    return Unauthorized();
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();

                return Json(new { redirectTo = Url.Action(nameof(Index)) });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
