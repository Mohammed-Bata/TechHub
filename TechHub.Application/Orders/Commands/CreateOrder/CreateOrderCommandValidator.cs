using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator: AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator() {
            RuleFor(x => x.AddressId)
               .NotNull()
               .NotEmpty()
               .WithMessage("AddressId is required.");
        }
    }
}
