using System;
using CodeHollow.FeedReader;

namespace ScubaMerchant
{
    partial class Program
    {
        public class FeedFromURL
        {
            public FeedFromURL(string url)
            {
                _url = url;
                lastFeed = new Feed();
            }

            public FeedItem LastItemAccessed { get; set; }

            public Feed Feed
            {
                get
                {
                    try
                    {
                        lastFeed = FeedReader.ReadAsync(_url).Result;
                    }
                    catch (Exception e)
                    {
                        // Console.WriteLine(e);
                    }

                    return lastFeed;
                }
            }

            private Feed lastFeed;
            private readonly string _url;
        }
    }
}
