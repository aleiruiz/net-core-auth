using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XLocker.API;
using XLocker.Data;
using XLocker.DTOs.Auth;
using XLocker.DTOs.Users;
using XLocker.Entities;
using XLocker.Exceptions.User;
using XLocker.Response.Common;
using XLocker.Response.User;

namespace XLocker.Services
{
    public interface IUserService
    {
        Task<ResponseList<RestrictedUser>> GetRestrictedUserList(GetRestrictedUserDTO request);
        Task<ResponseList<ABSUser>> GetAll();
        Task<ResponseList<ABSUser>> Get(GetUsersDTO request, string? roles = null);
        Task<int> GetDeliveryUsers();
        Task<bool> CheckIfAdminByEmail(string Email);
        Task<ABSUser> Create(AddUsersDTO request);
        Task<bool> Update(UpdateUsersDTO request, string userId);
        Task<bool> UpdatePassword(UpdatePasswordDTO user, string userId);
        Task<bool> Delete(string userId);
        Task<bool> SendPhoneVerificationCode(SendPhoneVerificationDTO request, string userId);
        Task<Response<string>> VerifyPhone(VerifyPhoneDTO request, string userId);
    }
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public readonly IPasswordHasher<User> _passwordHasher;
        public readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IWalletService _walletService;
        private readonly ISMSProvider _smsProvider;

