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

        private void UpdateControls()
        {
            this.tableLayoutPanel1.Controls.Clear();

            foreach(KeyboardShortcut keyboardShortcut in KeyboardShortcuts.Instance.Shortcuts)
            {
                ShortcutControl scc = new ShortcutControl(keyboardShortcut);
                scc.ShortcutRecorded += this.OnShortcutRecorded;
                tableLayoutPanel1.Controls.Add(scc);
            }

            Update();
        }

        private void OnShortcutRecorded(object sender, ShortcutControl.ShortcutRecordedEventArgs ea)
        {
            int settingValue = (int)(ea.Shortcut.Modifiers | (Keys.KeyCode & ea.Shortcut.Key));
            KeyboardShortcuts.Instance.AddOrUpdate(ea.Shortcut);
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
            Properties.Settings.Default.Save();

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Save();
            this.Close();
        }
    }
}
