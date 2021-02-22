using System;
using System.Collections.Generic;
using System.Text;

namespace Twitch_Integration
{
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
}
