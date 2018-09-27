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
    public partial class Form1 : Form
    {
        private NotifyIcon tray = new NotifyIcon();

        System.Timers.Timer timer = new System.Timers.Timer(60000);

        void LoadSettings()
        {
            Keys modifiers = Properties.Settings.Default.SHORTCUT_SAVEPOSITION != 0 ? ((Keys)Properties.Settings.Default.SHORTCUT_SAVEPOSITION & Keys.Modifiers) : (Keys.Control | Keys.Shift);
            Keys key = Properties.Settings.Default.SHORTCUT_SAVEPOSITION != 0 ? ((Keys)Properties.Settings.Default.SHORTCUT_SAVEPOSITION & Keys.KeyCode) : (Keys.U);
            KeyboardShortcuts.Instance.Shortcuts.Add(new KeyboardShortcut(Settings.SHORTCUT_SAVEPOSITIONS,
               key, modifiers, () =>
               {
                   WindowSetter.Instance.SaveWindowPositions();
               }, "Save positions"));

            modifiers = Properties.Settings.Default.SHORTCUT_RESTOREPOSITIONS != 0 ? ((Keys)Properties.Settings.Default.SHORTCUT_RESTOREPOSITIONS & Keys.Modifiers) : (Keys.Control | Keys.Shift);
            key = Properties.Settings.Default.SHORTCUT_RESTOREPOSITIONS != 0 ? ((Keys)Properties.Settings.Default.SHORTCUT_RESTOREPOSITIONS & Keys.KeyCode) : (Keys.R);
            KeyboardShortcuts.Instance.Shortcuts.Add(new KeyboardShortcut(Settings.SHORTCUT_RESTOREPOSITIONS,
                key, modifiers, () =>
                {
                    WindowSetter.Instance.RestoreWindows();
                }, "Restore positions"));

            modifiers = Properties.Settings.Default.SHORTCUT_ENABLEDISABLE != 0 ? ((Keys)Properties.Settings.Default.SHORTCUT_ENABLEDISABLE & Keys.Modifiers) : (Keys.Control | Keys.Shift);
            key = Properties.Settings.Default.SHORTCUT_ENABLEDISABLE != 0 ? ((Keys)Properties.Settings.Default.SHORTCUT_ENABLEDISABLE & Keys.KeyCode) : (Keys.D);
            KeyboardShortcuts.Instance.Shortcuts.Add(new KeyboardShortcut(Settings.SHORTCUT_ENABLEDISABLE,
               key, modifiers, () =>
               {
                   WindowSetterEnabled = !WindowSetterEnabled;
               }, "Enable/disable"));
        }

        private bool enabled;
        public bool WindowSetterEnabled
        {
            get
            {
                return enabled;
            }
            set
            {
                WindowSetter.Instance.Active = value;
                enabled = value;
                enableDisable.Checked = value;
            }
        }

        private MenuItem enableDisable = null;

        private void InitContextMenu()
        {
            tray.Visible = true;
            tray.Icon = new Icon(SystemIcons.Application, 40, 40);
            tray.ContextMenu = new ContextMenu();

            enableDisable = new MenuItem("Enabled", (Object sender, EventArgs e) =>
            {
                WindowSetter.Instance.Active = !WindowSetter.Instance.Active;
                ((MenuItem)sender).Checked = WindowSetter.Instance.Active;
            });
            enableDisable.Checked = WindowSetter.Instance.Active;
            tray.ContextMenu.MenuItems.Add(enableDisable);

            tray.ContextMenu.MenuItems.Add("-");

            var settings = new MenuItem("Settings", (Object sender, EventArgs e) =>
            {
                var settingsForm = new SettingsForm();
                settingsForm.Show();
            });
            tray.ContextMenu.MenuItems.Add(settings);

            tray.ContextMenu.MenuItems.Add("-");

            var savePositions = new MenuItem("Save window positions", (Object sender, EventArgs e) =>
            {
                WindowSetter.Instance.SaveWindowPositions();
            });
            tray.ContextMenu.MenuItems.Add(savePositions);

            var restorePositions = new MenuItem("Restore window positions", (Object sender, EventArgs e) =>
            {
                WindowSetter.Instance.RestoreWindows();
            });
            tray.ContextMenu.MenuItems.Add(restorePositions);


            var exitMenuItem = new MenuItem("Exit", (Object sender, EventArgs e) =>
            {
                Application.Exit();
            });
            tray.ContextMenu.MenuItems.Add(exitMenuItem);
        }

        public Form1()
        {

            timer.AutoReset = true;
            WindowSetter.Instance.Timer = timer;
            timer.Elapsed += (sender, ea) =>
            {
                WindowSetter.Instance.SaveWindowPositions();
            };
            InitContextMenu();
            WindowSetterEnabled = Properties.Settings.Default.Enabled;
            InitializeComponent();

            LoadSettings();    

            Application.ApplicationExit += (Object sender, EventArgs e) =>
            {
                KeyboardHook.Instance.Deactivate();
            };
        

            this.Load += (Object sender, EventArgs e) =>
            {
                MessageBox.Show("LOAD");
                ((Form)sender).Hide();
            };

        }


    }
}
