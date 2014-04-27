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
        private Thread soundThread;
        private Thread imageThread;
        BufferedWaveProvider bufferStream;
        public ReceiverPresenter(IReceiverView view)
        {
            this.view = view;
            view.TryClose += view_TryClose;
            StartReceive();
        }

        void view_TryClose(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StartReceive()
        {
       

            var localIPSound = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listeningSocket.Bind(localIPSound);

            output = new WaveOut();
            
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            output.Init(bufferStream);
            

            soundThread = new Thread(Listening);
            soundThread.Start();

            imageThread = new Thread(ListeningImage);
            imageThread.Start();
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
            
            
            try
            {
                Socket s = l.AcceptSocket();
                IPEndPoint adress = (IPEndPoint)s.RemoteEndPoint;
                try
                {
                    //view.TryCall.Invoke(this, new TryCallArguments(adress));
                }
                catch (Exception e)
                {

                }
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
        
       

        
    }
}
