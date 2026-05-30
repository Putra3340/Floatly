using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
using UserControl = System.Windows.Controls.UserControl;

namespace Floatly.Views.Controls
{
    /// <summary>
    /// Interaction logic for SlideMenu.xaml
    /// </summary>
    public partial class SlideMenu : UserControl
    {
        public SlideMenu()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty IsOpenProperty =
       DependencyProperty.Register(
           nameof(IsOpen),
           typeof(bool),
           typeof(SlideMenu),
           new PropertyMetadata(false, OnIsOpenChanged));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty MenuWidthProperty =
            DependencyProperty.Register(
                nameof(MenuWidth),
                typeof(double),
                typeof(SlideMenu),
                new PropertyMetadata(280d));

        public double MenuWidth
        {
            get => (double)GetValue(MenuWidthProperty);
            set => SetValue(MenuWidthProperty, value);
        }

        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register(
                nameof(OverlayOpacity),
                typeof(double),
                typeof(SlideMenu),
                new PropertyMetadata(0.5d));

        public double OverlayOpacity
        {
            get => (double)GetValue(OverlayOpacityProperty);
            set => SetValue(OverlayOpacityProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(SlideMenu));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnIsOpenChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is SlideMenu menu)
            {
                menu.Animate((bool)e.NewValue);
            }
        }

        private void Animate(bool open)
        {
            double to = open ? 0 : MenuWidth;

            var animation = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            MenuTransform.BeginAnimation(
                TranslateTransform.XProperty,
                animation);
        }
    }
    }
