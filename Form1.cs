using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace chatClient
{
    // класс создания визуализации чата
    public partial class chatForm : Form
    {
        private delegate void printer(string data);
        private delegate void cleaner();
        printer Printer;
        cleaner Cleaner;
        private Socket _serverSocket;
        private Thread _clientThread;
        private const string _serverHost = "localhost";
        private const int _serverPort = 9933;


        public chatForm()
        {
            InitializeComponent();
            Printer = new printer(print);
            Cleaner = new cleaner(clearChat);
            connect();
            _clientThread = new Thread(listner);
            _clientThread.IsBackground = true;
            _clientThread.Start();
        }
        private void listner()
        {
            while (_serverSocket.Connected)
            {
                byte[] buffer = new byte[8196];
                int bytesRec = _serverSocket.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                if (data.Contains("#updatechat"))
                {
                    UpdateChat(data);
                    continue;
                }
            }
        }
        // проверка связи с сервером
        private void connect()
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(_serverHost);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, _serverPort);
                _serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(ipEndPoint);
            }
            catch { print("Сервер недоступен!"); }
        }

        // очистка чата 
        private void clearChat()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(Cleaner);
                return;
            }
            chatBox.Clear();
        }

        // Обнволение чата со связью с другими участниками

        private void UpdateChat(string data)
        {
            //#updatechat&userName~data|username~data
            clearChat();
            string[] messages = data.Split('&')[1].Split('|');
            int countMessages = messages.Length;
            if (countMessages <= 0) return;
            for (int i = 0; i < countMessages; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(messages[i])) continue;
                    print(String.Format("[{0}]:{1}.", messages[i].Split('~')[0], messages[i].Split('~')[1]));
                }
                catch { continue; }
            }
        }

        // отправка данных на сервер
        private void send(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int bytesSent = _serverSocket.Send(buffer);
            }
            catch { print("Связь с сервером прервалась...");}
        }

        // вывод сообщений в чат
        private void print(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(Printer, msg);
                return;
            }
            if (chatBox.Text.Length == 0)
                chatBox.AppendText(msg);
            else
                chatBox.AppendText(Environment.NewLine + msg);
        }

        // вход в чат нажатием кнопки
        private void enterChat_Click(object sender, EventArgs e)
        {
            string Name = userName.Text;
            if (string.IsNullOrEmpty(Name)) return;
            send("#setname&" + Name);
            chatBox.Enabled = true;
            chat_msg.Enabled = true;
            chat_send.Enabled = true;
            userName.Enabled = false;
            enterChat.Enabled = false;
        }

        // нажатие клавиши отправки в чат
        private void chat_send_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        // отправка сообщений в чат 
        private void sendMessage()
        {
            try
            {
                string data = chat_msg.Text;
                if (string.IsNullOrEmpty(data)) return;
                send("#newmsg&" + data);
                chat_msg.Text = string.Empty;
            }
            catch { MessageBox.Show("Ошибка при отправке сообщения!"); }
        }

        private void chatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                sendMessage();
        }

        private void chat_msg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                sendMessage();
        }
    }
}
