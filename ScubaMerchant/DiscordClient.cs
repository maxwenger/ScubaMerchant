using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Discord;
using Discord.WebSocket;

namespace ScubaMerchant
{
    partial class Program
    {
        internal class DiscordClient
        {
            private DiscordSocketClient _client;
            private readonly ulong _merchantChannelId;
            private readonly string _botToken;

            public DiscordClient(string botToken, ulong channelId)
            {
                _botToken = botToken;
                _merchantChannelId = channelId;

                var running = false;

                while (!running)
                {
                    try
                    {
                        InitBot().GetAwaiter().GetResult();
                        running = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Startup Failed:" + e.Message + "\n Trying again...");
                    }
                }

            }

            private async Task InitBot()
            {
                _client = new DiscordSocketClient();

                _client.Log += Log;
                _client.MessageReceived += MessageReceived;

                await _client.LoginAsync(TokenType.Bot, _botToken);
                await _client.StartAsync();
            }

            private Task Log(LogMessage msg)
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            }

            private async Task MessageReceived(SocketMessage message)
            {
                if (message.Content == ";ping")
                {
                    await message.Channel.SendMessageAsync("Pong!");
                }
            }

            public async Task SendFeedItem(FeedItem item)
            {
                var timestamp = item.PublishingDate ?? DateTimeOffset.Now;

                var embed = new EmbedBuilder()
                    .WithColor(Color.DarkRed)
                    .WithAuthor(GetNameFromAuthor(item.Author))
                    .WithTitle(item.Title)
                    .WithUrl(item.Link)
                    .WithDescription(FormatForDisplay(StripHTML(item.Content)))
                    .WithTimestamp(timestamp)
                    .Build();

                await ((ISocketMessageChannel)_client.GetChannel(_merchantChannelId)).SendMessageAsync(embed: embed);
            }

            private static string StripHTML(string input) => 
                Regex.Replace(input, "<.*?>", string.Empty);

            private static string FormatForDisplay(string input)
            {
                input = Regex.Replace(input, @"View attachment \d*", string.Empty);
                input = Regex.Replace(input, @"\r\n?|\n", " ");

                return input;
            }

            private static string GetNameFromAuthor(string author) => 
                Regex.Match(author, @"\(([^)]*)\)").Groups[1].ToString();
        }
    }
}
