using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace Final_Project
{
    class ServerSetup
    {
        private Socket listenerSocket;
        private static TextBox chatFieldContainer;
        public Socket acceptedSocket;
        byte[] buffer = new byte[4];

        public ServerSetup(TextBox chatField)
        {
            chatFieldContainer = chatField;
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            StartListening();
            addTextToTextbox("Waiting for incoming connections! Clients can connect to you at " + getLocalIP());
        }
        public void StartListening()
        {
            try
            {
                listenerSocket.Bind(new IPEndPoint(IPAddress.Any, 12345));
                listenerSocket.Listen(1);
                listenerSocket.BeginAccept(acceptCallBack, listenerSocket);
            }
            catch (Exception ex)
            {
                addTextToTextbox("\nError Occured!\n" + ex + "\nPlease restart application and try again!");
            }
        }
        public void acceptCallBack(IAsyncResult ar)
        {
            try
            {
                acceptedSocket = listenerSocket.EndAccept(ar);
                replaceTextInTextbox("Connection Sucessful!");
                startRecieving();
            }
            catch (Exception ex)
            {
                addTextToTextbox("\nError Occured!\n" + ex + "\nPlease restart application and try again!");
            }
        }
        public void startRecieving()
        {
            buffer = new byte[4];
            acceptedSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiveCallBack, null);
        }
        private void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (acceptedSocket.EndReceive(ar) > 1)
                {
                    buffer = new byte[BitConverter.ToInt32(buffer, 0)];
                    acceptedSocket.Receive(buffer, buffer.Length, SocketFlags.None);
                    string data = Encoding.Default.GetString(buffer);
                    addTextToTextbox("\nPartner: " + data);
                    startRecieving();
                }
                else
                {
                    acceptedSocket.Disconnect(true);
                    addTextToTextbox("\nClient Disconnected, this chat session is over.");
                }
            }
            catch
            {
                if(!acceptedSocket.Connected)
                {
                    acceptedSocket.Disconnect(true);
                    addTextToTextbox("\nClient Disconnected, this chat session is over.");
                }
                else
                {
                    startRecieving();
                }
            }
        }

        private string getLocalIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
        private static void addTextToTextbox(string textToAdd)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => chatFieldContainer.AppendText(textToAdd)));
        }
        private static void replaceTextInTextbox(string textToAdd)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => chatFieldContainer.Text = textToAdd));
        }
    }
}
