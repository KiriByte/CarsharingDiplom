using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CarsharingProject.Controllers
{
    [Authorize(Roles ="admin")]
    public class UsersWithRolesController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersWithRolesController(UserManager<UserModel> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            List<UserWithRoleViewModel> usersWithRoles = new();

            var users = _context.Users.ToList();
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault();
                usersWithRoles.Add(new UserWithRoleViewModel()
                {
                    UserName = user.UserName,
                    Role = userRole,
                    AllRoles = roles
                });
            }

            return View(usersWithRoles);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRoleAsync(string userName, string selectedRole)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, selectedRole);

            return RedirectToAction("Index"); // Пример перенаправления на действие "Index"
        }

    }
}
