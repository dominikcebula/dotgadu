// DotGadu - example9
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using System.Collections.Generic;
using System.Text;
using DotGadu;

namespace BotGadu
{
    class Program
    {
        private static Gadu gadu;
        private static DateTime start;

        private static void Connect()
        {
            try
            {
                Console.WriteLine("Connecting...");
                GaduServer gaduServer = new GaduServer("91.197.13.26", 8074);
                gadu = new Gadu();

                gadu.Reciever.OnLoginOK += new OnLoginOKHandler(Reciever_OnLoginOK);
                gadu.Reciever.OnLoginFailed += new OnLoginFailedHandler(Reciever_OnLoginFailed);
                gadu.Reciever.OnRecieveMessage += new OnRecieveMessageHandler(Reciever_OnRecieveMessage);
                gadu.Sender.OnPacketSent += new OnPacketSentHandler(Sender_OnPacketSent);

                gadu.Connect(gaduServer);
                gadu.Login(179824, "secretpass");
                gadu.changeStatus(GaduPacketConstans.GG_STATUS_AVAIL_DESCR, "http://dotgadu.sf.net");
            }
            catch (Exception e)
            {
                Console.WriteLine("E: " + e.StackTrace);
                Console.WriteLine("E: " + e.Message);
                ReConnect();
            }
        }

        static void Sender_OnPacketSent(IGaduPacket packet)
        {
            Console.WriteLine("packet...");
        }

        static void Reciever_OnRecieveMessage(GaduPacketRecieveMessage msg)
        {
            String msgs;
            DateTime minus;
            Console.WriteLine(msg.Sender + ":" + msg.Message);
            minus = new DateTime(DateTime.Now.ToBinary() - start.ToBinary());
            msgs = "Witam\nJestem botem napisanym w\nDotGadu - http://dotgadu.sf.net\n";
            msgs += "Dzialam juz " + (minus.DayOfYear - 1) + " dni " + minus.Hour + " godzin " + minus.Minute + " minut " + minus.Second + " sec";
            gadu.sendMessage(msg.Sender, msgs);
        }

        static void Reciever_OnLoginFailed()
        {
            Console.WriteLine("Login Failed");
            ReConnect();
        }

        static void Reciever_OnLoginOK()
        {
            Console.WriteLine("Login OK");
        }

        private static void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            try
            {
                gadu.Logout();
            }
            catch (Exception e)
            {
                Console.WriteLine("E: " + e.Message);
                Console.WriteLine("E: " + e.StackTrace);
            }
            finally
            {
                gadu.Disconnect();
                gadu = null;
            }
        }

        private static void ReConnect()
        {
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("ReConnecting...");
            Disconnect();
            System.Threading.Thread.Sleep(8000);
            Connect();
        }

        static void Main(string[] args)
        {
            start = DateTime.Now;
            Gadu.OnGaduCriticalError += new OnGaduCriticalErrorHandler(Gadu_OnGaduCriticalError);
            Connect();

            String cmd;
            while (true)
            {
                cmd = Console.ReadLine();
                if (cmd == "reconnect")
                    ReConnect();
                else if (cmd == "exit")
                    break;
            }

            gadu.Logout();
            gadu.Disconnect();
        }

        static void Gadu_OnGaduCriticalError()
        {
            Console.WriteLine("Critical error!");
            ReConnect();
        }
    }
}
