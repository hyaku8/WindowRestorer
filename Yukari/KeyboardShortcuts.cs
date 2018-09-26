using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Yukari
{
    class KeyboardShortcuts
    {

        public List<KeyboardShortcut> Shortcuts { get; private set; }

        private KeyboardShortcuts()
        {
            Shortcuts = new List<KeyboardShortcut>();
            if (!KeyboardHook.Instance.IsActive())
                KeyboardHook.Instance.Activate();
        }

        ~KeyboardShortcuts()
        {
            if (KeyboardHook.Instance.IsActive())
                KeyboardHook.Instance.Deactivate();
        }

        private static readonly KeyboardShortcuts instance = new KeyboardShortcuts();
        public static KeyboardShortcuts Instance
        {
            get
            {
                return instance;
            }
        }

        public class ShortcutRecorderEventArgs : EventArgs
        {
            public KeyboardShortcut Shortcut;
        }
        public delegate void ShortcutRecordedEventHandler(object sender, ShortcutRecorderEventArgs eventArgs);
        public event ShortcutRecordedEventHandler ShortcutRecorded;


        private KeyboardShortcut recordedShortcut;
        private bool[] preserveState;
        public void StopRecording()
        {
            KeyboardShortcut old = this.Shortcuts.FirstOrDefault(x => x.Id == this.recordedShortcut.Id);
            if (old != null)
            {
                this.recordedShortcut.Label = old.Label;
                this.Shortcuts.Remove(old);
                old = null;
            }

            this.ShortcutRecorded(this, new ShortcutRecorderEventArgs()
            {
                Shortcut = recordedShortcut
            });

            for (int i = 0; i < Shortcuts.Count; i++)
            {
                Shortcuts[i].Enabled = preserveState[i];
            }
            this.recordedShortcut.Enabled = true;
            this.Shortcuts.Add(this.recordedShortcut);
            this.recordedShortcut = null;
            this.preserveState = null;
            GC.Collect();
        }


        public void Record(string id, KeyboardShortcut.ShortcutAction action)
        {
            this.preserveState = new bool[Shortcuts.Count];
            for (int i = 0; i < Shortcuts.Count; i++)
            {
                preserveState[i] = Shortcuts[i].Enabled;
                Shortcuts[i].Enabled = false;
            }
            KeyboardHook.KeyDownEventHandler recorder = null;
            recorder = new KeyboardHook.KeyDownEventHandler((object sender, KeyboardHook.KeyDownEventArgs e) =>
            {
                KeyboardHook.Instance.KeyDown -= recorder;
                if(this.recordedShortcut == null)
                {
                    this.recordedShortcut = new KeyboardShortcut(id, e.Key, e.ModifierKeys, action, false);
                }
                else
                {
                    this.recordedShortcut.Key = e.Key;
                    this.recordedShortcut.Modifiers = e.ModifierKeys;
                }
            });
            KeyboardHook.Instance.KeyDown += recorder;
        }
    }

}
