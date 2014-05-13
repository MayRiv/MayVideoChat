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
        public static bool exit = false; 
        private TcpListener imageTcpListener;
        private Socket listeningSocket;
        private WaveOut output;
        private Thread soundThread;
        private Thread imageThread;
        private Thread messageThread;
        private BufferedWaveProvider bufferStream;
        public ReceiverPresenter(IReceiverView view)
        {
            this.view = view;
            view.TryClose += view_TryClose;
            StartReceive();
        }

        void view_TryClose(object sender, EventArgs e)
        {
            exit = true;
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

            messageThread = new Thread(ListeningMessage);
            messageThread.Start();
        }
        
        private void Listening()
        {
            output.Play();
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 5555);
            try
            {
                while (!exit)
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
            finally
            {
                output.Stop();

            }
        }
        
        private void ListeningImage()
        {
           
            //EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 5555);
            EndPoint ep =  new IPEndPoint(IPAddress.Any,6666);
            
            imageTcpListener = new TcpListener((IPEndPoint)ep);
            imageTcpListener.Start();


            try
            {
                Socket s = imageTcpListener.AcceptSocket();
                IPEndPoint adress = (IPEndPoint)s.RemoteEndPoint;

                view.Call(new TryCallArguments(adress.Address));
                
                while (!exit)
                {
                    byte[] data = new byte[1024];
                    data = ReceiveVarData(s);
                    MemoryStream st = new MemoryStream(data);

                    view.Picture.Image = Bitmap.FromStream(st);
                }
            }
            catch (SocketException ex)
            { view.ShowError(ex); }
            
            
            finally
            {
                imageTcpListener.Stop();

            }
            
        }
        private void ListeningMessage()
        {
            var localIPSound = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);
            var listeningSocketMessage = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listeningSocketMessage.Bind(localIPSound);
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 7777);
            try
            {
                while (!exit)
                {
                    try
                    {
                        byte[] data = new byte[65535];
                        int received = listeningSocketMessage.ReceiveFrom(data, ref remoteIp);
                        char[] messageArray = new char[received];
                        for(int i = 0; i < received; i++)
                            messageArray[i] = (char)data[i];
                        string message = new string(messageArray);
                        //view.UpdateLog(message);
                        view.Log.Invoke(new Action(() => { view.Log.AppendText(message + Environment.NewLine); }));

                    }
                    catch (SocketException ex)
                    { view.ShowError(ex); }
                }
            }
            catch(Exception e)
            {
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
        
       

        
    }
}
