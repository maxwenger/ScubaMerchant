using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ScubaMerchant
{
    partial class Program
    {
        private static DiscordClient client;

        private static string[] RSSFeeds =
        {
            "https://www.scubaboard.com/community/forums/classifieds-bcds-and-weight-systems.747/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-computers-gauges-watches-analyzers.746/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-exposure-suits.745/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-lights.750/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-other-gear-and-multiple-items.748/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-photography.483/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-regulators.744/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-tanks-valves-and-bands.749/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-videography.447/index.rss",
            "https://www.scubaboard.com/community/forums/classifieds-miscellaneous-items.288/index.rss"
        };

        private static List<FeedFromURL> Feeds;

        static void Main(string[] args)
        {
            URLsToFeedList();

            client = new DiscordClient(Settings.BotToken, Settings.ChannelId);

            new SendNewFeedItemsTimed(client).StartAsync(new CancellationToken());

            Console.Read();
        }

        private static void URLsToFeedList()
        {
            Feeds = new List<FeedFromURL>();
            foreach (var url in RSSFeeds)
            {
                Feeds.Add(new FeedFromURL(url));
            }
        }

        private static class Settings
        {
            private static IDictionary environmentVariables;

            static Settings()
            {
                environmentVariables = Environment.GetEnvironmentVariables();
            }

            public static string BotToken => (string)environmentVariables["BotToken"];
            public static ulong ChannelId
            {
                get {
                    ulong.TryParse((string)environmentVariables["ChannelID"], out var value);
                    return value; 
                }
            }
        }
    }
}
