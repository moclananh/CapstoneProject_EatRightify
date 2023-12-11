
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;


namespace Component.ViewModels.Utilities.Promotions.FluentValidations
{
    public class PromotionCreateValidation : AbstractValidator<PromotionCreateRequest>
    {
        public PromotionCreateValidation()
        {
           // Greater than a particular value
            RuleFor(customer => customer.DiscountPercent).GreaterThan(0);

            //Greater than another property
            RuleFor(customer => customer.DiscountPercent).LessThan(100);

            RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("The Name must not be null.");
        }

    }
}
