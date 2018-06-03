using Newtonsoft.Json;

namespace ApprovalFlowSample
{
    /// <summary>
    /// 申請。
    /// </summary>
    public class ApplicationEntity
    {
        [JsonProperty("id")]
        public string Id { get { return InstanceId; } }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("tweetId")]
        public long? TweetId { get; set; }

        [JsonProperty("isApproved")]
        public bool? IsApproved { get; set; }
    }
}
