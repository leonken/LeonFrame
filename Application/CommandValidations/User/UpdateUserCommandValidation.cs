using System;
using System.Collections.Generic;
using System.Text;
using Application.Commands.User;
using FluentValidation;

namespace Application.CommandValidations.User
{
    public class UpdateUserCommandValidation : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidation()
        {
            RuleFor(r => r.UserName).NotEmpty().WithMessage("UserName不能为空");
        }
    }
}
