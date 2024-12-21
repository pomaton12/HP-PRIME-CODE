using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
namespace HP_PRIME_CODE.UtilityAvalonEdit
{
    // Pintar linea al escribir
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;
        private readonly Canvas _minimap;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor, Canvas minimap)
        {
            _editor = editor;
            _minimap = minimap;
        }

        public KnownLayer Layer => KnownLayer.Selection;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            double ThemeSelected = Properties.Settings.Default.TemaSettings;
            // Determina el color del borde según el tema
            Pen borderPen = ThemeSelected == 0
                ? new Pen(new SolidColorBrush(Color.FromRgb(230, 230, 242)), 1) // Tema claro
                : new Pen(new SolidColorBrush(Color.FromRgb(50, 50, 50)), 1); // Tema oscuro

            borderPen.Freeze(); // Mejora el rendimiento

            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(null, borderPen, new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
            }

            // Dibuja la posición de la línea en el minimapa
            HighlightCurrentLineInMinimap(currentLine.LineNumber - 1);
        }

        private void HighlightCurrentLineInMinimap(int currentLineIndex)
        {
            // Borra el rectángulo rojo anterior del minimapa
            if (_minimap.Children.Count > 0)
            {
                _minimap.Children.RemoveAt(_minimap.Children.Count - 1);
            }

            // Calcula la posición de la línea actual en el minimapa
            string[] lines = _editor.Text.Split('\n');
            double top = (double)currentLineIndex / lines.Length * (_minimap.Height - 37);
            double ThemeSelected = Properties.Settings.Default.TemaSettings;
            // Define el color del rectángulo en el minimapa según el tema
            SolidColorBrush minimapBrush = ThemeSelected == 0
                ? new SolidColorBrush(Color.FromRgb(76, 76, 76)) // Rojo claro para tema claro
                : new SolidColorBrush(Color.FromRgb(172, 173, 171)); // Rojo oscuro para tema oscuro

            // Crea un rectángulo y lo posiciona en la línea actual
            var rect = new Rectangle
            {
                Fill = minimapBrush,
                Width = 20,
                Height = 2,
            };
            Canvas.SetTop(rect, top);
            _minimap.Children.Add(rect);
        }
    }


}
