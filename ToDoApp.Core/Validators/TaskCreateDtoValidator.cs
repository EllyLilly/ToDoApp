using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTO;

namespace ToDoApp.Core.Validators
{
    public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
    {
        public TaskCreateDtoValidator() 
        {
            
            RuleFor(x => x.TaskName).NotEmpty().MaximumLength(150);
        }
    }
}
