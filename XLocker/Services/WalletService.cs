using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using XLocker.Data;
using XLocker.DTOs.Wallet;
using XLocker.Entities;
using XLocker.Exceptions.User;
using XLocker.Response.Common;
using XLocker.Response.Wallet;
using XLocker.Types;

namespace XLocker.Services
{

    public interface IWalletService
    {
        Task<float> GetBalance(string userId);
        Task<Wallet> Pay(string userId, int credits, string? concept);
        Task<Wallet> VerificationReward(string userId);
        Task<ResponseList<Wallet>> GetTransactions(GetWalletDTO request, string userId);
        Task<Wallet> Award(CreateAwardDTO request);
        Task<List<WalletExcel>> GetBalanceReport(GetWalletDTO request, string userId);
    }

    public class WalletService : IWalletService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public WalletService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<float> GetBalance(string userId)
        {

            var deposits = await _context.Wallets.Where(x => x.UserId == userId && x.TransactionType == TransactionType.Deposit).SumAsync(x => x.Credits);
            var charges = await _context.Wallets.Where(x => x.UserId == userId && x.TransactionType == TransactionType.Charge).SumAsync(x => x.Credits);
            return deposits - charges;
        }

        public async Task<Wallet> Pay(string userId, int credits, string? concept)
        {
            var balance = await GetBalance(userId);
            if (balance < credits)
            {
                throw new NotEnoughBalanceException();
            }
            var walletTransaction = new Wallet
            {
                UserId = userId,
                Concept = concept,
                Credits = credits,
                TransactionType = TransactionType.Charge
            };
            await _context.Wallets.AddAsync(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task<Wallet> Award(CreateAwardDTO request)
        {
            var walletTransaction = new Wallet
            {
                UserId = request.UserId,
                Concept = request.Concept,
                Credits = request.Credits,
                TransactionType = TransactionType.Deposit
            };
            await _context.Wallets.AddAsync(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task<Wallet> VerificationReward(string userId)
        {
            var walletTransaction = new Wallet
            {
                UserId = userId,
                Concept = "Premio por verificacion exitosa",
                Credits = 6,
                TransactionType = TransactionType.Deposit
            };
            await _context.Wallets.AddAsync(walletTransaction);
            await _context.SaveChangesAsync();
            return walletTransaction;
        }

        public async Task<ResponseList<Wallet>> GetTransactions(GetWalletDTO request, string userId)
        {
            var query = _context.Wallets.Where(x => x.UserId == userId);
            if (request.Search != null)
            {
                query = query.Where(x => x.Concept.IsNullOrEmpty() || x.Concept.Contains(request.Search));
            }

            if (request.ShouldSearchByDate())
            {
                query = query.Where(x => x.UpdatedAt > request.FromDate && x.UpdatedAt < request.ToDate);
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            var totalCount = await query.CountAsync();
            var mappedTransactions = _mapper.Map<List<Wallet>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<Wallet> { TotalCount = totalCount, Data = mappedTransactions };
        }

        public async Task<List<WalletExcel>> GetBalanceReport(GetWalletDTO request, string userId)
        {
            var query = _context.Wallets.Where(x => x.UserId == userId);
            if (request.Search != null)
            {
                query = query.Where(x => x.Concept.IsNullOrEmpty() || x.Concept.Contains(request.Search));
            }

            if (request.ShouldSearchByDate())
            {
                query = query.Where(x => x.UpdatedAt > request.FromDate && x.UpdatedAt < request.ToDate);
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            return _mapper.Map<List<WalletExcel>>((await query.ToListAsync()).Select(x => { x.UserName = x.User.Name; return x; }));
        }
    }
}
