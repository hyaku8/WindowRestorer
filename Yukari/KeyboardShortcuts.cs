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
        public class KeyboardShortcut
        {
            public delegate void ShortcutAction();

            ~KeyboardShortcut()
            {
                if (KeyboardHook.Instance != null)
                    KeyboardHook.Instance.KeyDown -= this.Binding;
            }

            public KeyboardShortcut(Keys key, Keys modifiers, ShortcutAction action, bool enabled)
            {
                this.key = key;
                this.modifiers = modifiers;
                this.Action = action;
                this.UpdateHandler();
                this.Enabled = enabled;
            }

            public KeyboardShortcut(Keys key, Keys modifiers, ShortcutAction action)
            {
                this.key = key;
                this.modifiers = modifiers;
                this.Action = action;
                this.UpdateHandler();
                this.Enabled = true;
            }

            public ShortcutAction Action { get; set; }
            private Keys key;
            public Keys Key
            {
                get
                {
                    return key;
                }
                set
                {
                    this.key = value;
                    UpdateHandler();
                }
            }
            private Keys modifiers;
            public Keys Modifiers
            {
                get
                {
                    return modifiers;
                }
                set
                {
                    this.modifiers = value;
                    UpdateHandler();
                }
            }

            public bool Enabled { get; set; }

            public KeyboardHook.KeyDownEventHandler Binding { get; private set; }

            public void UpdateHandler()
            {
                if (this.Binding != null)
                {
                    try
                    {
                        KeyboardHook.Instance.KeyDown -= this.Binding;
                    }
                    catch { }
                }
                this.Binding = new KeyboardHook.KeyDownEventHandler((object sender, KeyboardHook.KeyDownEventArgs eventArgs) =>
                {
                    if (this.Enabled && this.Action != null && eventArgs.Key == this.key && eventArgs.ModifierKeys == this.modifiers)
                    {
                        this.Action();
                    }
                });
                KeyboardHook.Instance.KeyDown += this.Binding;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

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

        public void Record(KeyboardShortcut.ShortcutAction action)
        {
            bool[] preserveState = new bool[Shortcuts.Count];
            for (int i = 0; i < Shortcuts.Count; i++)
            {
                preserveState[i] = Shortcuts[i].Enabled;
                Shortcuts[i].Enabled = false;
            }
            KeyboardHook.KeyDownEventHandler recorder = null;
            recorder = new KeyboardHook.KeyDownEventHandler((object sender, KeyboardHook.KeyDownEventArgs e) =>
            {
                KeyboardShortcut shortcut = new KeyboardShortcut(e.Key, e.ModifierKeys, action);
                this.ShortcutRecorded(this, new ShortcutRecorderEventArgs()
                {
                    Shortcut = shortcut
                });
                KeyboardHook.Instance.KeyDown -= recorder;
            });
            KeyboardHook.Instance.KeyDown += recorder;

            for (int i = 0; i < Shortcuts.Count; i++)
            {
                Shortcuts[i].Enabled = preserveState[i];
            }
        }
    }

}
