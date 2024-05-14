using XLocker.Data;
using XLocker.DTOs.Business;
using XLocker.Entities;
using XLocker.Exceptions.Business;
using XLocker.Response.Common;
using Microsoft.EntityFrameworkCore;

namespace XLocker.Services
{

    public interface IBusinessService
    {
        Task<ResponseList<Business>> GetAll();
        Task<ResponseList<Business>> GetAll(GetBusinessDTO request);
        Task<Business> Create(CreateBusinessDTO request);
        Task<bool> Update(UpdateBusinessDTO request, string businessId);
        Task<bool> Delete(string businessId);
    }
    public class BusinessService : IBusinessService
    {
        private readonly DataContext _context;
        public BusinessService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseList<Business>> GetAll()
        {
            var businesses = await _context.Businesses.ToListAsync();
            return new ResponseList<Business> { TotalCount = businesses.Count, Data = businesses };
        }

        public async Task<ResponseList<Business>> GetAll(GetBusinessDTO request)
        {
            var totalCount = await _context.Businesses.CountAsync();
            var businesses = await _context.Businesses.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync();
            return new ResponseList<Business> { TotalCount = totalCount, Data = businesses };
        }

        public async Task<Business> Create(CreateBusinessDTO request)
        {
            var alreadyExist = await _context.Businesses.FirstOrDefaultAsync(u => u.Name == request.Name);
            if (alreadyExist != null)
            {
                throw new BusinessAlreadyExistException();
            }
            var business = new Business
            {
                Name = request.Name,
                Identification = request.Identification,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email
            };
            await _context.Businesses.AddAsync(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task<bool> Update(UpdateBusinessDTO request, string businessId)
        {
            var business = await _context.Businesses.FirstOrDefaultAsync(l => l.Id == businessId);
            if (business == null)
            {
                throw new BusinessDoesNotExistsException();
            }
            var alreadyExist = await _context.Businesses.FirstOrDefaultAsync(l => l.Name == request.Name && l.Id != businessId);
            if (alreadyExist != null)
            {
                throw new BusinessAlreadyExistException();
            }
            business.Identification = request.Identification;
            business.Address = request.Address;
            business.Phone = request.Phone;
            business.Email = request.Email;
            business.Name = request.Name;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Delete(string businessId)
        {
            var business = await _context.Businesses.FirstOrDefaultAsync(l => l.Id == businessId);
            if (business == null)
            {
                throw new BusinessDoesNotExistsException();
            }

            _context.Businesses.Remove(business);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}
