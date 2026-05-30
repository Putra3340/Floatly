using Floatly.Enums;
using Floatly.ViewModels.Windows;
using SharpHook;
using SharpHook.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using Wpf.Ui.Controls;
using Application = System.Windows.Application;

namespace Floatly.Views.Windows
{
    /// <summary>
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>
    public partial class FloatingWindow : Window
    {
        public FloatingWindowViewModel ViewModel { get; }
        private readonly HashSet<KeyCode> _pressedKeys = new();
        private double WorkAreaWidth => SystemParameters.WorkArea.Width;
        private double WorkAreaHeight => SystemParameters.WorkArea.Height;
        private FloatingPosition _currentPosition = FloatingPosition.BottomRight;
        private CancellationTokenSource? _animationCts;
        private bool AutoAnimateOnHover = false;
        private readonly DispatcherTimer _hoverTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        public FloatingWindow(FloatingWindowViewModel vm, SimpleGlobalHook hook)
        {
            ViewModel = vm;
            DataContext = this;
            InitializeComponent();
            hook.KeyPressed += Hook_KeyPressed;
            hook.KeyReleased += Hook_KeyReleased;
            this.Loaded += Window_Loaded;
            Task.Run(() => hook.Run());
        }

        private void Hook_KeyReleased(object? sender, KeyboardHookEventArgs e)
        {
            bool alt =
                _pressedKeys.Contains(KeyCode.VcLeftAlt);

            bool lShift =
                _pressedKeys.Contains(KeyCode.VcLeftShift);
            if(alt && lShift)
            {

                switch (e.Data.KeyCode)
                {
                    case KeyCode.VcLeftControl:
                        AutoAnimateOnHover = !AutoAnimateOnHover;
                        break;
                }
            }

           _pressedKeys.Remove(e.Data.KeyCode);
        }

        private void Hook_KeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            _pressedKeys.Add(e.Data.KeyCode);

            bool alt =
                _pressedKeys.Contains(KeyCode.VcLeftAlt);

            bool lShift =
                _pressedKeys.Contains(KeyCode.VcLeftShift);

            if (!alt || !lShift)
                return;

            switch (e.Data.KeyCode)
            {
                case KeyCode.VcW:
                    if(_currentPosition == FloatingPosition.BottomLeft)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.TopLeft));
                    }
                    else if (_currentPosition == FloatingPosition.BottomRight)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.TopRight));
                    }
                    break;

                case KeyCode.VcA:
                    if (_currentPosition == FloatingPosition.TopRight)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.TopLeft));
                    }
                    else if (_currentPosition == FloatingPosition.BottomRight)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.BottomLeft));
                    }
                    break;

                case KeyCode.VcS:
                    if (_currentPosition == FloatingPosition.TopLeft)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.BottomLeft));
                    }
                    else if (_currentPosition == FloatingPosition.TopRight)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.BottomRight));
                    }
                    break;

                case KeyCode.VcD:
                    if(_currentPosition == FloatingPosition.TopLeft)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.TopRight));
                    }
                    else if (_currentPosition == FloatingPosition.BottomLeft)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => Animate(FloatingPosition.BottomRight));
                    }
                    break;

                case KeyCode.VcUp:
                    // ALT + LSHIFT + UP
                    break;

                case KeyCode.VcDown:
                    // ALT + LSHIFT + DOWN
                    break;

                case KeyCode.VcLeft:
                    // ALT + LSHIFT + LEFT
                    break;

                case KeyCode.VcRight:
                    // ALT + LSHIFT + RIGHT
                    break;

                case KeyCode.VcM:
                    // ALT + LSHIFT + M
                    break;

                case KeyCode.VcC:
                    // ALT + LSHIFT + C
                    break;

                case KeyCode.VcSpace:
                    // ALT + LSHIFT + SPACE
                    break;
            }
        }

        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SeekCommand.Execute(ProgressSlider.Value);
        }

        private async Task Animate(FloatingPosition targetPosition, int durationMs = 300)
        {
            if (targetPosition == _currentPosition)
                return;

            _animationCts?.Cancel();
            _animationCts = new CancellationTokenSource();

            try
            {
                await AnimateInternal(targetPosition, durationMs, _animationCts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
        private async Task AnimateInternal( FloatingPosition targetPosition, int durationMs, CancellationToken token)
        {
            double startX = Left;
            double startY = Top;

            double margin = 20;

            var (targetX, targetY) = targetPosition switch
            {
                FloatingPosition.TopLeft =>
                    (margin, margin),

                FloatingPosition.TopRight =>
                    (WorkAreaWidth - ActualWidth - margin, margin),

                FloatingPosition.BottomLeft =>
                    (margin, WorkAreaHeight - ActualHeight - margin),

                FloatingPosition.BottomRight =>
                    (WorkAreaWidth - ActualWidth - margin,
                     WorkAreaHeight - ActualHeight - margin),

                _ => (Left, Top)
            };
            _currentPosition = targetPosition;

            var sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < durationMs)
            {
                token.ThrowIfCancellationRequested();

                double t = sw.ElapsedMilliseconds / (double)durationMs;
                t = 1 - Math.Pow(1 - t, 3); // EaseOutCubic

                Left = startX + ((targetX - startX) * t);
                Top = startY + ((targetY - startY) * t);

                await Task.Delay(16, token);
            }

            Left = targetX;
            Top = targetY;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hoverTimer.Tick += (_, _) =>
            {
                bool hovering = IsMouseOverWindow();

                if (hovering && AutoAnimateOnHover)
                {
                    Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        await Animate(GetNextPosition());
                    });
                }
            };

            _hoverTimer.Start();
        }
        private bool IsMouseOverWindow()
        {
            System.Drawing.Point screenPos = System.Windows.Forms.Control.MousePosition;
            System.Windows.Point clientPos = PointFromScreen(
                new System.Windows.Point(screenPos.X, screenPos.Y));

            return clientPos.X >= 0 &&
                   clientPos.X < ActualWidth &&
                   clientPos.Y >= 0 &&
                   clientPos.Y < ActualHeight;
        }
        private FloatingPosition GetNextPosition()
        {
            return _currentPosition switch
            {
                FloatingPosition.TopLeft => FloatingPosition.TopRight,
                FloatingPosition.TopRight => FloatingPosition.BottomRight,
                FloatingPosition.BottomRight => FloatingPosition.BottomLeft,
                FloatingPosition.BottomLeft => FloatingPosition.TopLeft,
                _ => FloatingPosition.TopRight
            };
        }
    }
}
