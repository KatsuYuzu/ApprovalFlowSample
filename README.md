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
    "ConsumerKey": "{Twitter ConsumerKey}",
    "ConsumerSecret": "{Twitter ConsumerSecret}",
    "AccessToken": "{Twitter AccessToken}",
    "AccessSecret": "{Twitter AccessSecret}"
  }
}
```
