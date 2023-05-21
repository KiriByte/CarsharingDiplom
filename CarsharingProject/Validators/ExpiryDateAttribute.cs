using System.ComponentModel.DataAnnotations;

namespace CarsharingProject.Validators
{
    public class ExpiryDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var expiryMonth = validationContext.ObjectType.GetProperty("ExpiryMonth")?.GetValue(validationContext.ObjectInstance);
            var expiryYear = validationContext.ObjectType.GetProperty("ExpiryYear")?.GetValue(validationContext.ObjectInstance);

            if (expiryMonth != null && expiryYear != null)
            {
                if (int.TryParse(expiryYear.ToString(), out int year) && int.TryParse(expiryMonth.ToString(), out int month))
                {
                    var currentYear = DateTime.Now.Year % 100;
                    var currentMonth = DateTime.Now.Month;

                    if (year > currentYear || (year == currentYear && month >= currentMonth))
                    {
                        return ValidationResult.Success;
                    }
                }
            }

            return new ValidationResult("Card has expired");
        }
    }
}
