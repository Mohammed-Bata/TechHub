using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Addresses.Queries.GetAddress
{
    public class GetAddressQueryHandler: IRequestHandler<GetAddressQuery,AddressDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetAddressQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<AddressDto> Handle(GetAddressQuery request,CancellationToken cancellationToken)
        {
            const string sql = @"SELECT Street, City, Governorate, PostalCode
                           FROM Addresses
                           WHERE Id = @Id AND UserId = @UserId";

            using var connection = _sqlConnectionFactory.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<AddressDto>(sql, new { request.Id, request.UserId });

            if (result == null) {
                throw new NotFoundException("Address Not Found");
            }
            return result;
        }
    }
}
