using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtToken.DTOs.AccountDts
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName).MinimumLength(5).MaximumLength(20).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(3).MaximumLength(20).NotEmpty();
        }
    }
}
