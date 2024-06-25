using FluentValidation;
using System.Globalization;

namespace OpenAPI.Ordering.Validators
{
    public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
    {
        public PayOrderCommandValidator()
        {
            RuleFor(x => x.CardNumber)
            .CreditCard().WithMessage("Invalid card number format. A valid card number is required.")
            .NotEmpty().WithMessage("Card number is required. Example format: 1234-5678-9012-3456");

            RuleFor(x => x.ExpiryDate)
                .NotEmpty().WithMessage("Expiry date is required. Example format: MM/yyyy")
                .Must(BeAValidExpiryDate).WithMessage("Expiry date must be in the future. Example format: MM/yyyy");
        }
        private bool BeAValidExpiryDate(string expiryDate)
        {
            if (DateTime.TryParseExact(expiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                // Set the day to the last day of the month to compare with current date
                var lastDayOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
                var expiryDateWithDay = new DateTime(date.Year, date.Month, lastDayOfMonth);
                return expiryDateWithDay > DateTime.Now;
            }

            return false;
        }
    }
}
