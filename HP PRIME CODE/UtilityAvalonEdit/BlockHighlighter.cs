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
    //Pintado de bloque rojo
    public class BlockHighlighter : IBackgroundRenderer
    {
        private readonly TextEditor _textEditor;
        private readonly BlockDetector _blockDetector;
        private Brush _highlightBrush;

        public BlockHighlighter(TextEditor textEditor)
        {
            _textEditor = textEditor;
            _blockDetector = new BlockDetector(textEditor);
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();

            var document = _textEditor.Document;
            if (document == null) return;

            var activeBlock = _blockDetector.GetActiveBlock(document);
            if (activeBlock == null) return;

            var (startLine, endLine) = activeBlock.Value;

            // Verificar si el bloque está en una sola línea (IF ... END en una misma línea)
            if (startLine == endLine)
            {
                // Omitir el resaltado para este bloque, pero no detener el resto del proceso
                //continue; 
            }

            // Obtener la indentación del bloque
            int blockIndentation = GetIndentationLevel(document, startLine);

            // Dibujar el fondo para las líneas dentro del bloque activo
            foreach (var visualLine in textView.VisualLines)
            {
                var lineNumber = visualLine.FirstDocumentLine.LineNumber;

                // Si la línea está dentro del bloque activo y no es la línea de apertura ni de cierre
                if (lineNumber > startLine && lineNumber < endLine)
                {
                    // Calcular la posición X de la guía de indentación solo para las líneas dentro del bloque
                    double guideX = Math.Round(textView.WideSpaceWidth * blockIndentation * _textEditor.Options.IndentationSize) - textView.ScrollOffset.X + 0.5;

                    // Dibujar la guía de indentación activa
                    drawingContext.DrawLine(new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80FF5555")), 1),
                        new Point(guideX, visualLine.VisualTop - textView.ScrollOffset.Y),
                        new Point(guideX, visualLine.VisualTop + visualLine.Height - textView.ScrollOffset.Y));
                }
            }
        }

        // Función auxiliar para obtener el nivel de indentación de una línea
        private int GetIndentationLevel(TextDocument document, int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > document.LineCount)
                return 0;

            string line = document.GetText(document.GetLineByNumber(lineNumber));
            return line.TakeWhile(char.IsWhiteSpace).Count() / _textEditor.Options.IndentationSize;
        }


    }


}
