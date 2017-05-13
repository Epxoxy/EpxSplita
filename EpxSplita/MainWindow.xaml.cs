using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GuetModel;
using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows.Media.Effects;

namespace EpxSplita
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            OpenLoginBtn.IsChecked = true;
            StateChanged += WindowStateChanged;
        }
        #region DataGrid Event Handler

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        #endregion

        #region DataGrid Tips & Tricks: Single-Click Editing
        /// <summary>
        /// DataGridCell PreviewMouseLeftButtonDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
        #endregion
        
        private string sMaxIcon = "\ue6e7", sRestoreIcon = "\ue712";
        private void WindowStateChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(sender.ToString());
            MaxBtnTB.Text = (sender as Window).WindowState == WindowState.Maximized ? sRestoreIcon : sMaxIcon;
        }

        private MessageShower ms;
        public async void showMessage(string value)
        {
            var dialog = new Sets.Widget.MessageDialog() { Content = value};
            await dialog.ShowAsync();
        }
    }
}
