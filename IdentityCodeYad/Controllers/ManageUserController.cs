using IdentityCodeYad.Models;
using IdentityCodeYad.ViewModels.ManageUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityCodeYad.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ManageUserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = _userManager.Users
                .Select(u => new UsersListViewNodel()
                { UserId = u.Id, UserName = u.UserName, Email = u.Email, PhoneNumber = u.PhoneNumber }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new EditViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
            };

            var rolesList = _roleManager.Roles
                .Select(r => new RolesViewModel()
                {
                    Id = r.Id,
                    roleName = r.Name
                })
                .ToList();

            ViewBag.Roles = rolesList;
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditViewModel model, List<string> SelectedRoles)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();
            user.UserName = model.UserName;
            user.PhoneNumber = model.Phone;
            var result = await _userManager.UpdateAsync(user);

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            await _userManager.AddToRolesAsync(user, SelectedRoles);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var Model = new EditViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
            };

            var rolesList = _roleManager.Roles
                .Select(r => new RolesViewModel()
                {
                    Id = r.Id,
                    roleName = r.Name
                })
                .ToList();

            ViewBag.Roles = rolesList;
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
    }
}