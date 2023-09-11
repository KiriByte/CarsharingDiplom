using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarsharingProject.Data;
using CarsharingProject.Interfaces;
using CarsharingProject.Models;
using CarsharingProject.Services;
using Microsoft.AspNetCore.Identity;

namespace CarsharingProject.Controllers
{
    public class BankCardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly IBankCard _bankCardService;

        public BankCardsController(
            ApplicationDbContext context,
            UserManager<UserModel> userManager,
            IBankCard bankCardService)
        {
            _context = context;
            _userManager = userManager;
            _bankCardService = bankCardService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var bankCards = await _bankCardService.GetUserCards(currentUser.Id);
            return View(bankCards);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CardNumber,CardHolderName,ExpiryMonth,ExpiryYear")]
            BankCardModel bankCardModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                _bankCardService.AddBankCard(bankCardModel, currentUser);
                return RedirectToAction(nameof(Index));
            }

            return View(bankCardModel);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = _userManager.GetUserId(User);
            _bankCardService.DeleteBankCard(id, currentUser);
            return RedirectToAction(nameof(Index));
        }
        
    }
}