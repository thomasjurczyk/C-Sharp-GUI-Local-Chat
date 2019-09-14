using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Threading;
using System.Windows;

namespace Final_Project
{
    class ClientSetup
    {
        static TextBox textboxContainer;
        IPAddress connectToThis;
        public Socket connectingSocket;
        byte[] buffer = new byte[4];

        public ClientSetup(IPAddress IPToConnect, TextBox chatBox)
        {
            textboxContainer = chatBox;
            connectToThis = IPToConnect;
            tryToConnect();
        }
        public void tryToConnect()
        {
            connectingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while(!connectingSocket.Connected)
            {
                Thread.Sleep(1000);
                try
                {
                    connectingSocket.Connect(new IPEndPoint(connectToThis, 12345));
                }
                catch { }
            }
            replaceTextInTextbox("Connection Sucessful!");
            startRecieving();
        }
        public void startRecieving()
        {
            buffer = new byte[4];
            connectingSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiveCallBack, null);
        }
        private void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (connectingSocket.EndReceive(ar) > 1)
                {
                    buffer = new byte[BitConverter.ToInt32(buffer, 0)];
                    connectingSocket.Receive(buffer, buffer.Length, SocketFlags.None);
                    string data = Encoding.Default.GetString(buffer);
                    addTextToTextbox("\nPartner: " + data);
                    startRecieving();
                }
                else
                {
                    connectingSocket.Disconnect(true);
                    addTextToTextbox("\nServer Disconnected, this chat session is over.");
                }
            }
            catch
            {
                if (!connectingSocket.Connected)
                {
                    connectingSocket.Disconnect(true);
                    addTextToTextbox("\nServer Disconnected, this chat session is over.");
                }
                else
                {
                    startRecieving();
                }
            }
        }
        private static void addTextToTextbox(string textToAdd)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => textboxContainer.AppendText(textToAdd)));
        }
        private static void replaceTextInTextbox(string textToAdd)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => textboxContainer.Text=textToAdd));
        }
    }
}
