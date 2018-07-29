using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleVncViewer
{
    public partial class SettingDialog : Form
    {
        public SettingDialog()
        {
            InitializeComponent();
        }

        private void c_buttonAddEncoding_Click(object sender, EventArgs e)
        {
            if (c_listBoxEncodingsUnused.SelectedIndex != -1)
            {
                var selectedEncoding = (VncLibrary.VncEnum.EncodeType) c_listBoxEncodingsUnused.Items[c_listBoxEncodingsUnused.SelectedIndex];
                c_listBoxEncodingsUnused.Items.RemoveAt(c_listBoxEncodingsUnused.SelectedIndex);
                c_listBoxEncodingsUsed.Items.Add(selectedEncoding);
            }
        }

        private void c_buttonRemoveEncoding_Click(object sender, EventArgs e)
        {
            if (c_listBoxEncodingsUsed.SelectedIndex != -1)
            {
                var selectedEncoding = (VncLibrary.VncEnum.EncodeType)c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex];
                c_listBoxEncodingsUsed.Items.RemoveAt(c_listBoxEncodingsUsed.SelectedIndex);
                c_listBoxEncodingsUnused.Items.Add(selectedEncoding);
            }
        }

        private void c_buttonUpEncoding_Click(object sender, EventArgs e)
        {
            if (c_listBoxEncodingsUsed.SelectedIndex >= 1)
            {
                var tmp = (VncLibrary.VncEnum.EncodeType)c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex];
                c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex]     = c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex - 1];
                c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex - 1] = tmp;
                --c_listBoxEncodingsUsed.SelectedIndex;
            }
        }

        private void c_buttonDownEncoding_Click(object sender, EventArgs e)
        {
            if (0 <= c_listBoxEncodingsUsed.SelectedIndex && c_listBoxEncodingsUsed.SelectedIndex < c_listBoxEncodingsUsed.Items.Count - 1)
            {
                var tmp = (VncLibrary.VncEnum.EncodeType)c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex];
                c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex]     = c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex + 1];
                c_listBoxEncodingsUsed.Items[c_listBoxEncodingsUsed.SelectedIndex + 1] = tmp;
                ++c_listBoxEncodingsUsed.SelectedIndex;
            }
        }
    }
}
