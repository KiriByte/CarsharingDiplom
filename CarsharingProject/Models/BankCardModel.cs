using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CarsharingProject.Validators;

namespace CarsharingProject.Models
{
    public class BankCardModel
    {
        
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Card number is required")]
        [CreditCard(ErrorMessage = "Invalid credit card number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Cardholder name is required")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "Expiry month is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])$",
            ErrorMessage = "Expiry month must be a two-digit number between 01 and 12")]
        [ExpiryDate(ErrorMessage = "Card has expired")]
        public string ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Expiry year is required")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "Expiry year must be a two-digit number")]
        public string ExpiryYear { get; set; }

        public virtual UserModel User { get; set; }= new();
    }
}