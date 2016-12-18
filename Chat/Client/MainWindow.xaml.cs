using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using Client.ServiceReference1;


namespace Client
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CallbackHandler : IServiceCallback
    {
        // Событие о получении списка пользователей
        public event EventHandler<CallbackEventArgs> UsersEven = delegate { };
        
        // Событие о получении новых сообщений
        public event EventHandler<CallbackEventArgs> NewMessagesEvent = delegate { };


        //передача новых сообщений
        public void SendNewMessages(ChatMessage[] messages)
        {
            CallbackEventArgs args = new CallbackEventArgs(messages);
            OnMessageEvent(args);
        }

        //передача информации о пользователях
        public void SendAllUsers(ChatUser[] users)
        {
            CallbackEventArgs args = new CallbackEventArgs(users);
            OnUsersEvent(args);
        }

        // Метод вызова события UsersEven
        protected virtual void OnUsersEvent(CallbackEventArgs e)
        {
            if (UsersEven != null)
            {
                UsersEven(this, e);
            }
        }

        // Метод вызова события NewMessagesEvent
        protected virtual void OnMessageEvent(CallbackEventArgs e)
        {
            if (NewMessagesEvent != null)
            {
                NewMessagesEvent(this, e);
            }
        }
    }

    public partial class MainWindow : Window
    {
        
        public static ServiceClient proxy = null;
        public CallbackHandler MyCallback = new CallbackHandler();

        public string MyName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MyCallback.UsersEven += new EventHandler<CallbackEventArgs>(callback_UsersEven);
            MyCallback.NewMessagesEvent += new EventHandler<CallbackEventArgs>(callback_MessageEvent);
        }

        //регистрация
        private void btnLog_in_out_Click(object sender, RoutedEventArgs e)
        {
            //форма для ввода имени
            LoginWindow loginDialog = new LoginWindow();
            loginDialog.ShowDialog();
            if (loginDialog.UserName != "")
            {
                proxy.ClientConnect(loginDialog.UserName);
                MyName = loginDialog.UserName;
            }
            btnLog_in_out.Content = "Выход";
            btnLog_in_out.Click -= btnLog_in_out_Click;
            btnLog_in_out.Click += btnExit_Click;
        }

        //задание proxy
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InstanceContext site = new InstanceContext(MyCallback);
            proxy = new ServiceClient(site);
        }

        //выход из чата
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            proxy.RemoveUser(MyName);
            btnSend.IsEnabled = false;
        }


        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ChatMessage newMessage = new ChatMessage() { User = MyName, Message = txtMessage.Text, Date = DateTime.Now };
            proxy.SendNewMessage(newMessage);
            txtMessage.Text = "";
        }

        // Обработчик события MessageEvent объекта обратного вызова
        void callback_MessageEvent(object sender, CallbackEventArgs e)
        {
            foreach (var item in e.Messages)
            {
                List<string> str = new List<string>();
                lstChat.Dispatcher.BeginInvoke((Action)(() =>
                {
                    string info = item.User + " <" + item.Date + ">:" + item.Message;
                    lstChat.Items.Add(info);
                }));
            }
        }

        // Обработчик события UsersEven объекта обратного вызова
        void callback_UsersEven(object sender, CallbackEventArgs e)
        {
            lstUsers.Dispatcher.BeginInvoke(new Action(delegate()
            {
                lstUsers.Items.Clear();
            }));
            foreach(var item in e.Users)
            {
                lstUsers.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    string info = item.UserName;
                    lstUsers.Items.Add(info);
                }));
            }
        }
    }
}
