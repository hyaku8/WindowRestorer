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

            this.FormClosing += (object sender, FormClosingEventArgs ea) =>
            {
                Save();
            };
        }

        private void Save()
        {
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                ShortcutControl scc = c as ShortcutControl;
                if (scc != null && scc.Recording)
                {
                    scc.SaveRecording();
                }
            }
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
            int settingValue = (int)(ea.Shortcut.Modifiers | (Keys.KeyCode & ea.Shortcut.Key));
            switch (ea.Shortcut.Id)
            {
                case Settings.SHORTCUT_ENABLEDISABLE:
                    Properties.Settings.Default.SHORTCUT_ENABLEDISABLE = settingValue;
                    break;
                case Settings.SHORTCUT_RESTOREPOSITIONS:
                    Properties.Settings.Default.SHORTCUT_RESTOREPOSITIONS = settingValue;
                    break;
                case Settings.SHORTCUT_SAVEPOSITIONS:
                    Properties.Settings.Default.SHORTCUT_SAVEPOSITION = settingValue;
                    break;
                default:
                    break;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}
