using IdentityCodeYad.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityCodeYad.Controllers
{
    public class ManageRoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ManageRoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string name)
        {
            var role = new ApplicationRole{Name = name};
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded) 
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) 
                return NotFound();
            await _roleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) 
                return NotFound();

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) 
                return NotFound();
            role.Name = name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded) 
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(role);
        }
    }
}