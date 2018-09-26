using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yukari
{
    public partial class ShortcutControl : UserControl
    {
        private KeyboardShortcut _shortcut;
        private bool recording;
        public KeyboardShortcut Shortcut
        {
            get
            {
                return _shortcut;
            }
            set
            {
                _shortcut = value;
                UpdateText();
            }
        }
        public ShortcutControl(KeyboardShortcut shortcut)
        {
            InitializeComponent();
            this.Shortcut = shortcut;
            KeyboardHook.Instance.KeyDown += this.OnKeyDown;
            UpdateText();
        }

        public void UpdateText()
        {
            this.shortcutTextbox.Text = Shortcut.ToString();
            this.label.Text = Shortcut.Label;
        }

        public void OnKeyDown(object sender, KeyboardHook.KeyDownEventArgs ea)
        {
            if(this.recording)
            {
                this.Shortcut.Key = ea.Key;
                this.Shortcut.Modifiers = ea.ModifierKeys;
                this.UpdateText();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!this.recording)
            {
                this.recording = true;
                KeyboardShortcuts.Instance.Record(this.Shortcut.Id, this.Shortcut.Action);
                this.button1.Text = "Done";
            }
            else
            {
                this.recording = false;
                KeyboardShortcuts.Instance.StopRecording();
                this.button1.Text = "Record";
            }
        }
    }
}
