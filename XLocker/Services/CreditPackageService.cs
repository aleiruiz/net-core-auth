using Microsoft.EntityFrameworkCore;
using XLocker.Data;
using XLocker.DTOs.CreditPackage;
using XLocker.Entities;
using XLocker.Exceptions.CreditPackage;
using XLocker.Response.Common;

namespace XLocker.Services
{

    public interface ICreditPackageService
    {
        Task<ResponseList<CreditPackage>> GetAll();
        Task<ResponseList<CreditPackage>> GetAll(GetCreditPackageDTO request);
        Task<CreditPackage> Create(CreateCreditPackageDTO request);
        Task<bool> Update(UpdateCreditPackageDTO request, string creditPackageId);
        Task<bool> Delete(string creditPackageId);
    }
    public class CreditPackageService : ICreditPackageService
    {
        private readonly DataContext _context;
        public CreditPackageService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseList<CreditPackage>> GetAll()
        {
            var packages = await _context.CreditPackages.ToListAsync();
            return new ResponseList<CreditPackage> { TotalCount = packages.Count, Data = packages };
        }

        public async Task<ResponseList<CreditPackage>> GetAll(GetCreditPackageDTO request)
        {
            var totalCount = await _context.CreditPackages.CountAsync();
            var packages = await _context.CreditPackages.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync();
            return new ResponseList<CreditPackage> { TotalCount = totalCount, Data = packages };
        }

        public async Task<CreditPackage> Create(CreateCreditPackageDTO request)
        {
            var alreadyExist = await _context.CreditPackages.FirstOrDefaultAsync(u => u.Name == request.Name);
            if (alreadyExist != null)
            {
                throw new CreditPackageAlreadyExistException();
            }
            var packages = new CreditPackage
            {
                Name = request.Name,
                CreditQuantity = request.CreditQuantity,
                Price = request.Price,
            };
            await _context.CreditPackages.AddAsync(packages);
            await _context.SaveChangesAsync();
            return packages;
        }

        public async Task<bool> Update(UpdateCreditPackageDTO request, string creditPackageId)
        {
            var packages = await _context.CreditPackages.FirstOrDefaultAsync(l => l.Id == creditPackageId);
            if (packages == null)
            {
                throw new CreditPackageDoesNotExistsException();
            }
            var alreadyExist = await _context.CreditPackages.FirstOrDefaultAsync(l => l.Name == request.Name && l.Id != creditPackageId);
            if (alreadyExist != null)
            {
                throw new CreditPackageAlreadyExistException();
            }
            packages.CreditQuantity = request.CreditQuantity;
            packages.Price = request.Price;
            packages.Name = request.Name;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Delete(string creditPackageId)
        {
            var packages = await _context.CreditPackages.FirstOrDefaultAsync(l => l.Id == creditPackageId);
            if (packages == null)
            {
                throw new CreditPackageDoesNotExistsException();
            }

            _context.CreditPackages.Remove(packages);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}
