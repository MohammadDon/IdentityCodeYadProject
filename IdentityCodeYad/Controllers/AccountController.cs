using System.Text;
using IdentityCodeYad.Tools;
using IdentityCodeYad.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityCodeYad.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender, IViewRenderService viewRenderService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Register()
        {
            ViewBag.IsSent = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userManager.CreateAsync(new IdentityUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone
            }, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                    return View();
                }
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callBackUrl = Url.ActionLink("ConfirmEmail", "Account", new {userId = user.Id, token = token},
                Request.Scheme);
            string body = await _viewRenderService.RenderToStringAsync("_RegisterEmail", callBackUrl);
            await _emailSender.SendEmailAsync(new EmailModel(user.Email, "تایید حساب", body));
            ViewBag.IsSent = true;
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            ViewBag.IsConfirmed = result.Succeeded ? true: false;
            return View();
        }
    }
}
