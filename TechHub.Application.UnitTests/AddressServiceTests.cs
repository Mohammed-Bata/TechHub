using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Services;
using TechHub.Domain;

namespace TechHub.Application.UnitTests
{
    public class AddressServiceTests
    {
        private readonly IUnitOfWork _unitOfWorkMock = NSubstitute.Substitute.For<IUnitOfWork>();
        private readonly ICacheService _cacheMock = NSubstitute.Substitute.For<ICacheService>();
        private readonly IRepository<Address> _addressRepositoryMock = NSubstitute.Substitute.For<IRepository<Address>>();
        private readonly AddressService _addressService;
        public AddressServiceTests()
        {
            _unitOfWorkMock.Addresses.Returns(_addressRepositoryMock);
            _addressService = new AddressService(_unitOfWorkMock,_cacheMock);
        }

        [Fact]
        public async Task GetAddresses_ShouldReturnCachedAddresses_WhenCacheExists()
        {
            // Arrange
            var userId = "testUserId";
            var cachedAddresses = new List<Address>
            {
                new Address { Id = 1, UserId = userId, Street = "123 Test St", City = "Test City",Governorate = "Cairo", PostalCode = "12345" }
            };
            _cacheMock.GetAsync<List<Address>>($"Addresses_{userId}").Returns(cachedAddresses);
            // Act
            var result = await _addressService.GetAddresses(userId);
            // Assert
            Assert.Equal(cachedAddresses, result);
        }

        [Fact]
        public async Task GetAddresses_ShouldReturnAddressesFromRepository_WhenCacheDoesNotExist()
        {
            // Arrange
            var userId = "testUserId";
            var addresses = new List<Address>
            {
                new Address { Id = 1, UserId = userId, Street = "123 Test St", City = "Test City",Governorate = "Cairo", PostalCode = "12345" }
            };
            _cacheMock.GetAsync<List<Address>>($"Addresses_{userId}").Returns((List<Address>)null);
            _unitOfWorkMock.Addresses.GetAll(Arg.Any<Expression<Func<Address, bool>>>()).Returns(addresses);
            // Act
            var result = await _addressService.GetAddresses(userId);
            // Assert
            Assert.Equal(addresses, result);
        }

        [Fact]
        public async Task CreateAddress()
        {
            // Arrange
            var userId = "testUserId";
            var addressDto = new AddressDto("123 Test St", "Test City", "Cairo", "12345");
           
            var newAddress = new Address
            {
                UserId = userId,
                Street = addressDto.Street,
                City = addressDto.City,
                Governorate = addressDto.Governorate,
                PostalCode = addressDto.PostalCode
            };

            await _unitOfWorkMock.Addresses.AddAsync(newAddress);
            await _unitOfWorkMock.SaveChangesAsync();
       
            await _cacheMock.RemoveAsync($"Addresses_{userId}");

            // Act
            var result = await _addressService.CreateAddress(addressDto, userId);
            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(newAddress, result);
        }

        [Fact]
        public async Task DeleteAddress()
        {
            // Arrange
            var userId = "testUserId";
            var addressId = 1;
            var address = new Address { Id = addressId, UserId = userId, Street = "123 Test St", City = "Test City", Governorate = "Cairo", PostalCode = "12345" };

            _unitOfWorkMock.Addresses.GetAsync(a => a.Id == addressId).Returns(address);
            await _unitOfWorkMock.Addresses.RemoveAsync(Arg.Any<Expression<Func<Address, bool>>>());
            await _unitOfWorkMock.SaveChangesAsync();

            await _cacheMock.RemoveAsync($"Addresses_{userId}");
            // Act
            var result = await _addressService.DeleteAddress(addressId, userId);
            // Assert
            Assert.True(result);
        }
    }
}
