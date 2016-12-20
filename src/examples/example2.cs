// DotGadu - example2
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using DotGadu;

namespace DotGaduTest
{
    class MainClass
    {
        public static bool exit = false;
        public static Gadu gadu;

        public static void Main(string[] args)
        {
            GaduServer gaduServer = new GaduServer("91.197.13.26", 8074);
            gadu = new Gadu();

            gadu.Reciever.OnLoginOK += new OnLoginOKHandler(loginok);
            gadu.Reciever.OnLoginFailed += new OnLoginFailedHandler(loginfailed);
            gadu.Reciever.OnRecieveMessage += new OnRecieveMessageHandler(RecvMsg);
            gadu.Reciever.OnRecieveMessageAck += new OnRecieveMessageAckHandler(MsgAck);

            gadu.Sender.OnPacketSent += new OnPacketSentHandler(packet);

            gadu.Connect(gaduServer);

            gadu.Login(179824, "password");

            while (!exit)
                System.Threading.Thread.Sleep(1000);

            Console.WriteLine("Koncze dzialanie");

            gadu.Logout();
            gadu.Disconnect();
        }

        public static void packet(IGaduPacket packet)
        {
            Console.WriteLine("Packet Sent...");
        }

        public static void loginok()
        {
            Console.WriteLine("Logowanie OK :)");
            gadu.sendMessage(3837462, "Witam, wlasnie testuje DotGadu :-)");
        }

        public static void loginfailed()
        {
            Console.WriteLine("Blad logowania :(");
            exit = true;	// nie udalo nam sie zalogowac, oznacz exit na true, program za chwile zakonczy dzialanie
        }

        public static void RecvMsg(GaduPacketRecieveMessage msg)
        {
            Console.WriteLine("Wiadomosc od " + msg.Sender + ":" + msg.Message);
            if (msg.Message == "exit\0")
                exit = true;
        }

        public static void MsgAck(GaduPacketMessageAck ack)
        {
            Console.WriteLine("Odebralem pakiet mowiacy o statusie wiadmosci");
            if (ack.Status == GaduPacketConstans.GG_ACK_BLOCKED)
                Console.WriteLine("Wiadomosc zablokowana");
            else if (ack.Status == GaduPacketConstans.GG_ACK_DELIVERED)
                Console.WriteLine("Wiadomosc dostarczono");
            else if (ack.Status == GaduPacketConstans.GG_ACK_QUEUED)
                Console.WriteLine("Wiadomosc zakoljekowano");
            else if (ack.Status == GaduPacketConstans.GG_ACK_MBOXFULL)
                Console.WriteLine("Odbiorca ma pelna skrzynke odbiorcza");
            else if (ack.Status == GaduPacketConstans.GG_ACK_NOT_DELIVERED)
                Console.WriteLine("Wiadomosc nie dostarczona");
        }
    }
}
