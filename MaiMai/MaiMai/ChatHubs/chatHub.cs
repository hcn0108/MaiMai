using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MaiMai.Models;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace SignalRMvc.chatHubs
{


    public class chatHub : Hub
    {
        
        maimaiEntities db = new maimaiEntities();
        public static List<string> ConnectedMem = new List<string>();
        public void Group(int UserID)
        {
            var user = db.Member.Find(UserID);
                user.connectionID = Context.ConnectionId;
                db.SaveChanges();

            //加入陣列判斷是否在線上
            if (!ConnectedMem.Contains(user.connectionID))
            {
                ConnectedMem.Add(user.connectionID);
                Clients.All.isConnected(ConnectedMem.Contains(user.connectionID), UserID);
            }

            //判斷會員等級 管理員&優質會員>加入 VIP群組
            if (user.userLevel != 3)
            {
                var reload_user = db.Member.Find(UserID);
                Groups.Add(reload_user.connectionID, "VIP");
            }
        }
        public override Task OnConnected()
        {           
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userID = 0;
            var user = db.Member.FirstOrDefault(m => m.connectionID == Context.ConnectionId);
            if(user != null)
            {
                userID = user.UserID;
            }
            if (ConnectedMem.Contains(Context.ConnectionId))
            {
                
                ConnectedMem.Remove(Context.ConnectionId);
                Clients.All.isConnected(ConnectedMem.Contains(Context.ConnectionId), userID);
            }
            return base.OnDisconnected(stopCalled);
        }

        public void CheckConnectedMem(int UserID)
        {
            var checkMem = db.Member.Find(UserID).connectionID;
            
            Clients.Caller.isConnected(ConnectedMem.Contains(checkMem), UserID);
        }

        public void Send(int sender,  string message)
        {
            // 呼叫所有客戶端的sendMessage方法
            Clients.All.addMessage(message);

            Notification noti = new Notification() {
                SenderID = sender,
                ReciverLevel = "All",
                NotifyText = message,
                CreateTime = DateTime.Now,
                Status = true,
                Category = "系統"
            };

            db.Notification.Add(noti);
            db.SaveChanges();
        }

        public void SendToOne(int sender, int reciver, string message)
        {
            var user = db.Member.Find(reciver);
            if(user != null)
            {
                var nowNotificationID = db.Notification.Max(m => m.NotificationID)+1;
                Clients.Client(user.connectionID).addMessage(message, nowNotificationID,"系統");

                Notification noti = new Notification()
                {
                    SenderID = sender,
                    ReciverLevel = reciver.ToString(),
                    NotifyText = message,
                    CreateTime = DateTime.Now,
                    Status = false,
                    Category = "系統"
                };

                db.Notification.Add(noti);
                db.SaveChanges();
            }
        }        

        public void SendToVIP(int sender,  string message)
        {
            Clients.Group("VIP").addMessage(message);
           
            Notification noti = new Notification()
            {
                SenderID = sender,
                ReciverLevel = "VIP",
                NotifyText = message,
                CreateTime = DateTime.Now,
                Status = true,
                Category = "系統"
            };

            db.Notification.Add(noti);
            db.SaveChanges();
        }

        //下架通知
        public void SendToOne_OffSale(int sender, int reciver)
        {
            var user = db.Member.Find(reciver);
            var message = "貼文違規，已下架";
            if (user != null)
            {
                var nowNotificationID = db.Notification.Max(m => m.NotificationID) + 1;
                Clients.Client(user.connectionID).addMessage(message, nowNotificationID, "違規");

                Notification noti = new Notification()
                {
                    SenderID = sender,
                    ReciverLevel = reciver.ToString(),
                    NotifyText = message,
                    CreateTime = DateTime.Now,
                    Status = false,
                    Category = "違規"
                };

                db.Notification.Add(noti);
                db.SaveChanges();
            }
        }

        //訂單狀態變更通知
        public void SendToOne_OrderStatus(int sender, int reciver, int status)
        {
            ///status 1 = 買家結帳 通知賣家
            ///status 2 = 賣家確認訂單 通知買家
            ///status 3 = 賣家出貨 通知買家
            ///status 4 = 買家結單 通知賣家
            var user = db.Member.Find(reciver);
            var message = "";
            switch (status)
            {
                case 1: message = "有人訂購你的商品囉!"; break;
                case 2: message = "賣家已確認訂單!"; break;
                case 3: message = "賣家已出貨!"; break;
                case 4: message = "買家已結單!"; break;
            }
            if (user != null)
            {
                var nowNotificationID = db.Notification.Max(m => m.NotificationID) + 1;
                Clients.Client(user.connectionID).addMessage(message, nowNotificationID, "訂單");

                Notification noti = new Notification()
                {
                    SenderID = sender,
                    ReciverLevel = reciver.ToString(),
                    NotifyText = message,
                    CreateTime = DateTime.Now,
                    Status = false,
                    Category = "訂單"
                };

                db.Notification.Add(noti);
                db.SaveChanges();
            }
        }

        //聊天室用
        public void OneToOneChat(int sender, int reciver, string message)
        {
            var senderinfo = db.Member.Find(sender);
            var user = db.Member.Find(reciver);

            Chat chat = new Chat()
            {
                SenderID = sender,
                ReciverID = reciver,
                ChatText = message,
                ChatTime = DateTime.Now
            };

            db.Chat.Add(chat);
            db.SaveChanges();

            if (user != null)
            {
                Clients.Client(user.connectionID).reciverMessage(message, new
                {
                    senderinfo.UserID,
                    senderinfo.userAccount
                });
                Clients.Caller.senderMessage(message, new
                {
                    user.UserID,
                    user.userAccount
                });

            }
        }

    }
}