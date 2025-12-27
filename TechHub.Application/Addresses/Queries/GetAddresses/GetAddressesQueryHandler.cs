using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Addresses.Queries.GetAddresses
{
    public class GetAddressesQueryHandler: IRequestHandler<GetAddressesQuery, List<AddressDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public GetAddressesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<List<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT Street, City, Governorate, PostalCode
                           FROM Addresses
                           WHERE UserId = @UserId";

            using var connection = _sqlConnectionFactory.CreateConnection();
            var result = await connection.QueryAsync<AddressDto>(sql, new { request.UserId });
            return result.ToList();
        }
    }

}
