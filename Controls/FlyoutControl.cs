using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Controls;assembly=Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class FlyoutControl : ContentControl
    {
        static FlyoutControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutControl), new FrameworkPropertyMetadata(typeof(FlyoutControl)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(FlyoutControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public Orientation OpenOrientation
        {
            get { return (Orientation)GetValue(OpenOrientationProperty); }
            set { SetValue(OpenOrientationProperty, value); }
        }

        public double FadeInFrom
        {
            get { return (double)GetValue(FadeInFromProperty); }
            set { SetValue(FadeInFromProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlyoutControl), new PropertyMetadata(false, OnIsOpenChanged));
        public static readonly DependencyProperty OpenOrientationProperty =
                    DependencyProperty.Register("OpenOrientation", typeof(Orientation), typeof(FlyoutControl), new PropertyMetadata(Orientation.Horizontal, OnOpenOrientationChanged));
        public static readonly DependencyProperty FadeInFromProperty =
            DependencyProperty.Register("FadeInFrom", typeof(double), typeof(FlyoutControl), new PropertyMetadata(0d, OnFadeInFromChanged));

        private static void OnOpenOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlyoutControl;
            if (control != null) control.UpdateAnimation();
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlyoutControl;
            if (control != null) control.UpdateOpenState();
        }

        private static void OnFadeInFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlyoutControl;
            if (control != null) control.UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            double toXValue = 0, toYValue = 0;
            if (OpenOrientation == Orientation.Horizontal)
            {
                toXValue = FadeInFrom;
            }
            else
            {
                toYValue = FadeInFrom;
            }
            if (targetInX != null) targetInX.Value = toXValue;
            if (targetOutX != null) targetOutX.Value = toXValue;
            if (targetInY != null) targetInY.Value = toYValue;
            if (targetOutY != null) targetOutY.Value = toYValue;
        }

        private void UpdateOpenState(bool useTransition = true)
        {
            if (IsOpen)
            {
                VisualStateManager.GoToState(this, "Open", useTransition);
                Keyboard.Focus(this);
            }
            else
            {
                Keyboard.ClearFocus();
                VisualStateManager.GoToState(this, "Close", useTransition);
            }
        }

        private void OnDissmissLayerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOpen) IsOpen = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var rect = GetTemplateChild("DismissLayer") as FrameworkElement;
            targetInX = GetTemplateChild("TargetInX") as DoubleKeyFrame;
            targetOutX = GetTemplateChild("TargetOutX") as DoubleKeyFrame;
            targetInY = GetTemplateChild("TargetInY") as DoubleKeyFrame;
            targetOutY = GetTemplateChild("TargetOutY") as DoubleKeyFrame;
            if (rect != null)
            {
                rect.MouseDown -= OnDissmissLayerMouseDown;
                rect.MouseDown += OnDissmissLayerMouseDown;
            }
            UpdateOpenState(false);
            UpdateAnimation();
        }

        private DoubleKeyFrame targetInX;
        private DoubleKeyFrame targetInY;
        private DoubleKeyFrame targetOutX;
        private DoubleKeyFrame targetOutY;
    }
}
