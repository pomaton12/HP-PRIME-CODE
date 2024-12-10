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
    public class BlockDetectorGeneral
    {
        private readonly TextEditor _textEditor;

        public BlockDetectorGeneral(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        /// <summary>
        /// Detecta todos los bloques en el documento.
        /// </summary>
        public List<(int startLine, int endLine)> GetAllBlocks(TextDocument document)
        {
            var blocks = new List<(int startLine, int endLine)>();
            var startOffsets = new Stack<int>();

            var openingRegex = new Regex(@"^\s*(BEGIN|IFERR|IF|FOR|WHILE|REPEAT|CASE)\b", RegexOptions.IgnoreCase);
            var closingRegex = new Regex(@"^\s*(END|UNTIL)\b", RegexOptions.IgnoreCase);
            var singleLineCaseRegex = new Regex(@"^\s*CASE\b.*END;", RegexOptions.IgnoreCase);
            var singleLineIFRegex = new Regex(@"^\s*IF\b.*END;", RegexOptions.IgnoreCase);

            for (int i = 0; i < document.LineCount; i++)
            {
                var line = document.GetLineByNumber(i + 1);
                var text = document.GetText(line).Trim();

                // Detectar palabras clave de apertura y cierre
                bool lineHasOpeningKeyword = openingRegex.IsMatch(text);
                bool lineHasClosingKeyword = closingRegex.IsMatch(text);
                bool lineIsSingleLineCase = singleLineCaseRegex.IsMatch(text);
                bool lineIsSingleLineIf = singleLineIFRegex.IsMatch(text);

                // Ignorar bloques de una sola línea
                if (lineIsSingleLineIf || lineIsSingleLineCase)
                {
                    continue;
                }

                if (lineHasOpeningKeyword && !lineHasClosingKeyword)
                {
                    // Abrir un nuevo bloque
                    startOffsets.Push(line.LineNumber);
                }
                else if (lineHasClosingKeyword && startOffsets.Count > 0)
                {
                    // Cerrar el bloque más reciente
                    int startLine = startOffsets.Pop();
                    int endLine = line.LineNumber;
                    blocks.Add((startLine, endLine));
                }
            }

            return blocks;
        }
    }
}
