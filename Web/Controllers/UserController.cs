using Application.DTOs.Users;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            await _userService.CreateUserAsync(createUserDTO);

            return RedirectToAction("GetAllUsersAsync", "User");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(loginUserDTO);
                if (user != null)
                {
                    return View("AccountDashboard", user);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(loginUserDTO);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<GetUserDTO> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var id = _userManager.GetUserId(HttpContext.User);
            changePasswordDTO.UserId = Guid.Parse(id);
            var user = await _userService.ChangePasswordAsync(changePasswordDTO);

            return user;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }
        [HttpGet]
        public IActionResult AccountDashboard()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
