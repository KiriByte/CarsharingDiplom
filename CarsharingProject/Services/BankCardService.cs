using System.Security.Claims;
using CarsharingProject.Data;
using CarsharingProject.Interfaces;
using CarsharingProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarsharingProject.Services;

public class BankCardService : IBankCard
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserModel> _userManager;

    public BankCardService(ApplicationDbContext context,
        UserManager<UserModel> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public void AddBankCard(BankCardModel card, UserModel user)
    {
        card.CardNumber = RemoveSpacesFromCardNumber(card.CardNumber);
        user.BankCards.Add(card);
        _context.SaveChanges();
    }

    public void DeleteBankCard(int cardId, string userId)
    {
        var card = _context.BankCards.Where(u => u.User.Id == userId).FirstOrDefault(c => c.Id == cardId);
        if (card != null)
        {
            _context.Remove(card);
            _context.SaveChanges();
        }
    }

    public async Task<List<BankCardModel>> GetUserCards(string userId)
    {
        var user = await _context.Users.Include(u => u.BankCards).FirstAsync(u => u.Id == userId);
        return user.BankCards;
    }

    private static string RemoveSpacesFromCardNumber(string cardId)
    {
        cardId.Replace("-", "");
        return cardId.Replace(" ", "");
    }
}