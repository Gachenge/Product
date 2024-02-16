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

        public async Task<IActionResult> Index()
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
                Console.WriteLine($"An error occurred while editing user: {ex.Message}");
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

        private bool UserExists(string id) {
            return _context.Users.Any(u => u.Id == id);
     
       }
    }
}
