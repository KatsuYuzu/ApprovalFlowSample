using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApprovalFlowSample
{
    public static class TweetMonitoringFunction
    {
        // See: https://docs.microsoft.com/ja-jp/azure/azure-functions/durable-functions-monitor

        [FunctionName(nameof(StartTweetMonitoringAsync))]
        public static async Task<bool> StartTweetMonitoringAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            var input = context.GetInput<ApplicationEntity>();
            if (!context.IsReplaying) { log.LogInformation($"Received monitor request. Tweet: {input?.TweetId}."); }

            VerifyRequest(input);

            var endTime = context.CurrentUtcDateTime.AddMinutes(3);
            if (!context.IsReplaying) { log.LogInformation($"Instantiating monitor for {input.TweetId}. Expires: {endTime}."); }

            bool isFavorited = false;

            while (context.CurrentUtcDateTime < endTime)
            {
                if (!context.IsReplaying) { log.LogInformation($"Checking current tweet conditions for {input.TweetId} at {context.CurrentUtcDateTime}."); }

                // Check the tweet
                isFavorited = await context.CallActivityAsync<bool>(nameof(IsFavoritedAsync), input.TweetId);

                if (isFavorited)
                {
                    if (!context.IsReplaying) { log.LogInformation($"Favorited for {input.TweetId}."); }
                    break;
                }
                else
                {
                    // Wait for the next checkpoint
                    var nextCheckpoint = context.CurrentUtcDateTime.AddMinutes(1);
                    if (!context.IsReplaying) { log.LogInformation($"Next check for {input.TweetId} at {nextCheckpoint}."); }

                    await context.CreateTimer(nextCheckpoint, CancellationToken.None);
                }
            }

            log.LogInformation($"Monitor expiring. Tweet: {input?.TweetId}.");

            return isFavorited;
        }

        [FunctionName(nameof(IsFavoritedAsync))]
        public static async Task<bool> IsFavoritedAsync(
            [ActivityTrigger]DurableActivityContext context,
            ILogger log)
        {
            var tweetId = context.GetInput<long>();

            var twitter = new TwitterClient();

            var isFavorited = await twitter.IsFavoritedAsync(tweetId);

            log.LogInformation($"{tweetId} ÇÕÅuÇ¢Ç¢ÇÀÅv{(isFavorited ? "Ç≥ÇÍÇ‹ÇµÇΩ" : "Ç≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ")}");

            return isFavorited;
        }

        private static void VerifyRequest(ApplicationEntity request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "An input object is required.");
            }

            if (request.TweetId == null)
            {
                throw new ArgumentNullException(nameof(request.TweetId), "A tweetId input is required.");
            }
        }
    }
}
