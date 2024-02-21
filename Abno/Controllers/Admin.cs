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

namespace Abno.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserRole _role;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        private void getUserRole()
        {
            var roleValue = User.FindFirstValue(ClaimTypes.Role);
            if (Enum.TryParse<UserRole>(roleValue, out var userRole))
            {
                _role = userRole;
            }
        }

        public async Task<IActionResult> UserManagement()
        {
            getUserRole();
            if (_role != UserRole.Admin)
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
            getUserRole();
            if (_role != UserRole.Admin)
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
            getUserRole();
            if (_role != UserRole.Admin)
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
            getUserRole();
            if (_role != UserRole.Admin)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updateUser = _context.Users.FirstOrDefault(u => u.Id == id);
                if (updateUser == null)
                {
                    return NotFound();
                }
                updateUser.Role = user.Role;
                updateUser.Primary = user.Primary;
                updateUser.Secondary = user.Secondary;

                _context.Update(updateUser);
                await _context.SaveChangesAsync();
                return Json(new { redirectTo = Url.Action(nameof(Index)) });
            }
            catch (Exception ex)
            {
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
            getUserRole();
            if (_role != UserRole.Admin)
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
            return Json(new { redirectTo = Url.Action(nameof(Index)) });
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

            List<DataSeries> userDataPoints = new List<DataSeries>();
            foreach (var usprod in userProducts)
            {
                var count = 0;
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var prod in usprod.CreatedAt.ToString("yy-MM-dd"))
                {
                    count++;
                    
                }
                dataPoints.Add(new DataPoint(usprod.CreatedAt.ToString("yy-MM-dd"), count));
            }

            var viewModel = new AdminViewModel
            {
                Product = new Product(),
                UsersPerProduct = GetUserProductsDictionary(userProducts),
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                UserProducts = userProducts,
                LineGraphData = await GetLineGraphData(),
                UserDataPoints = userDataPoints
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

        private async Task<List<DataSeries>> GetLineGraphData()
        {
            List<DataSeries> lineGraphData = new List<DataSeries>();
            foreach (var product in await _context.Product.ToListAsync())
            {
                DataSeries dataSeries = new DataSeries();
                dataSeries.Name = product.Name;
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var sub in await _context.UserProducts.ToListAsync())
                {
                    if (product.Id == sub.ProductId)
                    {
                        // Adjust DataPoint constructor based on your actual implementation
                        dataPoints.Add(new DataPoint(sub.CreatedAt.ToString("yy-MM-dd"), sub.ProductUser.ToString().Count()));
                    }
                }
                dataSeries.DataPoints = dataPoints;
                lineGraphData.Add(dataSeries);
            }
            return lineGraphData;
        }
    }
}
