using FluentValidation;
using ToDoApp.API.DTO;

namespace ToDoApp.API.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x =>x.Password).NotEmpty();
        }
    }
}
