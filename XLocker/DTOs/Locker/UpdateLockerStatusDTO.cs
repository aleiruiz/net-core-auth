using XLocker.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XLocker.DTOs.Locker
{
    public class UpdateLockerStatusDTO
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LockerStatus Status { get; set; }
    }
}
