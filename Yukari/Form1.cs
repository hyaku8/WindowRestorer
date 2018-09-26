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

        public Form1()
        {
            timer.AutoReset = true;
            WindowSetter.Instance.Timer = timer;
            timer.Elapsed += (sender, ea) =>
            {
                WindowSetter.Instance.SaveWindowPositions();
            };
            timer.Start();

            InitializeComponent();

            KeyboardShortcuts.Instance.Shortcuts.Add(new KeyboardShortcut(Settings.SHORTCUT_RESTOREPOSITIONS,
                Keys.R, (Keys.Control | Keys.Shift), () =>
            {
                WindowSetter.Instance.RestoreWindows();
            }, "Restore positions"));
            KeyboardShortcuts.Instance.Shortcuts.Add(new KeyboardShortcut(Settings.SHORTCUT_SAVEPOSITIONS,
                Keys.U, (Keys.Control | Keys.Shift), () =>
            {
                WindowSetter.Instance.SaveWindowPositions();
            }, "Save positions"));
            KeyboardShortcuts.Instance.Shortcuts.Add(new KeyboardShortcut(Settings.SHORTCUT_ENABLEDISABLE,
                Keys.D, (Keys.Control | Keys.Shift), () =>
            {
                WindowSetter.Instance.Timer.Enabled = !WindowSetter.Instance.Timer.Enabled;
            }, "Enable/disable"));


            Application.ApplicationExit += (Object sender, EventArgs e) =>
            {
                KeyboardHook.Instance.Deactivate();
            };
            tray.Visible = true;
            tray.Icon = new Icon(SystemIcons.Application, 40, 40);
            tray.ContextMenu = new ContextMenu();

            var settings = new MenuItem("Settings", (Object sender, EventArgs e) =>
            {
                var settingsForm = new SettingsForm();
                settingsForm.Show();
            });
            tray.ContextMenu.MenuItems.Add(settings);


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

            this.Load += (Object sender, EventArgs e) =>
            {
                MessageBox.Show("LOAD");
                ((Form)sender).Hide();
            };

        }


    }
}
