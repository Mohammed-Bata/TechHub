using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(Guid ProductId,string UserId,string Content,int Rating) : IRequest<Guid>;

}
