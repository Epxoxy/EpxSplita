using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace EpxSplita
{
    /// <summary>
    /// Interaction logic for MessageShower.xaml
    /// </summary>
    public partial class MessageShower : UserControl
    {
        public string MessageBoxTitle
        {
            get
            {
                return titletb.Text;
            }
            set
            {
                titletb.Text = value;
            }
        }

        public string MessageBoxText
        {
            get
            {
                return messagetb.Text;
            }
            set
            {
                messagetb.Text = value;
            }
        }

        public MessageShower(string text, string title)
        {
            InitializeComponent();
            DataContext = this;

            MessageBoxText = text;
            MessageBoxTitle = title;
            closeAction = () =>
            {
                if(parent != null)
                {
                    parent.Children.Remove(this);
                    System.Diagnostics.Debug.WriteLine("Message shower closed.");
                }
            };
        }

        public MessageShower(string text) : this(text, "Message") { }

        public MessageShower() :this(string.Empty) { }

        private Panel parent { get; set; }
        public void showIn(Panel panel, string value)
        {
            MessageBoxText = value;

            if (string.IsNullOrEmpty(value)) return;
            if(panel == null) panel = Application.Current.MainWindow.Content as Grid;
            if (panel.Children.Contains(this))  panel.Children.Remove(this);

            Panel.SetZIndex(this, 2);
            panel.Children.Add(this);
            if (panel != parent) parent = panel;
        }

        public void addShow(string value)
        {
            if (closeAction == null)
            {
                Panel panel = Application.Current.MainWindow.Content as Grid;
                showIn(panel, value);
            }
            else
            {
                MessageBoxText += value;
            }
        }

        private void closeMessageShower()
        {
            closeAction?.Invoke();
        }

        private Action closeAction;
        private void Button_Click(object sender, RoutedEventArgs e) => closeMessageShower();
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e) =>closeMessageShower();
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => closeMessageShower();
    }
}
