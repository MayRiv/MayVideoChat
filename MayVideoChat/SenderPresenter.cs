using AForge.Video;
using AForge.Video.DirectShow;
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
    class SenderPresenter
    {
        private bool isCalling = false;
        private Socket socket;
        
        private WaveIn waveIn;

        private ISenderView senderView;


        private FilterInfoCollection videoCaptureDevices;
        private VideoCaptureDevice camera;

        Socket imageSocket;
        public SenderPresenter(ISenderView view)
        {
            senderView = view;
            senderView.TryCall += new EventHandler<TryCallArguments>(onTryingCall);
            senderView.TryClose += new EventHandler<EventArgs>(senderView_TryClose);
            senderView.TrySendMessage += senderView_TrySendMessage;
        }

        void senderView_TrySendMessage(object sender, EventArgs e)
        {
            Socket messageSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            String text = senderView.Nickname + " : " + senderView.Message;
            char[] charArray = text.ToCharArray();

            byte[] array = new byte[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
                array[i] = (byte)charArray[i];
            messageSocket.SendTo(array, new IPEndPoint(IPAddress.Parse(senderView.IP),7777));
        }

        void senderView_TryClose(object sender, EventArgs e)
        {
            EndCall();
            //if (camera != null)
             //   camera.Stop();
        }
        private void onTryingCall(object sender, TryCallArguments e)
        {
            if (isCalling == true) return;
            else isCalling = true;
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(8000, 16, 1);
            IPEndPoint remotePoint = new IPEndPoint(e.Adress, 5555);
            waveIn.DataAvailable += (sender1, e1) =>
            {
                try
                {                
                    socket.SendTo(e1.Buffer, remotePoint);
                }
                catch (Exception ex)
                {
                    senderView.ShowError(ex);
                }
            };

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            waveIn.StartRecording();


            videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
           
            IPEndPoint ipep = new IPEndPoint(e.Adress, 6666);

            imageSocket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);
            imageSocket.Connect(ipep);
            
            camera = new VideoCaptureDevice(videoCaptureDevices[0].MonikerString);
            camera.NewFrame += camera_NewFrame;
            camera.Start();
        }
        private void camera_NewFrame(object sender, NewFrameEventArgs e)
        {
            Bitmap bitmap = (Bitmap)e.Frame.Clone();

            MemoryStream ms = new MemoryStream();

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            SendVarData(imageSocket,ms.GetBuffer());
        }
        private static int SendVarData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }
        private void EndCall()
        {
            isCalling = false;
            camera.Stop();
            waveIn.StopRecording();
        }
    }
}
