using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarsharingProject.Data;
using CarsharingProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CarsharingProject.Controllers
{
    public class BankCardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BankCardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BankCards
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var cards = _context.Users
                .Include(u => u.BankCards)
                .FirstOrDefault(u => u.Id == currentUser.Id)?
                .BankCards
                .ToList();

            return View(cards);
        }


        // GET: BankCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BankCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CardNumber,CardHolderName,ExpiryMonth,ExpiryYear,CVV")]
            BankCardModel bankCardModel)
        {
            if (ModelState.IsValid)
            {
                var existingCard = await _context.BankCards.FindAsync(bankCardModel.CardNumber);

                if (existingCard == null)
                {
                    // Карта не существует, создаем новую запись
                    _context.Add(bankCardModel);
                }
                existingCard = await _context.BankCards.FindAsync(bankCardModel.CardNumber);
                var user = await _userManager.GetUserAsync(User);
                var userWithCards = await _context.Users
                    .Include(u => u.BankCards)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);
                if (!userWithCards.BankCards.Any(c => c.CardNumber == bankCardModel.CardNumber))
                {
                    // Карта не привязана к пользователю, добавляем запись
                    userWithCards.BankCards.Add(existingCard);
                }
                else
                {
                    // Карта уже привязана к пользователю
                    ModelState.AddModelError("CardNumber", "This card is already associated with the user.");
                    return View(bankCardModel);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(bankCardModel);
        }


        // GET: BankCards/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.BankCards == null)
            {
                return NotFound();
            }

            var bankCardModel = await _context.BankCards
                .FirstOrDefaultAsync(m => m.CardNumber == id);
            if (bankCardModel == null)
            {
                return NotFound();
            }

            return View(bankCardModel);
        }

        // POST: BankCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.BankCards == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BankCards'  is null.");
            }

            var card = await _context.BankCards
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.CardNumber == id);

            if (card != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var userForDelete = card.Users.FirstOrDefault(u => u == user);
                if (userForDelete != null)
                {
                    card.Users.Remove(userForDelete);

                    if (card.Users.Count == 0)
                    {
                        // Remove the card from the database if it has no users
                        _context.BankCards.Remove(card);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BankCardModelExists(string id)
        {
            return (_context.BankCards?.Any(e => e.CardNumber == id)).GetValueOrDefault();
        }
    }
}