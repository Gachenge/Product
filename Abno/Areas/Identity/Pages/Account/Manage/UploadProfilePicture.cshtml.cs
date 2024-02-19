using Abno.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Abno.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class UploadProfilePictureModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public void OnGet()
        {
        }

        public UploadProfilePictureModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public IFormFile Avatar { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Avatar != null && Avatar.Length > 0)
                {
                    var user = await _userManager.GetUserAsync(User);

                    var avatarDirectory = Path.Combine("wwwroot", "Avatars"); // Relative path
                    var newAvatarFileName = $"{user.Id}_avatar.jpg";
                    var newAvatarPath = Path.Combine(avatarDirectory, newAvatarFileName);

                    // Update the user's Avatar claim
                    await _userManager.AddClaimAsync(user, new Claim("Avatar", $"/Avatars/{newAvatarFileName}"));

                    // Save the avatar image
                    using (var stream = new FileStream(newAvatarPath, FileMode.Create))
                    {
                        await Avatar.CopyToAsync(stream);
                    }

                    // Update the user
                    user.Avatar = $"/Avatars/{newAvatarFileName}";
                    await _userManager.UpdateAsync(user);
                }

                return RedirectToPage("/Account/Manage/Index");
            }

            return Page();
        }

    }
}