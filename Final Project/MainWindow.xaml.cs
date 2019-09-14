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
using System.Net;
using System.Net.Sockets;

namespace Final_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPAddress inputIP = null;
        bool hasIPinput = false;
        ServerSetup serverObject = null;
        ClientSetup clientObject = null;
        public MainWindow()
        {
            string msgText = "Would you like to be the server?\n If not click No to be Client\n Cancel to exit.";
            string dialogTitle = "Server/Client Selection";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxResult result = MessageBox.Show(msgText, dialogTitle, button);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    InitializeComponent();
                    sendButton.Content = "Send";
                    hasIPinput = true;
                    serverObject = new ServerSetup(chatField);
                    break;
                case MessageBoxResult.No:
                    InitializeComponent();
                    chatField.Text = "Please enter the IP address of the server in the format X.X.X.X";
                    break;
                default:
                    System.Windows.Application.Current.Shutdown();
                break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(hasIPinput==false)
            {
                sendButton.Content = "Send";
                inputIP=IPAddress.Parse(composeField.Text);
                hasIPinput = true;
                clientObject = new ClientSetup(inputIP, chatField);
                composeField.Text = "";
            }
            else if(serverObject!=null)
            {
                if(serverObject.acceptedSocket!=null && composeField.Text!="")
                {
                    SendText send = new SendText(serverObject.acceptedSocket, composeField.Text, chatField);
                    chatField.Text += "\nYou: " + composeField.Text;
                    composeField.Text = "";
                }
            }
            else if(clientObject!=null)
            {
                if (clientObject.connectingSocket != null && composeField.Text != "")
                {
                    SendText send = new SendText(clientObject.connectingSocket, composeField.Text, chatField);
                    chatField.Text += "\nYou: " + composeField.Text;
                    composeField.Text = "";
                }
            }
        }
        private void ComposeField_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Return || e.Key==Key.Enter)
            {
                Button_Click(new object(), new RoutedEventArgs());
            }
        }
    }
}