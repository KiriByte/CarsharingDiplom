using CarsharingProject.Data;
using CarsharingProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarsharingProject.Controllers
{
    [Authorize(Roles = "user,verifiedUser")]
    public class UserDocumentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserDocumentsController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.PassportPhotoPath != null)
            {
                string photoPassportPath = currentUser.PassportPhotoPath;
                ViewData["passportPath"] = photoPassportPath;
            }

            if (currentUser != null && currentUser.DriverLicensePhotoPath != null)
            {
                string photoLicensePath = currentUser.DriverLicensePhotoPath;
                ViewData["licensePath"] = photoLicensePath;
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadPassport(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var uniqueFileName = $"{currentUser.Id}_passport.jpg";

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Documents", "Passport");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            // Сохраняем файл на сервере
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Записываем путь к файлу в базу данных
            var imageUrl = $"/Documents/Passport/{uniqueFileName}";
            currentUser.PassportPhotoPath = imageUrl;
            await _userManager.UpdateAsync(currentUser);
            
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            await _userManager.RemoveFromRolesAsync(currentUser, userRoles);
            await _userManager.AddToRoleAsync(currentUser, "user");

            return RedirectToAction("Index", "UserDocuments", new { imageUrl });
        }

        [HttpPost]
        public async Task<IActionResult> UploadLicense(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Генерируем уникальное имя файла
            var uniqueFileName = $"{currentUser.Id}_license.jpg";

            // Определяем путь сохранения файла
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Documents", "License");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            // Сохраняем файл на сервере
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Записываем путь к файлу в базу данных
            var imageUrl = $"/Documents/License/{uniqueFileName}";
            currentUser.DriverLicensePhotoPath = imageUrl;
            await _userManager.UpdateAsync(currentUser);
            
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            await _userManager.RemoveFromRolesAsync(currentUser, userRoles);
            await _userManager.AddToRoleAsync(currentUser, "user");

            return RedirectToAction("Index", "UserDocuments");
        }
    }
}