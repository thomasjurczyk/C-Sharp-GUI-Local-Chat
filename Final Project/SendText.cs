using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Final_Project
{
    class SendText
    {
        static TextBox chatFieldContainer;
        public SendText(Socket sendOnThis, string textToSend, TextBox chatBox)
        {
            chatFieldContainer = chatBox;
            try
            {
                List<byte> fullMessage = new List<byte>();
                fullMessage.AddRange(BitConverter.GetBytes(textToSend.Length));
                fullMessage.AddRange(Encoding.Default.GetBytes(textToSend));
                sendOnThis.Send(fullMessage.ToArray());
            }
            catch(Exception ex)
            {
                addTextToTextbox("\nError Occured!\n" + ex + "\nPlease try to send this message again\n" + "Socket info at time:\n" + sendOnThis.Connected);
            }
        }
        private static void addTextToTextbox(string textToAdd)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => chatFieldContainer.AppendText(textToAdd)));
        }
    }
}
