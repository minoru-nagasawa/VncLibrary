using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VncLibrary;
using System.IO;
using System.Net.Sockets;
using OpenCvSharp.Extensions;
using System.Runtime.InteropServices;
using System.Threading;

namespace VncUiLibrary
{
    public partial class VncControl : UserControl
    {
        private class PointerEventParameter
        {
            public bool Enable
            {
                get;
                set;
            }
            public VncEnum.PointerEventButtonMask Mask
            {
                get;
                set;
            }
            public int X
            {
                get;
                set;
            }
            public int Y
            {
                get;
                set;
            }
        }

        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        private VncClient m_client;
        private Task      m_readTask;
        private CancellationTokenSource m_cancelTokenSource;
        private Image     m_image;
        private PointerEventParameter m_last;

        public VncControl()
        {
            InitializeComponent();

            // Dispose resouce
            this.Disposed += (s, e) =>
            {
                Disconnect();

                m_client?.Dispose();
                m_readTask?.Dispose();
                m_cancelTokenSource?.Dispose();
                m_image?.Dispose();
            };

            // For double buffering
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint,    true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Set mouse event
            this.MouseWheel += mouseEvent;
            this.MouseDown  += mouseEvent;
            this.MouseUp    += mouseEvent;
            this.MouseMove  += mouseEvent;

            m_last = new PointerEventParameter();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.DrawRectangle(Pens.DarkGray, 0, 0, this.Width - 1, this.Height - 1);
            }
            else
            {
                if (m_client != null && m_client.Connected)
                {
                    if (m_last.Enable)
                    {
                        m_client.WritePointerEvent(m_last.Mask, (UInt16)m_last.X, (UInt16)m_last.Y);
                        m_last.Enable = false;
                    }
                    lock (m_client.CanvasLock)
                    {
                        BitmapConverter.ToBitmap(m_client.InternalCanvas, (Bitmap)m_image);
                    }
                    e.Graphics.DrawImage(m_image, 0, 0, this.Width, this.Height);
                    m_client.WriteFramebufferUpdateRequest();
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
                }
            }
        }

        public bool Connect(VncConfig a_config)
        {
            // Connect Vnc
            m_client?.Dispose();
            m_client = new VncClient(a_config, (s) => new BufferedStream(new NetworkStream(s)), (s) => new NetworkStream(s));
            bool retVal = m_client.ConnectVnc();

            // Create new image to draw OpenCv Mat. 
            m_image?.Dispose();
            m_image = new Bitmap(m_client.ServerInitBody.FramebufferWidth, m_client.ServerInitBody.FramebufferHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Create read thread
            m_cancelTokenSource?.Dispose();
            m_cancelTokenSource = new CancellationTokenSource();
            var token = m_cancelTokenSource.Token;
            var handle = this.Handle;
            m_readTask?.Dispose();
            m_readTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (m_client != null && m_client.Connected)
                    {
                        var body = m_client.ReadServerMessage();
                        if (body.MessageType == VncEnum.MessageTypeServerToClient.FramebufferUpdate)
                        {
                            InvalidateRect(handle, (IntPtr)0, false);
                        }
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
            }, m_cancelTokenSource.Token);

            return retVal;
        }

        public void Disconnect()
        {
            // Cancel read thread
            m_cancelTokenSource?.Cancel();

            // There may not be any received messages and it may be blocked by Read.
            // Therefore, send FramebufferUpdate so that the client sends a message.
            m_client?.WriteFramebufferUpdateRequest();

            // Wait for completion or timeout (1 minute).
            m_readTask?.Wait(60 * 1000);

            // Disconnect
            m_client?.DisconnectVnc();

            // Execute to draw this control black.
            // Without this, the screen will not be updated and the VNC image will remain.
            InvalidateRect(this.Handle, (IntPtr)0, false);
        }

        private void mouseEvent(object sender, MouseEventArgs e)
        {
            if (m_client != null && m_client.Connected)
            {
                double xzoom = (double)this.Width  / m_client.ServerInitBody.FramebufferWidth;
                double yzoom = (double)this.Height / m_client.ServerInitBody.FramebufferHeight;
                int xpos = (int)(e.X / xzoom);
                int ypos = (int)(e.Y / yzoom);

                VncEnum.PointerEventButtonMask mask = VncEnum.PointerEventButtonMask.None;
                if (e.Button == MouseButtons.Left)
                {
                    mask |= VncEnum.PointerEventButtonMask.MouseButtonLeft;
                }
                if (e.Button == MouseButtons.Middle)
                {
                    mask |= VncEnum.PointerEventButtonMask.MouseButtonMiddle;
                }
                if (e.Button == MouseButtons.Right)
                {
                    mask |= VncEnum.PointerEventButtonMask.MouseButtonRight;
                }
                if (e.Delta > 0)
                {
                    mask |= VncEnum.PointerEventButtonMask.MouseWheelUp;
                }
                if (e.Delta < 0)
                {
                    mask |= VncEnum.PointerEventButtonMask.MouseWheelDown;
                }

                m_last.Enable = true;
                m_last.Mask   = mask;
                m_last.X      = xpos;
                m_last.Y      = ypos;
            }
        }
    }
}
