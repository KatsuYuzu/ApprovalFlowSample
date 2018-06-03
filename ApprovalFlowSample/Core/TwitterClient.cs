using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApprovalFlowSample
{
    public class TwitterClient
    {
        private CoreTweet.Tokens CreateTokens()
        {
            return CoreTweet.Tokens.Create(
                Settings.Instance.ConsumerKey,
                Settings.Instance.ConsumerSecret,
                Settings.Instance.AccessToken,
                Settings.Instance.AccessSecret);
        }

        /// <summary>
        /// ツイートします。戻り値はツイートの Id です。
        /// </summary>
        /// <param name="text">ツイートする文字列。</param>
        /// <returns>ツイートの Id。</returns>
        public async Task<long> TweetAsync(string text)
        {
            var tokens = CreateTokens();

            var tweet = await tokens.Statuses.UpdateAsync(new Dictionary<string, object>()
            {
                { "status", text }
            });

            return tweet.Id;
        }

        /// <summary>
        /// 「いいね」されたかどうかを取得します。
        /// </summary>
        /// <param name="id">ツイートの Id。</param>
        /// <returns>「いいね」されたかどうかの値。</returns>
        public async Task<bool> IsFavoritedAsync(long id)
        {
            var tokens = CreateTokens();

            var tweet = await tokens.Statuses.ShowAsync(new Dictionary<string, object>()
            {
                { "id", id }
            });

            return tweet.IsFavorited ?? false;
        }
    }
}
