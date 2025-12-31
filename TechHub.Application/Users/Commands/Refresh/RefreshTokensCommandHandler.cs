using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Users.Commands.Refresh
{
    public class RefreshTokensCommandHandler: IRequestHandler<RefreshTokensCommand,Tokens>
    {
        private readonly ITokenService _tokenService;

        public RefreshTokensCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Tokens> Handle(RefreshTokensCommand request,CancellationToken cancellationtoken)
        {
            var tokens = await _tokenService.RefreshAccessToken(request.tokens);

            if(tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
            {
                throw new Exception("invalid tokens");
            }

            return tokens;
        }
    }
}
