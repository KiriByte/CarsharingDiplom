using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarsharingProject.Controllers
{
    [Authorize(Roles = "admin, employee")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;

        public UsersController(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("user");
            var verifiedUsers = await _userManager.GetUsersInRoleAsync("verifiedUser");

            ViewData["Users"] = users;
            ViewData["verifiedUsers"] = verifiedUsers;
            return View();
        }


        public async Task<IActionResult> Verification(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return View(user);
        }

        public async Task<IActionResult> Verify(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, "verifiedUser");

            return RedirectToAction("Index");
        }
    }
}