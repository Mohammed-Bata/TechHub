using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Reviews.Commands.DeleteReview
{
    public record DeleteReviewCommand(Guid ReviewId) : IRequest;

}
