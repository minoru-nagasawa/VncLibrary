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
    public partial class MainForm : Form
    {
        private bool m_connectMode;

        public MainForm()
        {
            InitializeComponent();
            c_addressTextBox.Text   = Properties.Settings.Default.Address;
            c_passwordTextBox.Text  = Properties.Settings.Default.Password;
            if (Properties.Settings.Default.EncodingList == null)
            {
                Properties.Settings.Default.EncodingList = new System.Collections.Specialized.StringCollection();
                foreach (var v in VncClient.DefaultEncodeTypes)
                {
                    Properties.Settings.Default.EncodingList.Add(((int) v).ToString());
                    Properties.Settings.Default.Save();
                }
            }

            c_vncControl.DisconnectedEvent += (s, e) =>
            {
                c_connectButton.Text = "Connect";
                m_connectMode = true;
            };

            m_connectMode = true;
        }
        
        private void c_connectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(c_addressTextBox.Text)
            ||  string.IsNullOrEmpty(c_passwordTextBox.Text))
            {
                return;
            }

            if (m_connectMode)
            {
                // Protocol Version
                VncEnum.Version version = VncEnum.Version.None;
                if (Properties.Settings.Default.Protocol33)
                {
                    version = VncEnum.Version.Version33;
                }
                else if (Properties.Settings.Default.Protocol37)
                {
                    version = VncEnum.Version.Version37;
                }
                else if (Properties.Settings.Default.Protocol38)
                {
                    version = VncEnum.Version.Version38;
                }
                // Encodings
                var useEncodingList = new List<VncEnum.EncodeType>();
                foreach (var v in Properties.Settings.Default.EncodingList)
                {
                    useEncodingList.Add((VncEnum.EncodeType)(int.Parse(v)));
                }
                // Colour
                bool isColourSpecified = !Properties.Settings.Default.ColourLevelReceived;
                PixelFormat specifiedColor = PixelFormat.PixelFormatDepth24;
                if (Properties.Settings.Default.ColourLevelDepth16)
                {
                    specifiedColor = PixelFormat.PixelFormatDepth16;
                }
                else if (Properties.Settings.Default.ColourLevelDepth8Colour64)
                {
                    specifiedColor = PixelFormat.PixelFormatDepth8Colour64;
                }
                else if (Properties.Settings.Default.ColourLevelDepth8Colour8)
                {
                    specifiedColor = PixelFormat.PixelFormatDepth8Colour8;
                }

                var config = new VncConfig(c_addressTextBox.Text,
                                           5900,
                                           c_passwordTextBox.Text,
                                           version,
                                           useEncodingList.ToArray(),
                                           isColourSpecified,
                                           specifiedColor,
                                           Properties.Settings.Default.SendKeyboard,
                                           Properties.Settings.Default.SendPointer);
                bool result = c_vncControl.Connect(config);
                if (result)
                {
                    c_connectButton.Text = "Disconnect";
                    m_connectMode = false;
                }
            }
            else
            {
                c_vncControl.Disconnect();
                c_connectButton.Text = "Connect";
                m_connectMode = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Address  = c_addressTextBox.Text;
            Properties.Settings.Default.Password = c_passwordTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void c_buttonSettings_Click(object sender, EventArgs e)
        {
            using (var dialog = new SettingDialog())
            {
                // Encoding
                var useEncodingSet = new HashSet<VncEnum.EncodeType>();
                foreach (var v in Properties.Settings.Default.EncodingList)
                {
                    var encodeType = (VncEnum.EncodeType)(int.Parse(v));
                    dialog.c_listBoxEncodingsUsed.Items.Add(encodeType);
                    useEncodingSet.Add(encodeType);
                }
                foreach (var v in VncClient.DefaultEncodeTypes)
                {
                    if (!useEncodingSet.Contains(v))
                    {
                        dialog.c_listBoxEncodingsUnused.Items.Add(v);
                    }
                }

                // Colour Level
                if (Properties.Settings.Default.ColourLevelReceived)
                {
                    dialog.c_radioButtonColourReceived.Checked = true;
                }
                else if (Properties.Settings.Default.ColourLevelDepth24)
                {
                    dialog.c_radioButtonColourDepth24.Checked = true;
                }
                else if (Properties.Settings.Default.ColourLevelDepth16)
                {
                    dialog.c_radioButtonColourDepth16.Checked = true;
                }
                else if (Properties.Settings.Default.ColourLevelDepth8Colour64)
                {
                    dialog.c_radioButtonColourDepth8Colour64.Checked = true;
                }
                else if (Properties.Settings.Default.ColourLevelDepth8Colour8)
                {
                    dialog.c_radioButtonColourDepth8Colour8.Checked = true;
                }

                // Protocol
                if (Properties.Settings.Default.ProtocolReceived)
                {
                    dialog.c_radioButtonProtocolReceived.Checked = true;
                }
                else if (Properties.Settings.Default.Protocol33)
                {
                    dialog.c_radioButtonProtocol33.Checked = true;
                }
                else if (Properties.Settings.Default.Protocol37)
                {
                    dialog.c_radioButtonProtocol37.Checked = true;
                }
                else if (Properties.Settings.Default.Protocol38)
                {
                    dialog.c_radioButtonProtocol38.Checked = true;
                }

                // Misc
                dialog.c_checkBoxSendPointer.Checked  = Properties.Settings.Default.SendPointer;
                dialog.c_checkBoxSendKeyboard.Checked = Properties.Settings.Default.SendKeyboard;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Encoding
                    Properties.Settings.Default.EncodingList.Clear();
                    foreach (var v in dialog.c_listBoxEncodingsUsed.Items)
                    {
                        Properties.Settings.Default.EncodingList.Add(((int)v).ToString());
                    }

                    // Colour Level
                    Properties.Settings.Default.ColourLevelReceived = false;
                    Properties.Settings.Default.ColourLevelDepth24  = false;
                    Properties.Settings.Default.ColourLevelDepth16  = false;
                    Properties.Settings.Default.ColourLevelDepth8Colour64 = false;
                    Properties.Settings.Default.ColourLevelDepth8Colour8  = false;
                    if (dialog.c_radioButtonColourReceived.Checked)
                    {
                        Properties.Settings.Default.ColourLevelReceived = true;
                    }
                    else if (dialog.c_radioButtonColourDepth24.Checked)
                    {
                        Properties.Settings.Default.ColourLevelDepth24 = true;
                    }
                    else if (dialog.c_radioButtonColourDepth16.Checked)
                    {
                        Properties.Settings.Default.ColourLevelDepth16 = true;
                    }
                    else if (dialog.c_radioButtonColourDepth8Colour64.Checked)
                    {
                        Properties.Settings.Default.ColourLevelDepth8Colour64 = true;
                    }
                    else if (dialog.c_radioButtonColourDepth8Colour8.Checked)
                    {
                        Properties.Settings.Default.ColourLevelDepth8Colour8 = true;
                    }

                    // Protocol
                    Properties.Settings.Default.ProtocolReceived = false;
                    Properties.Settings.Default.Protocol33       = false;
                    Properties.Settings.Default.Protocol37       = false;
                    Properties.Settings.Default.Protocol38       = false;
                    if (dialog.c_radioButtonProtocolReceived.Checked)
                    {
                        Properties.Settings.Default.ProtocolReceived = true;
                    }
                    else if (dialog.c_radioButtonProtocol33.Checked)
                    {
                        Properties.Settings.Default.Protocol33 = true;
                    }
                    else if (dialog.c_radioButtonProtocol37.Checked)
                    {
                        Properties.Settings.Default.Protocol37 = true;
                    }
                    else if (dialog.c_radioButtonProtocol38.Checked)
                    {
                        Properties.Settings.Default.Protocol38 = true;
                    }

                    // Misc
                    Properties.Settings.Default.SendPointer  = dialog.c_checkBoxSendPointer.Checked;
                    Properties.Settings.Default.SendKeyboard = dialog.c_checkBoxSendKeyboard.Checked;
                }
            }
        }
    }
}
