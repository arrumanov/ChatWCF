using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChatServiceLibrary
{
    //служба создаст сессию, если входящая привязка поддерживает сессии
    [ServiceContract(SessionMode = SessionMode.Allowed, CallbackContract = typeof(IClientCallback))]
    public interface IService
    {
        //операция подключения пользователя
        [OperationContract]
        void ClientConnect(string userName);

        //операция добавления новых сообщений в словарь
        [OperationContract]
        void SendNewMessage(ChatMessage newMessage);

        //операция отключения пользователя
        [OperationContract]
        void RemoveUser(string userName);  
    }

    //контракт обратного вызова
    //реализуется клиентом, и может вызываться службой
    public interface IClientCallback
    {
        //рассылка новых сообщений всем подключенным пользователям
        [OperationContract(IsOneWay = true)]
        void SendNewMessages(ChatMessage[] messages);

        //рассылка информации о пользователях
        [OperationContract(IsOneWay = true)]
        void SendAllUsers(ChatUser[] users);
    }

    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return this.User + " <" + this.Date + ">:" + this.Message;
        }
    }

    [DataContract]
    public class ChatUser
    {
        [DataMember]
        public string UserName { get; set; }

        public override bool Equals(object obj)
        {
            return this.ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return this.UserName;
        }
    }
}
