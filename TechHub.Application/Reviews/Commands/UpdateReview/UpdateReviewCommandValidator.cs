using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Reviews.Commands.UpdateReview
{
    public class UpdateReviewCommandValidator:AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(r => r.Content)
               .NotEmpty().WithMessage("Content is required.")
               .MinimumLength(10).WithMessage("Content must be at least 10 characters long.")
               .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");
            RuleFor(r => r.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }
}
