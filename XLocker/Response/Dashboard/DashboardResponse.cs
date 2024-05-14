namespace XLocker.Response.Dashboard
{
    public class DashboardResponse
    {
        public int LockersAvailable { get; set; }
        public int MailboxesAvailable { get; set; }
        public int DeliveryUsers { get; set; }
        public int ServiceCompleted { get; set; }
        public int ServiceCreated { get; set; }
        public int ServiceAssigned { get; set; }
        public int ServiceOverdue { get; set; }
        public int ActiveService { get; set; }
    }
}
