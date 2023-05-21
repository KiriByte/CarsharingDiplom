using Microsoft.AspNetCore.Identity;

namespace CarsharingProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? PassportPhotoPath { get; set; } = null;

        public string? DriverLicensePhotoPath { get; set; } = null;

        public virtual List<BankCardModel> BankCards { get; set; } = new();
        public virtual List<RentModel> RentHistory { get; set; } = new();
    }
}