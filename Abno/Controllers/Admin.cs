using Abno.Data;
using Abno.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Internal;
using Abno.Common;

namespace Abno.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<User> _signManager;
        private UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager,
            SignInManager<User> signManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
        }
        
        public async Task<IActionResult> UserManagement()
        {
            if (!(_signManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var users = await _context.Users.ToListAsync();
            var userSubscriptions = new Dictionary<User, List<Product>>();
            var userProducts = await _context.UserProducts.ToListAsync();

            foreach (var user in users)
            {
                var subscriptions = userProducts.Where(up => up.UserId == user.Id)
                                                .Select(up => _context.Product.FirstOrDefault(p => p.Id == up.ProductId))
                                                .ToList();

                userSubscriptions.Add(user, subscriptions);
            }

            ViewData["Subscriptions"] = userSubscriptions;
            ViewData["users"] = users;
            return View();
        }

        public async Task<IActionResult> UserDetails(string? id)
        {
            if (!(_signManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => id == u.Id);
            if (user != null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> EditUser(string? id)
        {
            if (!(_signManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.u = user;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("Id, Role, Primary, Secondary")] User user)
        {
            if (!(_signManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                TempData[Constants.Error] = "An error occurred";
                return BadRequest(ModelState);
            }

            try
            {
                var updateUser = _context.Users.FirstOrDefault(u => u.Id == id);
                if (updateUser == null)
                {
                    return NotFound();
                }
                //updateUser.Role = user.Role;
                
                updateUser.Primary = user.Primary;
                updateUser.Secondary = user.Secondary;

                _context.Update(updateUser);
                await _context.SaveChangesAsync();
                TempData[Constants.Success] = "User updated successfully";

                return RedirectToAction("UserManagement");
            }
            catch (Exception ex)
            {
                TempData[Constants.Error] = "An error occurred";
                return StatusCode(500);
            }
        }


        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.u = user;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!(_signManager.IsSignedIn(User) || (User.IsInRole("Admin"))))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData[Constants.Success] = "Deleted successfully";
            return RedirectToAction("UserManagement");
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(u => u.Id == id);

        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Product.CountAsync();

            var userProducts = await _context.UserProducts
                .Include(up => up.Product)
                .Include(up => up.ProductUser)
                .ToListAsync();

            var subscriptionsPerDayPerProduct = userProducts
                .GroupBy(up => new { ProductId = up.Product.Id, Date = up.CreatedAt.Date })
                .Select(g => new { ProductId = g.Key.ProductId, Date = g.Key.Date, Count = g.Count() })
                .ToList();

            var lineGraphData = new Dictionary<String, List<DataPoint>>();
            foreach (var item in subscriptionsPerDayPerProduct)
            {
                var product = await _context.Product.FindAsync(item.ProductId);
                if (product != null)
                {
                    if (!lineGraphData.ContainsKey(product.Name))
                    {
                        lineGraphData[product.Name] = new List<DataPoint>();
                    }
                    lineGraphData[product.Name].Add(new DataPoint(item.Date.ToShortDateString(), item.Count));
                }
            }

            Dictionary<string, int> productSubscriberCounts = new Dictionary<string, int>();

            foreach (var userProduct in userProducts)
            {
                var productName = userProduct.Product.Name;

                if (productSubscriberCounts.ContainsKey(productName))
                {
                    productSubscriberCounts[productName]++;
                }
                else
                {
                    productSubscriberCounts[productName] = 1;
                }
            }

            int totalSubscribers = productSubscriberCounts.Sum(kvp => kvp.Value);

            Dictionary<string, double> productSubscriberPercentages = new Dictionary<string, double>();

            foreach (var kvp in productSubscriberCounts)
            {
                double percentage = ((double)kvp.Value / totalSubscribers) * 100;
                productSubscriberPercentages.Add(kvp.Key, percentage);
            }

            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var kvp in productSubscriberPercentages)
            {
                dataPoints.Add(new DataPoint(kvp.Key, kvp.Value));
            }


            var viewModel = new AdminViewModel
                {
                    Product = new Product(),
                    UsersPerProduct = GetUserProductsDictionary(userProducts),
                    TotalProducts = totalProducts,
                    TotalUsers = totalUsers,
                    UserProducts = userProducts,
                    DataPoints = JsonConvert.SerializeObject(dataPoints),
                    LineGraphData = JsonConvert.SerializeObject(lineGraphData)

            };

                return View(viewModel);
            }


            private Dictionary<Product, List<User>> GetUserProductsDictionary(List<UserProduct> userProducts)
        {
            Dictionary<Product, List<User>> usersPerProduct = new Dictionary<Product, List<User>>();
            foreach (var userProduct in userProducts)
            {
                if (!usersPerProduct.ContainsKey(userProduct.Product))
                {
                    usersPerProduct[userProduct.Product] = new List<User>();
                }
                usersPerProduct[userProduct.Product].Add(userProduct.ProductUser);
            }
            return usersPerProduct;
        }

        //// GET: Products/Create
        //[Authorize]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddNewUserViewModel user)
        {
            if (!(_signManager.IsSignedIn(User) || !User.IsInRole("Admin")))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var existingEmail = _context.Users.Where(u => u.Email == user.Email).FirstOrDefault();
                if (existingEmail != null)
                {
                    ModelState.AddModelError("", "Email is already registered");
                    TempData[Constants.Error] = "An error occurred";
                    return View();
                }
                var usr = new User { UserName=user.Email, Email=user.Email };
                var result = await _userManager.CreateAsync(usr, user.Password);

                if (result.Succeeded)
                {
                    TempData[Constants.Success] = "User added successfully";
                    return RedirectToAction("UserManagement");
                }
             
            }
            TempData[Constants.Error] = "An error occurred";
            return View("UserManagement");
        }
    }
}
