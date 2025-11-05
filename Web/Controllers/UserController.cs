using Application.DTOs.Users;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Filters;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(CreateUserDTO createUserDTO)
        {
            var userResult = await _userService.CreateUserAsync(createUserDTO);

            if (userResult.IsFailure)
            {
                TempData["ErrorMesage"] = userResult.Error.Description;
                return View(createUserDTO);
            }

            return RedirectToAction("GetAllUsers", "User");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            var user = await _userService.AuthenticateAsync(loginUserDTO);
            if (user.IsSuccess)
            {
                return View("AccountDashboard", user.Value);
            }

            TempData["ErrorMessage"] = user.Error.Description;
            return View(loginUserDTO);
        }
        [HttpPost]
        [CustomAuthorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _userService.Logout();

            if (result.IsFailure)
            {
                TempData["ErrorMesage"] = result.Error.Description;
                return RedirectToAction("AccountDashboard", "User");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [CustomAuthorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var id = _userManager.GetUserId(HttpContext.User);
            changePasswordDTO.UserId = Guid.Parse(id);
            var user = await _userService.ChangePasswordAsync(changePasswordDTO);

            if (user.IsFailure)
            {
                TempData["ErrorMesage"] = user.Error.Description;
                return RedirectToAction("AccountDashboard");
            }

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("AccountDashboard");
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();

            return View(users);
        }
        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> AccountDashboard()
        {
            return View();
        }

    }
}
