using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yukari
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            KeyboardShortcuts.Instance.ShortcutRecorded += this.OnShortcutRecorded;

            UpdateControls();
        }

        ~SettingsForm()
        {
            KeyboardShortcuts.Instance.ShortcutRecorded -= this.OnShortcutRecorded;
        }

        private void UpdateControls()
        {
            this.tableLayoutPanel1.Controls.Clear();

            foreach(KeyboardShortcut keyboardShortcut in KeyboardShortcuts.Instance.Shortcuts)
            {
                ShortcutControl scc = new ShortcutControl(keyboardShortcut);
                tableLayoutPanel1.Controls.Add(scc);
            }

            Update();
        }

        private void OnShortcutRecorded(object sender, KeyboardShortcuts.ShortcutRecorderEventArgs ea)
        {
        }

    }
}
