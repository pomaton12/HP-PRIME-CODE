using SharpVectors.Runtime;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;

namespace HP_PRIME_CODE.Controls
{
    /// <summary>
    /// Lógica de interacción para TitleCustom.xaml
    /// </summary>
    public partial class TitleCustom : UserControl
    {
        private Window _currentWindow;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private Point _initialClickPosition;
        private bool _isRestoringFromMaximized = false;


        public static readonly DependencyProperty HeaderProperty =
    DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(TitleCustom),
        new PropertyMetadata(null));

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public TitleCustom()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += UserControl_Unloaded;
            MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
            MouseMove += TitleBar_MouseMove;
            MouseLeftButtonUp += TitleBar_MouseLeftButtonUp;
            MouseDoubleClick += TitleBar_MouseDoubleClick;

            this.DataContext = this; // Aseguramos que el DataContext esté bien establecido
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _currentWindow = Window.GetWindow(this);

            if (_currentWindow != null)
            {
                // Suscribe al evento de cambio de estado
                _currentWindow.StateChanged += MainWindow_StateChanged;
                // Llama al manejador inmediatamente para actualizar el contenido
                MainWindow_StateChanged(null, null);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.StateChanged -= MainWindow_StateChanged;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _initialClickPosition = e.GetPosition(_currentWindow);

                // Activa el arrastre solo si está maximizado
                if (_currentWindow.WindowState == WindowState.Maximized)
                {
                    _isRestoringFromMaximized = true;
                    _isDragging = false;  // No activa arrastre inmediato
                }
                else
                {
                    _isDragging = true;  // Si no está maximizado, activa el arrastre normal
                    try
                    {
                        _currentWindow.DragMove();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Error al mover la ventana: {ex.Message}");
                    }
                }
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRestoringFromMaximized && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(_currentWindow);

                // Calcula la distancia vertical desde el punto inicial
                var dragDistanceY = Math.Abs(currentPoint.Y - _initialClickPosition.Y);

                // Solo restaura si se ha arrastrado más de 10 píxeles en Y (ajustable)
                if (dragDistanceY > 10)
                {
                    // Restaura la ventana y ajusta para el arrastre
                    _currentWindow.WindowState = WindowState.Normal;
                    _isDragging = true;  // Ahora activa el arrastre

                    // Calcula una posición de inicio para la ventana restaurada
                    var mousePosition = e.GetPosition(_currentWindow);
                    _currentWindow.Top = mousePosition.Y - (_initialClickPosition.Y / 2);
                    _currentWindow.Left = mousePosition.X - (_currentWindow.Width / 2);

                    try
                    {
                        _currentWindow.DragMove();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Error al mover la ventana: {ex.Message}");
                    }

                    // Desactiva la restauración
                    _isRestoringFromMaximized = false;
                }
            }
            else if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    _currentWindow.DragMove();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error al mover la ventana: {ex.Message}");
                }
                _isDragging = false;
            }
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Reinicia los estados cuando se suelta el botón izquierdo
            _isDragging = false;
            _isRestoringFromMaximized = false;
        }

        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Cambia el estado de la ventana entre maximizado y normal
                if (_currentWindow.WindowState == WindowState.Maximized)
                {
                    _currentWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    _currentWindow.WindowState = WindowState.Maximized;
                }
            }
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            _currentWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            _currentWindow.WindowState = _currentWindow.WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (_currentWindow != null)
            {
                if (_currentWindow.WindowState == WindowState.Maximized)
                {
                    MaximizeToolTip.Content = "Restored";
                }
                else
                {
                    MaximizeToolTip.Content = "Maximized";
                }
            }

        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            _currentWindow.Close();
        }
    }



}
