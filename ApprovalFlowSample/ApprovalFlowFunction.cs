using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApprovalFlowSample
{
    public static class ApprovalFlowFunction
    {
        [FunctionName(nameof(TriggerApprovalFlow))]
        public static void TriggerApprovalFlow(
            [CosmosDBTrigger("ApprovalFlowSample", "Applications", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [OrchestrationClient] DurableOrchestrationClientBase starter)
        {
            foreach (var document in documents.Where(x => x.GetPropertyValue<ApplicationStatus>("status") == ApplicationStatus.None))
            {
                starter.StartNewAsync(nameof(ApprovalFlowFunction.StartApprovalFlowAsync), document);
            }
        }

        [FunctionName(nameof(StartApprovalFlowAsync))]
        public static async Task StartApprovalFlowAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var application = context.GetInput<ApplicationEntity>();

            try
            {
                var tweetId = await context.CallActivityAsync<long>(nameof(RequestApprovalAsync), application);

                var isFavorited = await context.CallSubOrchestratorAsync<bool>(nameof(TweetMonitoringFunction.StartTweetMonitoringAsync), tweetId);

                if (isFavorited)
                {
                    await context.CallActivityAsync(nameof(MarkApprovedAsync), application);
                }
                else
                {
                    await context.CallActivityAsync(nameof(MarkRejectedAsync), application);
                }
            }
            catch (Exception)
            {
                await context.CallActivityAsync(nameof(MarkFailedAsync), application);
            }
        }

        [FunctionName(nameof(RequestApprovalAsync))]
        public static async Task<long> RequestApprovalAsync(
            [ActivityTrigger]DurableActivityContext context,
            Binder binder,
            ILogger log)
        {
            var application = context.GetInput<ApplicationEntity>();

            if (application.Content == null)
            {
                throw new ArgumentNullException(nameof(application.Content), "A content input is required.");
            }

            var twitter = new TwitterClient();

            var tweetId = await twitter.TweetAsync(application.Content);

            log.LogInformation($"{tweetId} をツイートしました。: {application.Content}");

            application.TweetId = tweetId;
            application.Status = ApplicationStatus.Applying;

            var collector = await binder.BindAsync<IAsyncCollector<ApplicationEntity>>(new CosmosDBAttribute("ApprovalFlowSample", "Applications"));
            await collector.AddAsync(application);

            return tweetId;
        }

        [FunctionName(nameof(MarkApprovedAsync))]
        public static async Task MarkApprovedAsync(
            [ActivityTrigger]DurableActivityContext context,
            Binder binder)
        {
            var application = context.GetInput<ApplicationEntity>();

            application.Status = ApplicationStatus.Approved;

            var collector = await binder.BindAsync<IAsyncCollector<ApplicationEntity>>(new CosmosDBAttribute("ApprovalFlowSample", "Applications"));
            await collector.AddAsync(application);
        }

        [FunctionName(nameof(MarkRejectedAsync))]
        public static async Task MarkRejectedAsync(
            [ActivityTrigger]DurableActivityContext context,
            Binder binder)
        {
            var application = context.GetInput<ApplicationEntity>();

            application.Status = ApplicationStatus.Rejected;

            var collector = await binder.BindAsync<IAsyncCollector<ApplicationEntity>>(new CosmosDBAttribute("ApprovalFlowSample", "Applications"));
            await collector.AddAsync(application);
        }

        [FunctionName(nameof(MarkFailedAsync))]
        public static async Task MarkFailedAsync(
            [ActivityTrigger]DurableActivityContext context,
            Binder binder)
        {
            var application = context.GetInput<ApplicationEntity>();

            application.Status = ApplicationStatus.Failed;

            var collector = await binder.BindAsync<IAsyncCollector<ApplicationEntity>>(new CosmosDBAttribute("ApprovalFlowSample", "Applications"));
            await collector.AddAsync(application);
        }
    }
}
