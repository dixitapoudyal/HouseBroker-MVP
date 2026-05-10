using HouseBroker.App.Auth.D;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.App.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(r => r == Roles.Broker || r == Roles.Seeker)
                .WithMessage($"Role must be either '{Roles.Broker}' or '{Roles.Seeker}'.");
        }
    }
}
