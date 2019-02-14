using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Microsoft.Extensions.Hosting;

namespace ScubaMerchant
{
    partial class Program
    {
        internal class SendNewFeedItemsTimed : IHostedService, IDisposable
        {

            private Timer _timer;
            private DiscordClient client;

            public SendNewFeedItemsTimed(DiscordClient client)
            {
                this.client = client;
            }

            private async void SendFeedItems(object state)
            {
                foreach (var feed in Feeds)
                {
                    var feedList = feed.Feed.Items;
                    var latestItem = feedList?.First();

                    if (feedList == null || latestItem == null) continue;
                    if (feed.LastItemAccessed == null) feed.LastItemAccessed = latestItem;
                    
                    if (!latestItem.Link.Equals(feed.LastItemAccessed.Link))
                    {
                        var items = new Queue<FeedItem>(feedList);
                        
                        while (items.Count > 0 && !items.Peek().Link.Equals(feed.LastItemAccessed.Link))
                        {
                            await client.SendFeedItem(items.Dequeue());
                        }

                        feed.LastItemAccessed = latestItem;
                    }
                }
            }
            
            public Task StartAsync(CancellationToken cancellationToken)
            {
                _timer = new Timer(SendFeedItems, null, TimeSpan.Zero,TimeSpan.FromSeconds(5));
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _timer?.Change(Timeout.Infinite, 0);

                return Task.CompletedTask;
            }

            public void Dispose()
            {
                _timer?.Dispose();
            }
        }
    }
}
