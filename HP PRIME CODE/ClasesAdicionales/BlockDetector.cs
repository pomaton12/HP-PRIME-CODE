using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HP_PRIME_CODE.ClasesAdicionales
{
    public class BlockDetector
    {
        private readonly TextEditor _textEditor;

        public BlockDetector(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        /// <summary>
        /// Detecta el bloque que contiene el cursor actual.
        /// </summary>
        public (int startLine, int endLine)? GetActiveBlock(TextDocument document)
        {
            var startOffsets = new Stack<int>();
            var openingRegex = new Regex(@"^\s*(BEGIN|IFERR|IF|FOR|WHILE|REPEAT|CASE)\b", RegexOptions.IgnoreCase);
            var closingRegex = new Regex(@"^\s*(END|UNTIL)\b", RegexOptions.IgnoreCase);
            var singleLineCaseRegex = new Regex(@"^\s*CASE\b.*END;", RegexOptions.IgnoreCase);
            var singleLineIFRegex = new Regex(@"^\s*IF\b.*END;", RegexOptions.IgnoreCase);
            int caretLine = _textEditor.TextArea.Caret.Line;

            for (int i = 0; i < document.LineCount; i++)
            {
                var line = document.GetLineByNumber(i + 1);
                var text = document.GetText(line).Trim();

                // Detectar palabras clave de apertura y cierre
                bool lineHasOpeningKeyword = openingRegex.IsMatch(text);
                bool lineHasClosingKeyword = closingRegex.IsMatch(text);
                bool lineIsSingleLineCase = singleLineCaseRegex.IsMatch(text);
                bool lineIsSingleLineIf = singleLineIFRegex.IsMatch(text);

                // Si encontramos una línea con IF y END en la misma línea (como IF ... END;)
                if (lineIsSingleLineIf || lineIsSingleLineCase)
                {
                    // Omitir el procesamiento de este bloque de una sola línea
                    continue;
                }

                // Detectar bloque de apertura (por ejemplo, IF) sin su cierre (END) en la misma línea
                if (lineHasOpeningKeyword && !lineHasClosingKeyword)
                {
                    startOffsets.Push(line.LineNumber);
                }
                else if (lineHasClosingKeyword && !lineHasOpeningKeyword && !lineIsSingleLineCase && !lineIsSingleLineIf)
                {
                    if (startOffsets.Count > 0)
                    {
                        int startLine = startOffsets.Pop();
                        int endLine = line.LineNumber;

                        // Verificar si el cursor está dentro del bloque actual
                        if (caretLine >= startLine && caretLine <= endLine)
                        {
                            return (startLine, endLine); // Bloque activo
                        }
                    }
                }
            }

            return null; // No se encontró un bloque activo
        }
    }
}