        public UserService(DataContext context, IPasswordHasher<User> passwordHasher, IServiceProvider serviceProvider, IMapper mapper, IWalletService walletService, ISMSProvider smsProvider)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            _mapper = mapper;
            _walletService = walletService;
            _smsProvider = smsProvider;
        }
        public async Task<ResponseList<RestrictedUser>> GetRestrictedUserList(GetRestrictedUserDTO request)
        {
            var query = _context.Users.Include(x => x.Roles).Where(x => x.Status != Types.UserStatus.Inactive).AsQueryable();

            if (request.Roles != null)
            {
                var roles = request.Roles.Split(",");
                query = query.Where(x => x.Roles.Select(x => x.Name != null ? x.Name.ToLower() : "").Where(x => roles.Contains(x)).Count() > 0);
            }
            var response = await query.ToListAsync();
            var mappedUsers = _mapper.Map<List<RestrictedUser>>(response);
            return new ResponseList<RestrictedUser> { TotalCount = mappedUsers.Count, Data = mappedUsers };
        }

        public async Task<ResponseList<ABSUser>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            var mappedUsers = _mapper.Map<List<ABSUser>>(users);
            return new ResponseList<ABSUser> { TotalCount = users.Count, Data = mappedUsers };
        }

        public async Task<ResponseList<ABSUser>> Get(GetUsersDTO request, string? requestedRoles)
        {
            var query = _context.Users.Include(x => x.Roles).AsQueryable();
            if (request.Search != null)
            {
                query = query.Where(x => x.Name.Contains(request.Search));
            }
            if (requestedRoles != null)
            {
                var roles = requestedRoles.Split(",");
                query = query.Where(x => x.Roles.Select(x => x.Name != null ? x.Name.ToLower() : "").Where(x => roles.Contains(x)).Count() > 0);
            }
            var totalCount = await query.CountAsync();

            var response = await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync();

            var mappedUsers = _mapper.Map<List<ABSUser>>(response);
            return new ResponseList<ABSUser> { TotalCount = totalCount, Data = mappedUsers };
        }

        public async Task<int> GetDeliveryUsers()
        {
            return await _context.Users.Where(x => x.Roles.Where(x => x.Name == "delivery").Count() > 0).CountAsync();
        }

        public async Task<bool> CheckIfAdminByEmail(string Email)
        {
            var user = await _context.Users.FirstAsync((x) => x.Email == Email);
            if (user == null)
            {
                throw new UserDoesNotExistException();
            }

            return user.IsAdmin;
        }

        public async Task<ABSUser> Create(AddUsersDTO request)
        {
            var alreadyExist = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (alreadyExist != null)
            {
                throw new NotEnoughBalanceException();
            }
            var guid = Guid.NewGuid();
            var identityUser = new User
            {
                Email = request.Email,
                Name = request.Name,
                Id = guid.ToString(),
                Status = request.Status,
                UserName = request.Email,
                NormalizedUserName = request.Email.ToUpper(),
                NormalizedEmail = request.Email.ToUpper(),
                LockoutEnabled = true,
                IsAdmin = true,
                EmailConfirmed = true,
            };

            await _context.Users.AddAsync(identityUser);

            var hashedPassword = _passwordHasher.HashPassword(identityUser, request.Password);
            identityUser.SecurityStamp = Guid.NewGuid().ToString();
            identityUser.PasswordHash = hashedPassword;


            await _context.SaveChangesAsync();

            await _userManager.AddToRolesAsync(identityUser, request.roles);

            return _mapper.Map<ABSUser>(identityUser);
        }

        public async Task<bool> Update(UpdateUsersDTO request, string userId)
        {
            var identityUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (identityUser == null)
            {
                throw new UserDoesNotExistException();
            }
            var alreadyExist = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != userId);
            if (alreadyExist != null)
            {
                throw new NotEnoughBalanceException();
            }
            identityUser.UserName = request.Email;
            identityUser.Email = request.Email;
            identityUser.Name = request.Name;
            identityUser.Status = request.Status;
            identityUser.NormalizedUserName = request.Email.ToUpper();
            identityUser.NormalizedEmail = request.Email.ToUpper();


            var response = await _context.SaveChangesAsync();
            var currentRoles = await _userManager.GetRolesAsync(identityUser);
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
            }

            await _userManager.AddToRolesAsync(identityUser, request.roles);
            return response > 0;
        }

        public async Task<bool> UpdatePassword(UpdatePasswordDTO request, string userId)
        {
            var identityUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (identityUser == null)
            {
                throw new UserDoesNotExistException();
            }
            var hashedPassword = _passwordHasher.HashPassword(identityUser, request.Password);
            identityUser.SecurityStamp = Guid.NewGuid().ToString();
            identityUser.PasswordHash = hashedPassword;
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }

        public async Task<bool> Delete(string userId)
        {
            var identityUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (identityUser == null)
            {
                throw new UserDoesNotExistException();
            }
            _context.Users.Remove(identityUser);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }

        public async Task<bool> SendPhoneVerificationCode(SendPhoneVerificationDTO request, string userId)
        {
            var identityUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (identityUser == null)
            {
                throw new UserDoesNotExistException();
            }
            if (identityUser.PhoneNumber == null && request.PhoneNumber == null)
            {
                throw new PhoneNotFoundException();
            }

            Random r = new Random();

            int rInt = r.Next(100000, 999999);

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                identityUser.PhoneNumber = request.PhoneNumber;
            }
            identityUser.PhoneVerificationCode = rInt.ToString();

            var response = await _context.SaveChangesAsync();

            _ = _smsProvider.SendVerificationCode(identityUser.PhoneNumber, identityUser.PhoneVerificationCode);

            return response > 0;

        }

        public async Task<Response<string>> VerifyPhone(VerifyPhoneDTO request, string userId)
        {
            var identityUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (identityUser == null)
            {
                throw new UserDoesNotExistException();
            }

            if (request.PhoneVerificationCode != identityUser.PhoneVerificationCode)
            {
                throw new VerificationCodeMismatchException();
            }

            identityUser.PhoneNumberConfirmed = true;

            var message = "";

            if (!identityUser.RewardClaimed)
            {
                var isPhoneAlreadyVerified = await _context.VerifiedPhones.FirstOrDefaultAsync(x => x.PhoneNumber == identityUser.PhoneNumber);
                if (isPhoneAlreadyVerified == null)
                {
                    message = "El telefono se ha verificado correctamente y se ha proporcionado un bono de bienvenida!";
                    var verifiedPhone = new VerifiedPhones { PhoneNumber = identityUser.PhoneNumber ?? "" };
                    await _context.VerifiedPhones.AddAsync(verifiedPhone);
                    await _walletService.VerificationReward(userId);
                    identityUser.RewardClaimed = true;
                }
                else
                {
                    message = "El telefono se ha verificado correctamente pero la recomensa para este numero ya ha sido redimida.";
                }
            }
            else
            {
                message = "El telefono se ha verificado correctamente.";
            }


            var response = await _context.SaveChangesAsync();

            return new Response<string> { Data = message, Status = 200 };

        }
    }
}
