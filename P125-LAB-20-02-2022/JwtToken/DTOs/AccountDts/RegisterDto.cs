using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtToken.DTOs.AccountDts
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConformPassword { get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName).MinimumLength(5).MaximumLength(20).NotEmpty();
            RuleFor(x => x.FullName).MinimumLength(3).MaximumLength(20).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(3).MaximumLength(20).NotEmpty();
            RuleFor(x => x.ConformPassword).MinimumLength(3).MaximumLength(20).NotEmpty();
            RuleFor(x => x).Custom((x, context) =>
            {
                if (x.Password!=x.ConformPassword)
                {
                    context.AddFailure("ConformPassword", "şifre uyqun deyil");
                };
            });
        }
    }
}
