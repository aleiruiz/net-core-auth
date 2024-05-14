using XLocker.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace XLocker.DTOs.Locker
{
    public class CreateLockerDTO
    {
        public required string Name { get; set; }

        public string Reference { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int MaxMailboxes { get; set; } = 0;

        public ConnectionState Connection { get; set; } = ConnectionState.Online;

        [JsonConverter(typeof(StringEnumConverter))]
        public LockerStatus? Status { get; set; }
    }
}
