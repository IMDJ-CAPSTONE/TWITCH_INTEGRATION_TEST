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


    class Poll
    {
        public string topic;
        public string option1;
        public string option2;
        public string option3;
        public string option4;
        private int votes1;
        private int votes2;
        private int votes3;
        private int votes4;
        public TimeSpan time;
        public bool active;


        //!newpoll what should be the next song? All Star, 22, Smells Like Teen Spirit, 
        public Poll(string input)
        {
            string[] question = input.Split('?');
            string[] option = question[1].Split(',');

            topic = question[0].Substring(9) + "?";

            if (option[0] != null)
            {
                option1 = option[0];
            }

            if (option[1] != null)
            {
                option2 = option[1];
            }

            if (option[2] != null)
            {
                option3 = option[2];
            }

            if (option[3] != null)
            {
                option4 = option[3];
            }

            active = true;
        }

        private void vote1()
        {
            votes1++;
        }

        private void vote2()
        {
            votes2++;
        }

        private void vote3()
        {
            votes3++;
        }

        private void vote4()
        {
            votes4++;
        }

        public void vote(string input)
        {
            //!vote 1
            int thevote;
            string castvote = input.Substring(5);
            try
            {
                thevote = int.Parse(castvote);

                if (thevote == 1)
                {
                    vote1();
                }
                else if (thevote == 2)
                {
                    vote2();
                }
                else if (thevote == 3)
                {
                    vote3();
                }
                else if (thevote == 4)
                {
                    vote4();
                }
                else
                {
                    //error invalid vote
                }
            }
            catch (Exception e)
            {

            }

        }

        public string display()
        {
            string display = topic + 
                            "   1." + option1 + " votes=" + votes1 + 
                            ",  2." + option2 + " votes=" + votes2 + 
                            ",  3." + option3 + " votes=" + votes3 +
                            ",  4." + option4 + " votes=" + votes4;

            return display;
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
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            }                
            else
            {
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
            }
        }
    }
}
