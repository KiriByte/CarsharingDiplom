using CarsharingProject.Models;

namespace CarsharingProject.Interfaces;

public interface IBankCard
{
    void AddBankCard(BankCardModel card, UserModel user);
    void DeleteBankCard(int cardId, string userId);
    Task<List<BankCardModel>> GetUserCards(string userId);

}