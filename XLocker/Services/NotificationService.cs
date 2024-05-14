using XLocker.Data;
using XLocker.DTOs.Notification;
using XLocker.Entities;
using XLocker.Exceptions.Role;
using XLocker.Response.Common;
using Microsoft.EntityFrameworkCore;

namespace XLocker.Services
{

    public interface INotificationService
    {

        Task<ResponseList<Notification>> GetAll();
        Task<ResponseList<Notification>> GetActiveNotifications();
        Task<ResponseList<Notification>> Get(GetNotificationDTO request);
        Task<Notification> Create(CreateNotificationDTO request);
        Task<bool> Update(UpdateNotificationDTO request, string notificationId);
        Task<bool> Delete(string notificationId);
    }
    public class NotificationService : INotificationService
    {
        private readonly DataContext _context;

        public NotificationService(DataContext context)
        {
            _context = context;
        }

        public async Task<Notification> GetNotificationById(string notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(l => l.Id == notificationId);
            if (notification == null)
            {
                throw new NotificationDoesNotExistException();
            }
            return notification;
        }

        public async Task<ResponseList<Notification>> GetAll()
        {
            var diagnostics = await _context.Notifications.ToListAsync();
            return new ResponseList<Notification> { TotalCount = diagnostics.Count, Data = diagnostics };
        }

        public async Task<ResponseList<Notification>> GetActiveNotifications()
        {
            var dueDate = DateTime.UtcNow.AddDays(-1);
            var diagnostics = await _context.Notifications.OrderByDescending(x => x.UpdatedAt).ToListAsync();
            return new ResponseList<Notification> { TotalCount = diagnostics.Count, Data = diagnostics };
        }

        public async Task<ResponseList<Notification>> Get(GetNotificationDTO request)
        {
            var query = _context.Notifications.AsQueryable();
            if (request.Search != null)
            {
                query = query.Where(x => x.Title.Contains(request.Search));
            }
            var totalCount = await query.CountAsync();
            var response = await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync();

            return new ResponseList<Notification> { TotalCount = totalCount, Data = response };
        }

        public async Task<Notification> Create(CreateNotificationDTO request)
        {
            var notification = new Notification
            {
                Description = request.Description,
                Title = request.Title,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> Update(UpdateNotificationDTO request, string notificationId)
        {
            var notification = await GetNotificationById(notificationId);

            notification.Description = request.Description;
            notification.Title = request.Title;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Delete(string notificationId)
        {
            var notification = await GetNotificationById(notificationId);

            _context.Notifications.Remove(notification);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}
