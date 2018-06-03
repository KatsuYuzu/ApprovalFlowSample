using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApprovalFlowSample
{
    public static class Starter
    {
        // Azure の DurableFunction のサンプルです。
        //
        // 承認フローがトリガーされたら、申請をツイートします。
        // その後、ツイートを監視して、「いいね」されたかどうかを取得します。
        // 「いいね」された場合、承認済みとしてレコードを更新します。
        //
        // 下記の設定を "local.settings.json" に設定してください。
        //
        // "Values": {
        //   "AzureWebJobsCosmosDBConnectionString": "AccountEndpoint={AccountEndpoint};AccountKey={AccountKey};",
        //   "ConsumerKey": "{ConsumerKey}",
        //   "ConsumerSecret": "{ConsumerSecret}",
        //   "AccessToken": "{AccessToken}",
        //   "AccessSecret": "{AccessSecret}"
        // }

        [FunctionName(nameof(RunTriggerApprovalFlow))]
        public static async Task<HttpResponseMessage> RunTriggerApprovalFlow(
            [HttpTrigger(AuthorizationLevel.Function, methods: "post")] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClientBase starter,
            ILogger log)
        {
            var input = await req.Content.ReadAsAsync<ApplicationEntity>();

            VerifyRequest(input);

            string instanceId = await starter.StartNewAsync(nameof(ApprovalFlowFcuntion.StartApprovalFlowAsync), input.Content);

            var res = starter.CreateCheckStatusResponse(req, instanceId);
            res.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(10));
            return res;
        }

        private static void VerifyRequest(ApplicationEntity request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "An input object is required.");
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                throw new ArgumentNullException(nameof(request.Content), "A content input is required.");
            }
        }
    }
}
