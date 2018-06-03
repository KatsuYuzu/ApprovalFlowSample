using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ApprovalFlowSample
{
    public static class ApprovalFlowFcuntion
    {
        /// <summary>
        /// 承認フローを開始します。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [FunctionName(nameof(StartApprovalFlowAsync))]
        public static async Task StartApprovalFlowAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var input = context.GetInput<string>();

            var application = await context.CallActivityAsync<ApplicationEntity>(nameof(RequestApprovalAsync), input);

            application.IsApproved = await context.CallSubOrchestratorAsync<bool>(nameof(TweetMonitoringFcuntion.StartTweetMonitoringAsync), application);

            await context.CallActivityAsync(nameof(EndApprovalFlowAsync), application);
        }

        /// <summary>
        /// 承認を要求します。
        /// </summary>
        /// <param name="context"></param>
        [FunctionName(nameof(RequestApprovalAsync))]
        public static async Task<ApplicationEntity> RequestApprovalAsync(
            [ActivityTrigger]DurableActivityContext context,
            Binder binder,
            ILogger log)
        {
            var input = context.GetInput<string>();

            var twitter = new TwitterClient();

            var tweetId = await twitter.TweetAsync(input);

            log.LogInformation($"「{input}」をツイートしました。 Tweet: {tweetId}");

            var collector = await binder.BindAsync<IAsyncCollector<ApplicationEntity>>(
                new CosmosDBAttribute("ApprovalFlowSample", "Applications") { CreateIfNotExists = true });

            var application = new ApplicationEntity
            {
                InstanceId = context.InstanceId,
                TweetId = tweetId,
                Content = input
            };

            await collector.AddAsync(application);

            return application;
        }

        /// <summary>
        /// 承認フローを終了します。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [FunctionName(nameof(EndApprovalFlowAsync))]
        public static void EndApprovalFlowAsync(
            [ActivityTrigger]DurableActivityContext context,
            [CosmosDB("ApprovalFlowSample", "Applications", Id = "{InstanceId}")]ApplicationEntity application)
        {
            var input = context.GetInput<ApplicationEntity>();

            application.IsApproved = input.IsApproved;
        }
    }
}
