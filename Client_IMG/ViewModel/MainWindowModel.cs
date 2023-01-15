using Client_IMG.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Controls;
using System.IO;
using Color = System.Windows.Media.Color;

namespace Client_IMG.ViewModel
{
    internal class MainWindowModel : BaseViewModel
    {
        public RelayCommand Select_file { get; set; }
        public RelayCommand Send_Img { get; set; }
        private bool IsOkay { get; set; } = false;
        private MainWindow _MW {get; set; }
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress IPAddres = IPAddress.Loopback;
        IPEndPoint op;
        int port = 27001;
        byte[] data1;
        BitmapImage Image1 { get; set; }
        public MainWindowModel(MainWindow mw)
        {
            Send_Img = new RelayCommand(Client, CanTrue);
            Select_file = new RelayCommand(File_send, AlwaysTrue);
            Connection_Server();
            _MW = mw;
        }

        private void Connection_Server()
        {
            try
            {
                    op = new IPEndPoint(IPAddres, port);
                    socket.Connect(op);
            }
            catch (Exception)
            {
                MessageBox.Show("Can not connct to the server >>>");
                throw;
            }

        }
        private bool AlwaysTrue(object parametr) => true;
        private bool CanTrue (object parametr)
        {
            return IsOkay;
        }
        private void File_send(object parametr)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
            if (op.ShowDialog() == true)
            {
                Image1 = new BitmapImage(new Uri(op.FileName));
                ImageBrush brush  = new ImageBrush(Image1);
                _MW.Img_Background.Background = brush;

      
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Image1));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data1 = ms.ToArray();
                }


                IsOkay = true;

            }
          
        }
        private void Client(object parametr)
        {
            try
            {
                if (socket.Connected)
                {
                  

                        var msg = "Send";
                        var bytes = Encoding.UTF8.GetBytes(msg);
                        socket.Send(data1);
                    data1 = null;
                    Image1 = null;
           
                    _MW.Img_Background.Background = new SolidColorBrush(Colors.White);
                    IsOkay = false;

                    
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Can not connct to the server >>>");
           
                _MW.Close();
            }
        }
    }
}
