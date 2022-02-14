using System.Text;
using IdentityCodeYad.Tools;
using IdentityCodeYad.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace IdentityCodeYad.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender, IViewRenderService viewRenderService, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            //ViewBag.IsSent = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.IsSent = false;
                return View();
            }

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
                    //ViewBag.IsSent = false;
                    return View();
                }
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            var mobileCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

            return RedirectToAction("ConfirmMobile", new{phone = user.PhoneNumber,token = mobileCode});

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            //string? callBackUrl = Url.ActionLink("ConfirmEmail", "Account", new { userId = user.Id, token = token },
            //    Request.Scheme);
            //string body = await _viewRenderService.RenderToStringAsync("_RegisterEmail", callBackUrl);
            //await _emailSender.SendEmailAsync(new EmailModel(user.Email, "تایید حساب", body));
            //ViewBag.IsSent = true;
            //return View();
        }

        public IActionResult ConfirmMobile(string phone, string token)
        {
            if (phone == null || token == null) return BadRequest();
            ConfirmMobileVM vm = new ConfirmMobileVM()
            {
                Phone = phone,
                Code = token
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmMobile(ConfirmMobileVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u=>u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError("","کاربری یافت نشد");
                return View();
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.SmsCode);
            if (!result)
            {
                ModelState.AddModelError(string.Empty,"کد وارد شده معتبر نمیباشد");
                return View(model);
            }

            user.PhoneNumberConfirmed = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            ViewBag.IsConfirmed = result.Succeeded ? true : false;
            return View();
        }

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginWithPhoneVM model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"کاربری با این مشخصات یافت نشد");
                return View(model);
            }

            var mobileCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

            return RedirectToAction("LoginConfirmation", new {phone = user.PhoneNumber, token = mobileCode});

            //var result =
            //    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            //if (result.Succeeded)
            //{
            //    if (Url.IsLocalUrl(returnUrl))
            //        return Redirect(returnUrl);
            //    else
            //        return RedirectToAction("Index", "Home");
            //}
            //else if (result.RequiresTwoFactor)
            //{
            //    return RedirectToAction("LoginWith2fa");
            //}
            //else if (result.IsLockedOut)
            //{
            //    ModelState.AddModelError(string.Empty, "حساب کابری شما قفل شده است");
            //    return View(model);
            //}

            //ModelState.AddModelError(string.Empty, "تلاش برای ورود نامعتبر میباشد");
            //return View();
        }

        public IActionResult LoginConfirmation(string phone,string token)
        {
            if (phone == null || token == null) return BadRequest();

            LoginConfirmationVM vm = new()
            {
                Phone = phone,
                Code = token
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> LoginConfirmation(LoginConfirmationVM model)
        {
            if(!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"کاربری یافت نشد");
                return View(model);
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.SmsCode);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "کد وارد شده معتبر نمیباشد");
                return View(model);
            }

            await _signInManager.SignInAsync(user, true);
            return Redirect("/");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            ViewBag.IsSent = false;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsSent = false;
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"تلاش برای ارسال ایمیل ناموفق بود");
                ViewBag.IsSent = false;
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callBackUrl = Url.ActionLink("ResetPassword", "Account", new { email = user.Email, token = token },
                Request.Scheme);
            string body = await _viewRenderService.RenderToStringAsync("_ResetPasswordEmail", callBackUrl);
            await _emailSender.SendEmailAsync(new EmailModel(user.Email, "بازیابی کلمه عبور", body));
            ViewBag.IsSent = true;
            return View();
        }

        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) return BadRequest();

            ResetPasswordVM model = new ResetPasswordVM()
            {
                Email = email,
                Token = token
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "تلاش برای بازیابی کلمه عبور ناموفق بود");
                return View(model);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,err.Description);
                }
            }

            return RedirectToAction("Login");
        }

        #region Remote Validations

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyUserName(string userName)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.UserName == userName);
            if (!any)
                return Json(true);

            return Json("نام کاربری مورد نظر از قبل ثبت شده است");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyEmail(string email)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.Email == email);
            if (!any)
                return Json(true);

            return Json("ایمیل مورد نظر از قبل ثبت شده است");
        }

        #endregion
    }
}
