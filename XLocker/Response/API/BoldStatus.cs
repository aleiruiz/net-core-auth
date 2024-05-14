using Newtonsoft.Json;

namespace XLocker.Response.API
{
    public class BoldStatus
    {
        public string? link_id { get; set; }
        public string? transaction_id { get; set; }
        public int? total { get; set; }
        public int? subtotal { get; set; }
        public string? description { get; set; }
        public string? reference_id { get; set; }
        public string? payment_method { get; set; }
        public string? payer_email { get; set; }
        public string? transaction_date { get; set; }
        public string? payment_status { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<BoldError>? errors { get; set; }
    }

    public class BoldError
    {
        [JsonProperty(PropertyName = "message")]
        public string? message { get; set; }
    }
}
