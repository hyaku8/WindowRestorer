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

        private KeyboardShortcuts.KeyboardShortcut savePositionsShortCut;
        private KeyboardShortcuts.KeyboardShortcut restorePositionsShortCut;
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
            this.savePositionsShortCut = new KeyboardShortcuts.KeyboardShortcut(Keys.R, (Keys.Control | Keys.Shift), () =>
            {
                WindowSetter.Instance.RestoreWindows();
            });
            this.restorePositionsShortCut = new KeyboardShortcuts.KeyboardShortcut(Keys.U, (Keys.Control | Keys.Shift), () =>
            {
                WindowSetter.Instance.SaveWindowPositions();
            });
            KeyboardShortcuts.Instance.Shortcuts.Add(savePositionsShortCut);
            KeyboardShortcuts.Instance.Shortcuts.Add(restorePositionsShortCut);

            Application.ApplicationExit += (Object sender, EventArgs e) =>
            {
                KeyboardHook.Instance.Deactivate();
            };
            tray.Visible = true;
            tray.Icon = new Icon(SystemIcons.Application, 40, 40);
            tray.ContextMenu = new ContextMenu();

            var settings = new MenuItem("Settings", (Object sender, EventArgs e) =>
            {
                
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
