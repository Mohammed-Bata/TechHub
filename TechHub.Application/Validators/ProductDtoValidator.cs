using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .Length(2, 50)
                .WithMessage("Product name must be between 2 and 50 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Product description is required.")
                .Length(10, 500)
                .WithMessage("Product description must be between 10 and 500 characters.");
            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Product price is required.")
                .GreaterThan(0)
                .WithMessage("Product price must be greater than zero.");
            RuleFor(x => x.StockAmount)
                .NotEmpty()
                .WithMessage("Stock amount is required.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock amount cannot be negative.");
            RuleFor(x => x.Brand)
                .NotEmpty()
                .WithMessage("Brand is required.")
                .Length(2, 50)
                .WithMessage("Brand name must be between 2 and 50 characters.");
            RuleFor(x => x.ProductCode)
                .NotEmpty()
                .WithMessage("Product code is required.")
                .Length(5, 20)
                .WithMessage("Product code must be between 5 and 20 characters.");
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category ID is required.");
            RuleFor(x => x.CoverImage)
                .NotNull()
                .WithMessage("Cover image file is required.")
                .Must(file => file.Length > 0 && file.Length <= 5 * 1024 * 1024) // Max size: 5 MB
                .WithMessage("Cover image file size must be less than or equal to 5 MB.");
            
        }
    }
}
