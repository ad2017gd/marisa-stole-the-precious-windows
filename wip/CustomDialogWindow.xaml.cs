using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Media;

namespace wip
{

    /// <summary>
    /// Interaction logic for CustomDialogWindow.xaml
    /// </summary>
    public partial class CustomDialogWindow : Window
    {

        public bool IsClosed { get; private set; } = false;

        protected override void OnClosed(EventArgs e)
        {

            IsClosed = true;
            base.OnClosed(e);
            this.Dispatcher.InvokeShutdown();
        }


        public BitmapImage? DialogIcon { get; set; } = null;
        public Visibility HasIcon { get => DialogIcon != null ? Visibility.Visible : Visibility.Collapsed; }

        public List<string> buttons = new List<string>() { "OK", "Retry", "Cancel" };
        public string LabelText { get; set; } = "";

        public List<DialogStyleButton> Buttons
        {
            get
            {
                return new List<DialogStyleButton>(buttons.ToArray().Select(x =>
                {
                    var button = new DialogStyleButton();
                    button.Text = x;
                    button.Button.Click += (_, _) =>
                    {
                        this.Close();
                    };
                    return button;
                }));
            }
        }

        

        public CustomDialogWindow(string content, string title, List<string> buttons, MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Question:
                    SystemSounds.Question.Play();
                    DialogIcon = App.icons[0];
                    break;
                case MessageBoxIcon.Warning:
                    SystemSounds.Exclamation.Play();
                    DialogIcon = App.icons[1];
                    break;
                case MessageBoxIcon.Hand:
                    SystemSounds.Hand.Play();
                    DialogIcon = App.icons[2];
                    break;
                case MessageBoxIcon.Information:
                    SystemSounds.Asterisk.Play();
                    DialogIcon = App.icons[3];
                    break;
            }

            this.DataContext = this;

            this.LabelText = content;
            this.buttons = buttons;

            
            InitializeComponent();
            this.Title = title;



        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        public CustomDialogWindow(string content, string title) : this(content, title, new List<string>() { "OK" }, MessageBoxIcon.None) { }
        public CustomDialogWindow(string content) : this(content, "", new List<string>() { "OK" }, MessageBoxIcon.None) { }

        public CustomDialogWindow() : this("", "", new List<string>() { "OK" }, MessageBoxIcon.None) { }
    }
}
