/*
* FILE :
* PROJECT : 
* PROGRAMMER : Jason Kassies & Div Dankara
* FIRST VERSION : 
* DESCRIPTION :
* The functions in this file are used to …
*/

using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Twitch_Integration
{

    class Program {

        static void Main(string[] args)
        {
            Bot bot = new Bot();

            //start the bot on its own thread so that the console can react to input
            //Thread thread1 = new Thread(bot.start);

            string input = "";

            while(input != "!exit")
            {
                input = Console.ReadLine();

            }
            
        }

    }

    class Bot
    {
        TwitchClient client;
        string channelName = "Jdog0616";
        string authToken = "oub0s2grmw1u7x9mtql06kwldmvi69";
        Poll poll;

        public Bot()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(channelName, authToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, channelName);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnBeingHosted += Client_OnBeingHosted;
            client.OnGiftedSubscription += Clinet_OnGiftedSub;
            client.OnModeratorJoined += Client_OnModJoin;
            client.OnReSubscriber += Client_OnReSub;
            client.OnUserBanned += Client_UserBanned;


            client.Connect();
        }



        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains("fuck"))
            {
                //client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromSeconds(5), "Bad word! 5 second timeout!");
                client.SendMessage(e.ChatMessage.Channel, "hey now...");
            }

            if (e.ChatMessage.Message.StartsWith('!'))
            {
                //COMMAND DETECTED
                if(e.ChatMessage.Message.StartsWith("!newpoll "))
                {
                    if(e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                    {
                        //!newpoll what should be the next song? All Star, 22, Smells Like Teen Spirit
                        
                        poll = new Poll(e.ChatMessage.Message);
                        
                    }
                    else
                    {
                        client.SendMessage(e.ChatMessage.Channel, "only mods can use this command");
                    }
                }


                if(e.ChatMessage.Message.StartsWith("!vote"))
                {
                    if (poll.active)
                    {
                        poll.vote(e.ChatMessage.Message);
                        client.SendMessage(e.ChatMessage.Channel, poll.display());
                    }
                    else
                    {
                        //no poll to vote so it gets ignored
                    }
                }
            }
            
            
            Console.WriteLine("{0} says : {1}", e.ChatMessage.Username, e.ChatMessage.Message);
            
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            {
                client.SendMessage(e.Channel, $"{e.Subscriber.DisplayName} joined to the rave thanks to Prime!");
                client.SendMessage(e.Channel, "Hey there! Do you want to know about Twitch Prime? Oh! You may be asking, " +
                                              "What's Twitch Prime? Let me tell ya! When you connect your Amazon account to your " +
                                              "Twitch account, you can get 1 free sub to ANY streamer on Twitch, every month! " +
                                              "Yup, and along with that, get yourself some Twitch loot! With Twitch loot, you can " +
                                              "go ahead and get yourself some exclusive Twitch gear and your favorite games! And until April 30th, " +
                                              "you can get yourself some Fortnite skins, with Twitch loot! So go ahead! Grab your Amazon account," +
                                              " grab a family or friend's Amazon Prime account, and link it to your Twitch account TODAY!");
            }                
            else
            {
                client.SendMessage(e.Channel, $"{e.Subscriber.DisplayName} joined to the rave!");
            }
        }

        private void Client_OnReSub(object sender, OnReSubscriberArgs e)
        {
            if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            {
                client.SendMessage(e.Channel, $"{e.ReSubscriber.DisplayName} has been raving for {e.ReSubscriber.Months} months thanks to Prime!!");
            }
            else
            {
                client.SendMessage(e.Channel, $"{e.ReSubscriber.DisplayName} has been raving for {e.ReSubscriber.Months} months!!");
            }
        }

        private void Clinet_OnGiftedSub(object sender, OnGiftedSubscriptionArgs e)
        {
            client.SendMessage(e.Channel, $"{e.GiftedSubscription.DisplayName} is giving out free bumps!!");
        }

        private void Client_OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            client.SendMessage(e.BeingHostedNotification.Channel, $"Shoutout to {e.BeingHostedNotification.HostedByChannel} for the Host!!");
        }

        private void Client_OnModJoin(object sender, OnModeratorJoinedArgs e)
        {
            client.SendMessage(e.Channel, $"Everybody watch out, we got a snitch over here --> {e.Username}");
        }

        private void Client_UserBanned(object sender, OnUserBannedArgs e)
        {
            client.SendMessage(e.UserBan.Channel, $"THE BAN HAMMER HAS STRUCK! rip {e.UserBan.Username}, maybe dont get caught doing {e.UserBan.BanReason} next time");
        }


    }
}
