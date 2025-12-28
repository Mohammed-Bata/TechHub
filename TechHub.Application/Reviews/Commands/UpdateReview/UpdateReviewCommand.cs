using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Reviews.Commands.UpdateReview
{
    public record UpdateReviewCommand(Guid Id,string Content,int Rating): IRequest<Guid>;
    
}
