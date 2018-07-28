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

        public event VncCauseEventHandler DisconnectedEvent;

        private IntPtr    m_handle;
        private VncClient m_client;
        private Task      m_readTask;
        private CancellationTokenSource m_cancelTokenSource;
        private Image     m_image;
        private PointerEventParameter m_last;
        private DateTime m_lastPointerSendDt;
        private List<List<VncEncodeAbstract>> m_encodeList;
        private Size      m_prevSize;
        private Fraction  m_xZoom;
        private Fraction  m_yZoom;
        private bool      m_needsRedraw;
        private bool      m_prevConnected;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Initially double-buffering was enabled..
        /// However, in the case of double-buffering, it is necessary to update the whole screen every time.
        /// This is very slow.
        /// Therefore, double-buffering is invalidated and I draw only difference.
        /// </remarks>
        public VncControl()
        {
            InitializeComponent();

            m_handle = this.Handle;
            m_last   = new PointerEventParameter();
            m_lastPointerSendDt = new DateTime();
            m_encodeList = new List<List<VncEncodeAbstract>>();
            m_prevSize = new Size();
            m_needsRedraw   = false;
            m_prevConnected = false;

            // Dispose resouce
            this.Disposed += (s, e) =>
            {
                Disconnect();

                m_client?.Dispose();
                m_readTask?.Dispose();
                m_cancelTokenSource?.Dispose();
                m_image?.Dispose();
            };

            // Set keyboard event
            this.PreviewKeyDown += (s, e) =>
            {
                switch (e.KeyData)
                {
                    case Keys.Tab:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        // Do not move the focus even if you press the key.
                        e.IsInputKey = true;
                        break;
                }
            };
            this.KeyPress += (s, e) =>
            {
                if (m_client != null && m_client.Connected)
                {
                    // for text
                    m_client.WriteKeyEvent(VncEnum.KeyEventDownFlag.KeyDown, (uint)e.KeyChar);
                    m_client.WriteKeyEvent(VncEnum.KeyEventDownFlag.KeyUp,   (uint)e.KeyChar);
                    e.Handled = true;
                }
            };
            this.KeyDown += (s, e) =>
            {
                if (m_client != null && m_client.Connected)
                {
                    uint key = VncWindowsKeyMap.GetVncKey((uint)e.KeyCode);
                    if (key != 0)
                    {
                        // for special key
                        m_client.WriteKeyEvent(VncEnum.KeyEventDownFlag.KeyDown, key);
                        e.Handled = true;
                    }
                }
            };
            this.KeyUp += (s, e) =>
            {
                if (m_client != null && m_client.Connected)
                {
                    uint key = VncWindowsKeyMap.GetVncKey((uint)e.KeyCode);
                    if (key != 0)
                    {
                        // for special key
                        m_client.WriteKeyEvent(VncEnum.KeyEventDownFlag.KeyUp, key);
                        e.Handled = true;
                    }
                }
            };

            // If this is not set, changing the size will not erase existing pictures.
            this.ResizeRedraw = true;

            // Set mouse event
            this.MouseWheel += mouseEvent;
            this.MouseDown  += mouseEvent;
            this.MouseUp    += mouseEvent;
            this.MouseMove  += mouseEvent;
        }

        protected override void WndProc(ref Message a_msg)
        {
            const int WM_SIZE       = 0x0005;
            const int WM_UPDATEUISTATE = 0x0128;

            if (a_msg.Msg == WM_SIZE)
            {
                // When restoring the window, redraw this control.
                // Because the screen at the time of restoration has been cleared, it becomes strange to draw only the difference.
                // When minimizing the window, LParam is 0.
                // When restoreing the window, LParam is not 0.
                const int SIZE_RESTORED = 0;
                if ((int) a_msg.WParam == SIZE_RESTORED && (int)a_msg.LParam != 0)
                {
                    m_needsRedraw = true;
                }
            }
            else if (a_msg.Msg == WM_UPDATEUISTATE)
            {
                // When TAB or Alt is pressed, this control is cleared.
                // Therefore, when this event is received, redraw this control.
                // https://blogs.msdn.microsoft.com/oldnewthing/20161212-00/?p=94915
                m_needsRedraw = true;
                return;
            }
            base.WndProc(ref a_msg);
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
                        m_lastPointerSendDt = DateTime.Now;
                        m_last.Enable = false;
                    }

                    // NearestNeighbor is the fastest.
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    // At the time of connection, the background black is left, so erase it
                    if (!m_prevConnected)
                    {
                        m_prevConnected = true;
                        e.Graphics.Clear(this.BackColor);
                    }

                    // Lock to access below.
                    // - m_client.InternalCanvas
                    // - m_encodeList
                    lock (m_client.CanvasLock)
                    {
                        BitmapConverter.ToBitmap(m_client.InternalCanvas, (Bitmap)m_image);

                        // When the size changes, redraw it all.
                        // Or if the focus is lost, redraw it all.
                        if (m_prevSize.Width != this.Width || m_prevSize.Height != this.Height || m_needsRedraw)
                        {
                            m_prevSize.Width  = this.Width;
                            m_prevSize.Height = this.Height;
                            m_needsRedraw = false;

                            // I could not draw correctly if the size was not an integer ratio.
                            // Therefore, find the integer ratio that the denominator becomes 10 or less.
                            m_xZoom = new Fraction(this.Width,  m_client.ServerInitBody.FramebufferWidth,  10);
                            m_yZoom = new Fraction(this.Height, m_client.ServerInitBody.FramebufferHeight, 10);

                            int width  = (int)(m_client.ServerInitBody.FramebufferWidth  * m_xZoom.Numerator / m_xZoom.Denominator);
                            int height = (int)(m_client.ServerInitBody.FramebufferHeight * m_yZoom.Numerator / m_yZoom.Denominator);
                        
                            // Redraw  all
                            e.Graphics.DrawImage(m_image, 0, 0, width, height);
                        }
                        // If the size is the same, draw only the difference.
                        else
                        {
                            foreach (var list in m_encodeList)
                            {
                                foreach (var v in list)
                                {
                                    // To draw correctly, I calculate drawing source rectangle and drawing destination rectangle.
                                    // ex.
                                    //   Framebuffer = 640x480
                                    //   Control     = 320x240
                                    //   Source Rectangle(x,y,w,h) =  (101, 101, 10, 10)
                                    //                             => (100, 100, 12, 12) (Convert to be divisible)
                                    //   Dest   Rectangle(x,y,w,h) => (50,   50,  5,  5)
                                    int newX = (int)(v.X / m_xZoom.Denominator) * m_xZoom.Denominator;
                                    int newY = (int)(v.Y / m_yZoom.Denominator) * m_yZoom.Denominator;
                                    int newW = (int)(Math.Ceiling((double)(v.Width  + v.X - newX) / m_xZoom.Denominator) * m_xZoom.Denominator);
                                    int newH = (int)(Math.Ceiling((double)(v.Height + v.Y - newY) / m_yZoom.Denominator) * m_yZoom.Denominator);
                                    int xpos = newX * m_xZoom.Numerator / m_xZoom.Denominator;
                                    int ypos = newY * m_yZoom.Numerator / m_yZoom.Denominator;
                                    int width  = newW * m_xZoom.Numerator / m_xZoom.Denominator;
                                    int height = newH * m_yZoom.Numerator / m_yZoom.Denominator;
                                    e.Graphics.DrawImage(m_image, new Rectangle(xpos, ypos, width, height), newX, newY, newW, newH, GraphicsUnit.Pixel);
                                }
                            }
                            m_encodeList.Clear();
                        }
                    }
                    m_client.WriteFramebufferUpdateRequest();
                }
                else
                {
                    m_prevConnected = false;
                    e.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
                }
            }
        }

        public bool Connect(VncConfig a_config)
        {
            // Connect Vnc
            m_client?.Dispose();
            m_client = new VncClient(a_config);
            m_client.DisconnectedEvent += (s, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        DisconnectedEvent?.Invoke(s, e);
                    });
                }
                else
                {
                    DisconnectedEvent?.Invoke(s, e);
                }
            };
            bool retVal = m_client.ConnectVnc();
            if (!retVal)
            {
                m_client.Dispose();
                m_client = null;
                return false;
            }

            // Draw the whole for the first time.
            m_needsRedraw = true;

            // Initialize zoom ratio for mouseEvent.
            m_xZoom = new Fraction(this.Width,  m_client.ServerInitBody.FramebufferWidth,  10);
            m_yZoom = new Fraction(this.Height, m_client.ServerInitBody.FramebufferHeight, 10);

            // Create new image to draw OpenCv Mat. 
            m_image?.Dispose();
            m_image = new Bitmap(m_client.ServerInitBody.FramebufferWidth, m_client.ServerInitBody.FramebufferHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Create read thread
            m_cancelTokenSource?.Dispose();
            m_cancelTokenSource = new CancellationTokenSource();
            var token = m_cancelTokenSource.Token;
            m_readTask?.Dispose();
            m_readTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (m_client != null && m_client.Connected)
                    {
                        // If Read is failed, it returns null.
                        var body = m_client.ReadServerMessage();
                        if (body == null)
                        {
                            // Disconnect
                            m_client?.DisconnectVnc();

                            // Execute to draw this control black.
                            // Without this, the screen will not be updated and the VNC image will remain.
                            InvalidateRect(m_handle, (IntPtr)0, false);
                            return;
                        }
                        if (body.MessageType == VncEnum.MessageTypeServerToClient.FramebufferUpdate)
                        {
                            InvalidateRect(m_handle, (IntPtr)0, false);
                            lock (m_client.CanvasLock)
                            {
                                m_encodeList.Add(body.EncodeList);
                            }
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

            // Wait for completion or timeout (1 second).
            m_readTask?.Wait(1 * 1000);

            // Disconnect
            m_client?.DisconnectVnc();

            // Execute to draw this control black.
            // Without this, the screen will not be updated and the VNC image will remain.
            InvalidateRect(m_handle, (IntPtr)0, false);
        }

        private void mouseEvent(object sender, MouseEventArgs e)
        {
            if (m_client != null && m_client.Connected)
            {
                // If the size is too small, Numerator will be 0.
                // In this case, divide by 0, so do not do anything
                if (m_xZoom.Numerator == 0 || m_yZoom.Numerator == 0)
                {
                    return;
                }

                int xpos = (int)(e.X * m_xZoom.Denominator / m_xZoom.Numerator);
                int ypos = (int)(e.Y * m_yZoom.Denominator / m_yZoom.Numerator);

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

                // If FramebufferUpdate is not received, the screen is not updated and the mouse pointer event is not send.
                // Because there is such a scene, if you do not send the mouse pointer event for a certain time, send it.
                if ((DateTime.Now - m_lastPointerSendDt).TotalMilliseconds > 20)
                {
                    m_client.WritePointerEvent(mask, (UInt16)xpos, (UInt16)ypos);
                    m_last.Enable = false;
                }
                else
                {
                    // Since it is useless to send every time, only record it.
                    // Then, it transmits at the interval of OnPaint.
                    m_last.Enable = true;
                    m_last.Mask   = mask;
                    m_last.X      = xpos;
                    m_last.Y      = ypos;
                }
            }
        }
    }
}
