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
        // Azure �� DurableFunction �̃T���v���ł��B
        //
        // ���F�t���[���g���K�[���ꂽ��A�\�����c�C�[�g���܂��B
        // ���̌�A�c�C�[�g���Ď����āA�u�����ˁv���ꂽ���ǂ������擾���܂��B
        // �u�����ˁv���ꂽ�ꍇ�A���F�ς݂Ƃ��ă��R�[�h���X�V���܂��B
        //
        // ���L�̐ݒ�� "local.settings.json" �ɐݒ肵�Ă��������B
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
