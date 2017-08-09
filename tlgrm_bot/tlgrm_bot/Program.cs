using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;               //Use internet
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace tlgrm_bot
{
    class MyTelBot
    {
        WebClient webClient = new WebClient();
        int update_id = 0;
        string messageFromId = "";
        string messageText = "";
        string startUrl = $"https://api.telegram.org/bot349767489:AAFH2i94VcV9bl3RgTILJugxv6mYNTSEip8";

        public void Init()
        {
            int update_id = 0;
            while (true)
            {
                string url = $"{startUrl}/getUpdates?offset={update_id + 1}";
                string response = webClient.DownloadString(url);
                if (response != $"{{\"ok\":true,\"result\":[]}}")
                {
                    var col = JObject.Parse(response)["result"].ToArray();
                    Console.WriteLine(response);
                    foreach (var msg in col)
                    {
                        update_id = Convert.ToInt32(msg["update_id"]);
                        try
                        {
                            messageFromId = msg["message"]["from"]["id"].ToString();
                            messageText = msg["message"]["text"].ToString();
                        }
                        catch { }
                    }
                }
                else
                {
                    Console.WriteLine("Init complete");
                    break;
                }
            }
        }

        public void SetParam(string token)
        {
            startUrl = $"https://api.telegram.org/bot{token}";
        }

        public void GetUpdate()
        {
            string url = $"{startUrl}/getUpdates?offset={update_id + 1}";
            string response = webClient.DownloadString(url);
            if (response != $"{{\"ok\":true,\"result\":[]}}")
            {
                var col = JObject.Parse(response)["result"].ToArray();
                foreach (var msg in col)
                {
                    update_id = Convert.ToInt32(msg["update_id"]);
                    try
                    {
                        if (msg["message"]["chat"]["type"].ToString() == "private")
                        {
                            messageFromId = msg["message"]["from"]["id"].ToString();
                            messageText = msg["message"]["text"].ToString();
                            SendAnsw(messageText);
                        }
                        if (msg["message"]["chat"]["type"].ToString() == "group")
                        {
                            messageFromId = msg["message"]["chat"]["id"].ToString();
                            messageText = msg["message"]["text"].ToString();
                            SendAnsw(messageText);
                        }
                        Console.WriteLine(msg["message"]["text"].ToString());
                    }
                    catch { }
                }
            }

        }

        public void SendMsg(string messageFromId, string messageText)
        {
            string url = $"{startUrl}/sendMessage?chat_id={messageFromId}&text={messageText}";
            webClient.DownloadString(url);
        }

        public string Udaff()
        {
            //WebClient webClient = new WebClient();
            string g = webClient.DownloadString("http://udaff.com/view_listen/photo/random.html");
            //string g = "<img src='/image/111/11109.jpg' width='400' height='300' alt='2' border='0' />";
            Regex regex = new Regex(@"\/image\/\d*.*.jpg");
            Match match = regex.Match(g);
            return match.ToString();
        }

        public void SendAnsw(string answ)
        {
            if (answ == "test") { SendMsg(messageFromId, "passed"); }
            if (answ == "да") { SendMsg(messageFromId, "манда!"); }
            if (answ=="удаф") { SendMsg(messageFromId, "http://udaff.com/"+Udaff()); }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            MyTelBot bot = new MyTelBot();
            bot.SetParam("349767489:AAFH2i94VcV9bl3RgTILJugxv6mYNTSEip8");
            bot.Init();
            while (true)
            {
                bot.GetUpdate();
                Thread.Sleep(100);
            }
        }
    }

}

