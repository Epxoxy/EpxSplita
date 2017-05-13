using System.Windows;

namespace EpxTools.ControlsExtension
{
    public class PasswordBoxMonitor : DependencyObject
    {
        public static bool GetLoadSaved(DependencyObject obj)
        {
            return (bool)obj.GetValue(LoadSavedProperty);
        }
        public static void SetLoadSaved(DependencyObject obj, bool value)
        {
            obj.SetValue(LoadSavedProperty, value);
        }
        public static readonly DependencyProperty LoadSavedProperty =
            DependencyProperty.RegisterAttached("LoadSaved", typeof(bool), typeof(PasswordBoxMonitor), new PropertyMetadata(false, OnLoadSavedChanged));

        public static string GetLoadKey(DependencyObject obj)
        {
            return (string)obj.GetValue(LoadKeyProperty);
        }
        public static void SetLoadKey(DependencyObject obj, string value)
        {
            obj.SetValue(LoadKeyProperty, value);
        }
        public static readonly DependencyProperty LoadKeyProperty =
            DependencyProperty.RegisterAttached("LoadKey", typeof(string), typeof(PasswordBoxMonitor), new PropertyMetadata(null));
        
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }
        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }
        public static readonly DependencyProperty IsMonitoringProperty = 
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }
        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }
        public static readonly DependencyProperty PasswordLengthProperty = 
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as System.Windows.Controls.PasswordBox;
            if (pb == null) { return; }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }
        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as System.Windows.Controls.PasswordBox;
            if (pb == null) { return; }
            SetPasswordLength(pb, pb.Password.Length);
        }

        private static void OnLoadSavedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as System.Windows.Controls.PasswordBox;
            if (pb == null) { return; }
            if ((bool)e.NewValue)
            {
                string loadKey = GetLoadKey(pb);
                if (string.IsNullOrEmpty(loadKey)) return;
                string decryptData = "";
                UserSerives.StoreUserData.decryptUserData(loadKey, ref decryptData);
                if (string.IsNullOrEmpty(decryptData))
                {
                    pb.Password = "";
                }
                else
                {
                    pb.Password = decryptData;
                    System.Diagnostics.Debug.WriteLine("Get password sucessful, " + decryptData);
                }
            }
            else
            {
                pb.Password = "";
            }
            System.Diagnostics.Debug.WriteLine("LoadSave changed : " + (bool)e.NewValue);
        }
    }
}
