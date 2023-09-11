using CarsharingProject.Data;
using CarsharingProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarsharingProject.Controllers;

public class RentCarControlController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserModel> _userManager;

    public RentCarControlController(ApplicationDbContext context, UserManager<UserModel> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public IActionResult Index(int id)
    {
        return View();
    }
}