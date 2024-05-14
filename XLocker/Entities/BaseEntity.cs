namespace XLocker.Entities
{
    public abstract class BaseEntity
    {
        public virtual string Id { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

}
