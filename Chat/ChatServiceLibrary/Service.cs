using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Timers;

namespace ChatServiceLibrary
{
    //создается единственный экземпляр службы, 
    //которому направляются все сообщения от всех клиентов
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IService
    {
        public static List<ChatUser> listUsers = new List<ChatUser>(); 
        //Коллекция (словарь) для хранения информации о подключенных пользователях и каналов обратного вызова
        private static Dictionary<ChatUser, IClientCallback> conectedUsers =
            new Dictionary<ChatUser, IClientCallback>();

        //Список содержащий информащию о новых сообщениях
        private static List<ChatMessage> incomingMessages = new List<ChatMessage>();

        //таймер для обновления списка пользователей
        private  System.Timers.Timer timerUsersUpgrade;

        //таймер для добавления новых сообщений
        private  System.Timers.Timer timerMessagesUpgrade;

        public Service()
        {
            //периодическое обновление списка пользователей и добавление новых сообщений
            timerUsersUpgrade = new System.Timers.Timer(5000);
            timerUsersUpgrade.Elapsed += OnTimedEventNewListUsers;
            timerMessagesUpgrade = new System.Timers.Timer(500);
            timerMessagesUpgrade.Elapsed += OnTimedEventNewMessages;
            timerUsersUpgrade.Start();
            timerMessagesUpgrade.Start();
        }

        public void OnTimedEventNewListUsers(Object source, ElapsedEventArgs e)
        {
            //сервис самостоятельно устанавливает связь с каждым пользователем
            //и передает список всех пользователей
            ChatUser[] tempUsers = listUsers.ToArray();

            foreach (var cb in conectedUsers.Values)
            {
                cb.SendAllUsers(tempUsers);
            }
            //Callback.SendAllUsers(conectedUsers.Keys.ToArray());
        }

        private static void OnTimedEventNewMessages(Object source, ElapsedEventArgs e)
        {
            //для каждого пользователя по его каналу обратного вызова
            //передаю новые сообщения
            foreach(var user in conectedUsers)
            {
                user.Value.SendNewMessages(incomingMessages.ToArray());
            }

            //обнуление списка
            incomingMessages = new List<ChatMessage>();

        }

        public void ClientConnect(string userName)
        {
            //добавление нового пользователя в словарь
            //если такое ИМЯ еще не занято
            ChatUser user = new ChatUser() { UserName = userName };
            if (!conectedUsers.ContainsKey(user))
            {
                listUsers.Add(user);
                IClientCallback callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
                conectedUsers.Add(user, callback);
                incomingMessages.Add(new ChatMessage() { User = "Подключен новый человек", Date = DateTime.Now, Message = userName });
            }
        }

        //сохранение нового сообщения
        public void SendNewMessage(ChatMessage newMessage)
        {
            incomingMessages.Add(newMessage);
        }

        public void RemoveUser(string userName)
        {
            //удаление
            conectedUsers.Remove(new ChatUser() { UserName = userName });
            listUsers.Remove(new ChatUser() { UserName = userName });
        }
    }
}
