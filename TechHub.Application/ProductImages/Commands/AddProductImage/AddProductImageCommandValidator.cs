using FluentValidation;
using TechHub.Application.Common;

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
                .Must(file => ImageValidator.IsValidImage(file))
                .WithMessage("Invalid image file. Only JPG, PNG, and WebP are allowed")
                .Must(file => file.Length <= 5 * 1024 * 1024)
                .WithMessage("File size must not exceed 5MB");
        }


    }
}
