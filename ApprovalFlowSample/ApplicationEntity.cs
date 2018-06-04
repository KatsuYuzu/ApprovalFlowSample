using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace ApprovalFlowSample
{
    public enum ApplicationStatus
    {
        None = 0,
        Applying = 1,
        Approved = 2,
        Rejected = 3,
        Failed = -1
    }

    /// <summary>
    /// 申請。
    /// </summary>
    public class ApplicationEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("tweetId")]
        public long? TweetId { get; set; }

        [JsonProperty("status")]
        public ApplicationStatus Status { get; set; }
    }
}
