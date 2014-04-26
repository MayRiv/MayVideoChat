using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MayVideoChat
{
    class ReceiverPresenter
    {
        private IReceiverView view;
        private Socket listeningSocket;
        private WaveOut output;
        private Thread thread;
        BufferedWaveProvider bufferStream;
        public ReceiverPresenter(IReceiverView view)
        {
            this.view = view;
            view.StartReceive += view_StartReceive;
        }

        void view_StartReceive(object sender, EventArgs e)
        {
       

            var localIPSound = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listeningSocket.Bind(localIPSound);

             output = new WaveOut();
            
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            output.Init(bufferStream);
            

            thread = new Thread(Listening);
            thread.Start();

            var thread2 = new Thread(ListeningImage);
            thread2.Start();
            
           
            //ListeningImage();
           

        }
        
        private void Listening()
        {
            output.Play();
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    byte[] data = new byte[65535];
                    int received = listeningSocket.ReceiveFrom(data, ref remoteIp);
                    bufferStream.AddSamples(data, 0, received);
                }
                catch (SocketException ex)
                { view.ShowError(ex); }
            }
        }
        
        private void ListeningImage()
        {
            output.Play();
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 5555);
            EndPoint ep =  new IPEndPoint(IPAddress.Any,6666);
            
            TcpListener l = new TcpListener((IPEndPoint)ep);
            l.Start();
            
            while (true)
            {
                try
                {
                    Socket s = l.AcceptSocket();
                    
                    NetworkStream stream = new NetworkStream(s);
                    
                    //view.Picture.Image = Bitmap.FromStream(stream);
                    while (true)
                    {
                        byte[] data = new byte[1024];
                        data = ReceiveVarData(s);
                        MemoryStream st = new MemoryStream(data);
                        
                        view.Picture.Image = Bitmap.FromStream(st);
                        
                    }
                    

                }
                catch (SocketException ex)
                { view.ShowError(ex); }
            }
        }

        private static byte[] ReceiveVarData(Socket s)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];

            recv = s.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, 0);
                if (recv == 0)
                {
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }
           //private void f(Object s)
           //{
           //    NetworkStream stream = new NetworkStream((Socket)s);
               
           //         while (true)
           //         {
           //            //if (stream.DataAvailable)
           //                 view.Picture.Image = Bitmap.FromStream(stream);
           //         }

           //}
       

        
    }
}
