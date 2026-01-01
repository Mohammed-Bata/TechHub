using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.ProductImages.Commands.AddProductImage
{
    public class AddProductImageCommandValidator: AbstractValidator<AddProductImageCommand>
    {
        public AddProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");
            RuleFor(x => x.ImageDto.Image)
                .NotNull()
                .WithMessage("Image is required.")
                .Must(file => file.Length > 0)
                .WithMessage("Image must not be empty.");
        }
    }
}
