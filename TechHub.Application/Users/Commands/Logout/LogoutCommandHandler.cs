using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;

namespace TechHub.Application.Users.Commands.Logout
{
    public class LogoutCommandHandler: IRequestHandler<LogoutCommand,bool>
    {
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(LogoutCommand request,CancellationToken cancellationtoken)
        {
            await _tokenService.RevokeRefreshToken(request.tokens);
            return true;
        }

    }
}
