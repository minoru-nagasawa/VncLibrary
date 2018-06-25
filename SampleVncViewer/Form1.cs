using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VncLibrary;

namespace SampleVncViewer
{
    public partial class Form1 : Form
    {
        private VncClient m_client;
        private bool      m_connectMode;

        public Form1()
        {
            InitializeComponent();
            c_addressTextBox.Text = Properties.Settings.Default.Address;
            c_passwordTextBox.Text = Properties.Settings.Default.Password;
            c_vnc33CheckBox.Checked = Properties.Settings.Default.Use33;

            m_connectMode = true;
        }

        private async void c_connectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(c_addressTextBox.Text)
            ||  string.IsNullOrEmpty(c_passwordTextBox.Text))
            {
                return;
            }

            if (m_connectMode)
            {
                var config = new VncConfig(c_addressTextBox.Text,
                                           5900,
                                           c_passwordTextBox.Text,
                                           c_vnc33CheckBox.Checked ? VncEnum.Version.Version33 : VncEnum.Version.None);
                m_client = new VncClient(config, (s) => new BufferedStream(new NetworkStream(s)), (s) => new NetworkStream(s));

                bool result = await m_client.ConnectVncAsync();
                if (result)
                {
                    c_connectButton.Text = "Disconnect";
                    m_connectMode = false;

                    using (var window = new OpenCvSharp.Window(m_client.ServerInitBody.NameString))
                    {
                        window.OnMouseCallback += async (ev, x, y, flags) =>
                        {
                            VncEnum.PointerEventButtonMask mask = VncEnum.PointerEventButtonMask.None;
                            if ((flags & OpenCvSharp.MouseEvent.FlagLButton) != 0)
                            {
                                mask |= VncEnum.PointerEventButtonMask.MouseButtonLeft;
                            }
                            if ((flags & OpenCvSharp.MouseEvent.FlagMButton) != 0)
                            {
                                mask |= VncEnum.PointerEventButtonMask.MouseButtonMiddle;
                            }
                            if ((flags & OpenCvSharp.MouseEvent.FlagRButton) != 0)
                            {
                                mask |= VncEnum.PointerEventButtonMask.MouseButtonRight;
                            }
                            if ((ev & OpenCvSharp.MouseEvent.MouseWheel) != 0)
                            {
                                if (flags > 0)
                                {
                                    mask |= VncEnum.PointerEventButtonMask.MouseWheelUp;
                                }
                                else
                                {
                                    mask |= VncEnum.PointerEventButtonMask.MouseWheelDown;
                                }
                            }
                            await m_client.WritePointerEventAsync(mask, (UInt16)x, (UInt16)y);
                        };

                        while (!m_connectMode)
                        {
                            var read = await m_client.ReadServerMessageAsync();
                            var messageType = (VncEnum.MessageTypeServerToClient)read[0];
                            if (messageType == VncEnum.MessageTypeServerToClient.FramebufferUpdate)
                            {
                                window.ShowImage(m_client.InternalCanvas);
                                await m_client.WriteFramebufferUpdateRequestAsync();
                            }
                        }
                        OpenCvSharp.Window.DestroyAllWindows();
                        m_client.Dispose();
                        m_client = null;
                    }
                }
            }
            else
            {
                c_connectButton.Text = "Connect";
                m_connectMode = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Address = c_addressTextBox.Text;
            Properties.Settings.Default.Password = c_passwordTextBox.Text;
            Properties.Settings.Default.Use33 = c_vnc33CheckBox.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
