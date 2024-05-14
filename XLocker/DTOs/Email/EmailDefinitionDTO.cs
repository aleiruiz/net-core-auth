namespace XLocker.DTOs.Email
{
    public class EmailDefinitionDTO
    {
        public required string Template { get; set; }

        public required string Subject { get; set; }
    }
}
