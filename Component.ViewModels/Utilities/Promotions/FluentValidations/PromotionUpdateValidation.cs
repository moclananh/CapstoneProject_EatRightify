using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Promotions.FluentValidations
{
    public class PromotionUpdateValidation : AbstractValidator<PromotionUpdateRequest>
    {
        public PromotionUpdateValidation()
        {
            RuleFor(x => x.DiscountPercent)
           .InclusiveBetween(1, 100) 
           .WithMessage("The value must be between 1 and 100.");

            RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("The Name must not be null.");
        }
    }
}
