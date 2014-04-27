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
            senderView.TryClose += senderView_TryClose;
        }

        void senderView_TryClose(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void onTryingCall(object sender, TryCallArguments e)
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(8000, 16, 1);
            waveIn.DataAvailable += (sender1, e1) =>
            {
                try
                {
                    IPEndPoint remotePoint = new IPEndPoint(e.Adress, 5555);
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
           
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);

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
            // Save to memory using the Jpeg format
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
    }
}
