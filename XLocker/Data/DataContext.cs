using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XLocker.Entities;

namespace XLocker.Data
{
    public class DataContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public override DbSet<User> Users { get; set; }

        public override DbSet<Role> Roles { get; set; }

        public DbSet<Locker> Lockers { get; set; }

        public DbSet<Mailbox> Mailboxes { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<ServiceTrack> ServiceTracks { get; set; }

        public DbSet<MaintanceOrder> MaintanceOders { get; set; }

        public DbSet<Diagnostic> Diagnostics { get; set; }

        public DbSet<Business> Businesses { get; set; }

        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        public DbSet<WithdrawalOrder> WithdrawalOrders { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<CreditPackage> CreditPackages { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<VerifiedPhones> VerifiedPhones { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(u => u.Roles).WithMany(r => r.Users).UsingEntity<IdentityUserRole<string>>();
            base.OnModelCreating(modelBuilder);

        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {

            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => (x.Entity is BaseEntity || x.Entity is User || x.Entity is Role)
                             && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                switch (entity.Entity)
                {
                    case User:
                        if (entity.State == EntityState.Added)
                        {
                            ((User)entity.Entity).CreatedAt = now;
                        }
                        ((User)entity.Entity).UpdatedAt = now;
                        break;
                    case Role:
                        if (entity.State == EntityState.Added)
                        {
                            ((Role)entity.Entity).CreatedAt = now;
                        }
                        ((Role)entity.Entity).UpdatedAt = now;
                        break;
                    default:
                        if (entity.State == EntityState.Added)
                        {
                            ((BaseEntity)entity.Entity).CreatedAt = now;
                        }
                        ((BaseEntity)entity.Entity).UpdatedAt = now;
                        break;
                }

            }
        }
    }
}
