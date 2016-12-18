using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.ServiceReference1;

namespace Client
{
    public class CallbackEventArgs : EventArgs
    {
        // Массив объектов типа ChatUser
        public readonly ChatUser[] Users;

        // Массив объектов типа ChatMessage
        public readonly ChatMessage[] Messages;

        // Конструктор (сцепленный с основным)
        public CallbackEventArgs(ChatUser[] users)
            : this(users, null)
        {
        }

        // Конструктор (сцепленный с основным)
        public CallbackEventArgs(ChatMessage[] messages)
            : this(null, messages)
        {
        }

        // Конструктор (основной)
        public CallbackEventArgs(ChatUser[] users, ChatMessage[] messages)
        {
            Users = users;
            Messages = messages;
        }
    }
}
