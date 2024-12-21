using HP_PRIME_CODE.ClasesAdicionales;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace HP_PRIME_CODE.UtilityAvalonEdit
{

    //Dibujar alcance de Bloques
    public class BlockScopeRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _textEditor;
        private readonly BlockDetectorGeneral _blockDetector;
        private readonly Brush _guideBrush;
        private readonly Pen _guidePen;

        public BlockScopeRenderer(TextEditor textEditor)
        {
            _textEditor = textEditor;
            _blockDetector = new BlockDetectorGeneral(textEditor);

            // Configuración de apariencia de las guías
            _guideBrush = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0)); // Verde translúcido
            _guideBrush.Freeze();
            _guidePen = Properties.Settings.Default.TemaSettings == 0
                ? new Pen(new SolidColorBrush(Color.FromRgb(211, 211, 211)), 1) // Azul oscuro para el borde en tema claro
                : new Pen(new SolidColorBrush(Color.FromRgb(64, 64, 64)), 1); // Naranja oscuro para el borde en tema oscuro;

            _guidePen.Freeze();

            // Redibujar la capa cuando cambien las líneas o el texto
            _textEditor.TextArea.TextView.ScrollOffsetChanged += (s, e) => _textEditor.TextArea.TextView.InvalidateLayer(Layer);
            if (_textEditor.Document != null)
            {
                _textEditor.Document.Changed += Document_Changed;
            }
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();

            var document = _textEditor.Document;
            if (document == null) return;

            // Obtener todos los bloques
            var allBlocks = _blockDetector.GetAllBlocks(document);
            if (!allBlocks.Any()) return;

            foreach (var block in allBlocks)
            {
                var (startLine, endLine) = block;

                // Dibujar guías de alcance para cada línea visible dentro del bloque
                foreach (var visualLine in textView.VisualLines)
                {
                    int lineNumber = visualLine.FirstDocumentLine.LineNumber;

                    if (lineNumber > startLine && lineNumber < endLine)
                    {
                        // Calcular la posición X de la guía de indentación
                        int indentationLevel = GetIndentationLevel(document, startLine);
                        double guideX = Math.Round(textView.WideSpaceWidth * indentationLevel) - textView.ScrollOffset.X + 0.5;

                        // Dibujar la guía de alcance del bloque
                        drawingContext.DrawLine(
                            _guidePen,
                            new Point(guideX, visualLine.VisualTop - textView.ScrollOffset.Y),
                            new Point(guideX, visualLine.VisualTop + visualLine.Height - textView.ScrollOffset.Y));
                    }
                }
            }
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            // Invalida la capa para forzar el redibujado después de un cambio
            _textEditor.TextArea.TextView.InvalidateLayer(Layer);
        }

        /// <summary>
        /// Obtiene el nivel de indentación de una línea.
        /// </summary>
        private int GetIndentationLevel(TextDocument document, int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > document.LineCount)
                return 0;

            string line = document.GetText(document.GetLineByNumber(lineNumber));

            // Contar los espacios iniciales y calcular el nivel de indentación
            int spaceCount = line.TakeWhile(char.IsWhiteSpace).Count();
            int indentationSize = _textEditor.Options.IndentationSize; // Generalmente 4
            return spaceCount;
        }

    }

}
