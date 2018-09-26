using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yukari
{
    public class KeyboardShortcut
    {
        public delegate void ShortcutAction();

        public string Id { get; set; }
        public string Label { get; set; }

        ~KeyboardShortcut()
        {
            if (KeyboardHook.Instance != null)
                KeyboardHook.Instance.KeyDown -= this.Binding;
        }

        public KeyboardShortcut(string id, Keys key, Keys modifiers,
            ShortcutAction action, bool enabled, string label = null)
        {
            this.Id = id;
            this.Label = label;
            this.key = key;
            this.modifiers = modifiers;
            this.Action = action;
            this.UpdateHandler();
            this.Enabled = enabled;
        }

        public KeyboardShortcut(string id, Keys key, Keys modifiers,
            ShortcutAction action, string label = null)
        {
            this.Id = id;
            this.Label = label;
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

        public override string ToString()
        {
            return this.modifiers.ToString() + " + " + this.key.ToString();
        }
    }
}
