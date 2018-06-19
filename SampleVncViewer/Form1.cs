using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
        private Image     m_image;
        private Task      m_vncThread;

        public Form1()
        {
            InitializeComponent();

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
                m_client = new VncClient(config, (s) => new NetworkStream(s));

                bool result = await m_client.ConnectVncAsync();
                if (result)
                {
                    m_vncThread = Task.Factory.StartNew(async () =>
                    {
                        while (m_client.Connected)
                        {
                            var read = await m_client.ReadServerMessageAsync();
                            var messageType = (VncEnum.MessageTypeServerToClient)read[0];
                            if (messageType == VncEnum.MessageTypeServerToClient.FramebufferUpdate)
                            {
                                c_pictureBox.Invoke((MethodInvoker)delegate
                                {
                                    using (var m = new MemoryStream(m_client.CreateCanvasImage()))
                                    {
                                        m_image?.Dispose();
                                        m_image = Image.FromStream(m);
                                        c_pictureBox.Image = m_image;
                                    }
                                });

                                await m_client.WriteFramebufferUpdateRequestAsync();
                            }
                            //Thread.Sleep(50);
                        }
                    });

                    c_connectButton.Text = "Disconnect";
                    m_connectMode = !m_connectMode;
                }
            }
            else
            {
                m_client.Dispose();
                m_client = null;

                c_connectButton.Text = "Connect";
                m_connectMode = !m_connectMode;
            }
        }
    }
}
