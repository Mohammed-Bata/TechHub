using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Services
{
    public class AddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        public AddressService(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<List<Address>> GetAddresses(string userId)
        {
            var cachekey = $"Addresses_{userId}";
            var cachedaddresses = await _cache.GetAsync<List<Address>>(cachekey);
            if (cachedaddresses != null)
            {
                return cachedaddresses.ToList();
            }

            var Addresses = await _unitOfWork.Addresses.GetAll(a => a.UserId == userId);

            await _cache.SetAsync(cachekey, Addresses);

            return Addresses.ToList();
        }

        public async Task<Address> GetAddress(int id,string userId)
        {

            string cacheKey = $"Address_{id}_{userId}";
            var cachedAddress = await _cache.GetAsync<Address>(cacheKey);
            if (cachedAddress != null)
            {
                return cachedAddress;
            }
            var address = await _unitOfWork.Addresses.GetAsync(a => a.Id == id);

            await _cache.SetAsync(cacheKey, address);

            return address;
        }

        public async Task<Address> CreateAddress(AddressDto addressDto,string userId)
        {
           
            var entity = new Address
            {
                Street = addressDto.Street,
                City = addressDto.City,
                Governorate = addressDto.Governorate,
                PostalCode = addressDto.PostalCode,
                UserId = userId
            };

            await _unitOfWork.Addresses.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            string cacheKey = $"Addresses_{userId}";
            await _cache.RemoveAsync(cacheKey);

            return entity;
        }

        public async Task<AddressDto> UpdateAddress(int id,
            AddressDto addressdto)
        {

            var address = await _unitOfWork.Addresses.GetAsync(a => a.Id == id);
            address.Street = addressdto.Street;
            address.City = addressdto.City;
            address.Governorate = addressdto.Governorate;
            address.PostalCode = addressdto.PostalCode;

            await _unitOfWork.SaveChangesAsync();

            string cacheKey = $"Address_{id}_{address.UserId}";
            await _cache.RemoveAsync(cacheKey);
            cacheKey = $"Addresses_{address.UserId}";
            await _cache.RemoveAsync(cacheKey);

            return addressdto;

        }

        public async Task<bool> DeleteAddress(int id, string userId)
        {
            await _unitOfWork.Addresses.RemoveAsync(c => c.Id == id);
            await _unitOfWork.SaveChangesAsync();

            string cacheKey = $"Address_{id}_{userId}";
            await _cache.RemoveAsync(cacheKey);
            cacheKey = $"Addresses_{userId}";
            await _cache.RemoveAsync(cacheKey);

            return true;
        }
    }
}
