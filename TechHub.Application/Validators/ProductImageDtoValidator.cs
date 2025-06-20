using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Validators
{
    public class ProductImageDtoValidator : AbstractValidator<ProductImageDto>
    {
       public ProductImageDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");
            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image is required.")
                .Must(file => file.Length > 0)
                .WithMessage("Image must not be empty.");
        }
    }
}
