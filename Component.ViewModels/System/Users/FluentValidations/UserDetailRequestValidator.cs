using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users.FluentValidations
{
    public class UserDetailRequestValidator : AbstractValidator<UserDetailVm>
    {
        public UserDetailRequestValidator()
        {
            RuleFor(x => x.Height).NotNull().WithMessage("Height is required")
                .GreaterThan(56).WithMessage("Height can't shorter than a newborn baby")
                .LessThan(244).WithMessage("Height can't higher than tallest guy");
            RuleFor(x => x.CurrentWeight).NotNull().WithMessage("Current weight is required")
                .GreaterThan(0).WithMessage("Current weight can't be zero");
            RuleFor(x => x.GoalWeight).NotNull().WithMessage("Goal Weight is required")
                .GreaterThan(0).WithMessage("Goal weight can't be zero");
        }
    }
}
