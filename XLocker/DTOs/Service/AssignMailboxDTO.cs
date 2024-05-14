using XLocker.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XLocker.DTOs.Service
{
    public class AssignMailboxDTO
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public required MailboxSize Size { get; set; }
    }
}
