using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abno.Data;
using Abno.Models;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using Abno.Common;
using Microsoft.AspNetCore.Identity;

namespace Abno.Controllers
{
    public class UserProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SignInManager<User> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public UserProductsController(ApplicationDbContext context, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }


        // GET: UserProducts
        public async Task<IActionResult> Index()
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var productUserViewModel = new SubscriberViewModel
            {
                Products = await _context.Product.ToListAsync(),
                UserProducts = _context.UserProducts
            .Include(up => up.Product)
            .Include(up => up.ProductUser)
            .ToList(),
                userProduct = new UserProduct()
            };
            return View(productUserViewModel);
        }


        // GET: UserProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Include(u => u.UserProducts).ToListAsync();
            
            var userProduct = await _context.UserProducts
                .Include(up => up.Product)
                .FirstOrDefaultAsync(up => up.ProductId == id);

            var subscribers = await _context.Users
                .Where(u => u.UserProducts.Any(up => up.ProductId == id))
                .ToListAsync();

            if (userProduct == null)
            {
                return NotFound();
            }

            

            var viewModel = new UserProductViewModel
            {
                userProduct = userProduct,
                subscribers= subscribers
            };

            return View(viewModel);
        }

        // GET: UserProducts/Create
        public async Task<IActionResult> Create(int id)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var product = _context.Product.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            var viewModel = new AddSubscriberViewModel
            {
                UserSelect = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName"),
                userProduct = new UserProduct { ProductId=product.Id },
                Product = product
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddSubscriberViewModel viewModel)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var existingSubscription = await _context.UserProducts
                    .AnyAsync(up => up.ProductId == viewModel.userProduct.ProductId && up.UserId == viewModel.userProduct.UserId);

                if (existingSubscription)
                {
                    ModelState.AddModelError("", "User is already subscribed to this product.");

                    ViewBag.UserId = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", viewModel.userProduct.UserId);
                    TempData[Constants.Error] = "User already subscribed";
                    return View(viewModel);
                }

                viewModel.userProduct.CreatedAt = DateTime.Now;

                _context.Add(viewModel.userProduct);
                await _context.SaveChangesAsync();
                TempData[Constants.Success] = "New subscription";
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", viewModel.userProduct.UserId);
            TempData[Constants.Error] = "An error occurred";
            return View();
        }

        // GET: UserProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var userProduct = await _context.UserProducts.FindAsync(id);
            if (userProduct == null)
            {
                return NotFound();
            }
            ViewBag.userProduct = userProduct;
            return View();
        }

        // POST: UserProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UpdatedAt,UserId,ProductId")] UserProduct userProduct)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            if (id != userProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userProduct.UpdatedAt = DateTime.Now;
                    _context.Update(userProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProductExists(userProduct.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData[Constants.Success] = "updated successfully";
                return RedirectToAction("Index");
            }
            return View(userProduct);
        }

        // GET: UserProducts/Delete/5
        public IActionResult Delete(int? id)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var userProduct = _context.UserProducts.FirstOrDefault(u => u.Id == id.GetValueOrDefault());

            if (userProduct == null)
            {
                return NotFound();
            }

            ViewBag.userProductId = userProduct.Id;

            return RedirectToAction("Index");
        }

        // POST: UserProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!(SignInManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var userProduct = await _context.UserProducts.FindAsync(id);
            if (userProduct == null)
            {
                return Unauthorized();
            }
            _context.UserProducts.Remove(userProduct);
            await _context.SaveChangesAsync();
            TempData[Constants.Success] = "Subscription stopped successfully";
            return RedirectToAction("Index");
        }

        private bool UserProductExists(int id)
        {
            return _context.UserProducts.Any(e => e.Id == id);
        }
    }
}
