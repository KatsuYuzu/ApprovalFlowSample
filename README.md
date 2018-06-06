# ApprovalFlowSample

Azure の DurableFunction のサンプルです。

## Usage

Create "local.settings.json" in your project folder.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "AzureWebJobsCosmosDBConnectionString": "AccountEndpoint={AccountEndpoint};AccountKey={AccountKey};",
    "TwitterConsumerKey": "{Twitter ConsumerKey}",
    "TwitterConsumerSecret": "{Twitter ConsumerSecret}",
    "TwitterAccessToken": "{Twitter AccessToken}",
    "TwitterAccessSecret": "{Twitter AccessSecret}"
  }
}
```
