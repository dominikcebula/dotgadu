// DotGadu - example11
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using System.Collections.Generic;
using DotGadu;

namespace DotGaduTest
{
    class MainClass
    {
        private static bool end = false;
        private static Gadu gadu;

        public static void Main(string[] args)
        {
            GaduServer gaduServer = new GaduServer("91.197.13.26", 8074);
            gadu = new Gadu();

            gadu.Reciever.OnLoginOK += new OnLoginOKHandler(LoginOK);
            gadu.Reciever.OnLoginFailed += new OnLoginFailedHandler(LoginFailed);
            gadu.Reciever.OnUserListReplay += new OnUserListReplayHandler(UserList);

            gadu.Connect(gaduServer);
            gadu.Login(179824, "");

            while (end == false)
                System.Threading.Thread.Sleep(100);

            gadu.Disconnect();
        }

        public static void UserList(GaduPacketUserListRequest gaduPacketUserListReplay)
        {
            List<GaduUser> list = new List<GaduUser>();
            list = GaduUser.ParseUserListString(gaduPacketUserListReplay.Request);
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine(list[i].Uin + " " + list[i].Nick + " " + list[i].Email);
        }

        public static void LoginOK()
        {
            Console.WriteLine("Login OK");
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_BUSY, "");

            GaduUser user;

            user = new GaduUser();
            user.Uin = 100;
            user.Nick = "dominik";
            user.Email = "dominikcebula@gmail.com";
            gadu.UserList.Add(user);

            user = new GaduUser();
            user.Uin = 200;
            user.Nick = "user";
            user.Email = "user@gmail.com";
            gadu.UserList.Add(user);

            gadu.UserList.sendAll();

            gadu.UserList.getListFromServer();
        }

        public static void LoginFailed()
        {
            Console.WriteLine("Login Failed");
            end = true;
        }
    }
}
