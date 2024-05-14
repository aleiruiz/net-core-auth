using XLocker.Data;
using XLocker.DTOs.Roles;
using XLocker.Entities;
using XLocker.Exceptions.Role;
using XLocker.Helpers;
using XLocker.Response.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace XLocker.Services
{

    public interface IRoleService
    {
        Task<ResponseList<Role>> GetAll();
        Task<ResponseList<Role>> GetManagmentRoles();
        Task<ResponseList<ABSRole>> Get(GetRoleDTO request);
        Task<Role> Create(CreateRoleDTO request);
        Task<bool> Update(UpdateRoleDTO request, string roleId);
        Task<bool> Delete(string roleId);
    }
    public class RoleService : IRoleService
    {
        private readonly DataContext _context;
        public readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        public RoleService(DataContext context, IServiceProvider serviceProvider, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        }

        public async Task<ResponseList<Role>> GetAll()
        {
            var roles = await _context.Roles.ToListAsync();
            return new ResponseList<Role> { TotalCount = roles.Count, Data = roles };
        }

        public async Task<ResponseList<Role>> GetManagmentRoles()
        {
            var roles = await _context.Roles.Where(x => x.Name != null && ManagmentHelper.ManagmentAllowedRoles.Contains(x.Name)).ToListAsync();
            return new ResponseList<Role> { TotalCount = roles.Count, Data = roles };
        }

        public async Task<ResponseList<ABSRole>> Get(GetRoleDTO request)
        {
            var query = _context.Roles.AsQueryable();
            if (request.Search != null)
            {
                query = query.Where(x => x.Name != null && x.Name.Contains(request.Search));
            }
            var totalCount = await query.CountAsync();

            var response = await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync();
            var mappedRoles = new List<ABSRole>();
            foreach (var role in response)
            {
                var newRole = _mapper.Map<ABSRole>(role);
                var aspRole = await _roleManager.FindByIdAsync(role.Id);
                if (aspRole != null)
                {
                    newRole.Permissions = (await _roleManager.GetClaimsAsync(role)).Select(x => x.Value).ToList();
                }
                mappedRoles.Add(newRole);
            }
            return new ResponseList<ABSRole> { TotalCount = totalCount, Data = mappedRoles };
        }

        public async Task<Role> Create(CreateRoleDTO request)
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
            {
                throw new RoleAlreadyExistException();
            }
            var role = new Role(request.Name);
            await _roleManager.CreateAsync(role);
            foreach (var permission in request.Permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
            return role;
        }

        public async Task<bool> Update(UpdateRoleDTO request, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                throw new RoleDoesNotExistException();
            }

            var alreadyExist = await _context.Roles.FirstOrDefaultAsync(u => u.Name == request.Name && u.Id != roleId);
            if (alreadyExist != null)
            {
                throw new RoleAlreadyExistException();
            }

            role.NormalizedName = request.Name.ToUpper();
            role.Name = request.Name;

            var response = await _roleManager.UpdateAsync(role);

            var claims = await _roleManager.GetClaimsAsync(role);

            var currentClaims = await _roleManager.GetClaimsAsync(role);

            var removedClaims = new List<Claim>();
            var addedClaims = request.Permissions;

            foreach (var claim in currentClaims)
            {
                if (request.Permissions.Any(s => s.Equals(claim.Value)))
                {
                    addedClaims = addedClaims.Where(val => val != claim.Value).ToArray();
                }
                else
                {
                    removedClaims.Add(claim);
                }
            }


            foreach (var claim in removedClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permission in addedClaims)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }


            return response.Succeeded;
        }

        public async Task<bool> Delete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                throw new RoleDoesNotExistException();
            }

            var response = await _roleManager.DeleteAsync(role);
            return response.Succeeded;
        }
    }
}
