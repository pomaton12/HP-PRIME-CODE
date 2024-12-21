using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HP_PRIME_CODE.UtilityAvalonEdit
{

    // Para Replegar
    // Para Replegar
    public class MyFoldingStrategy
    {
        private TextEditor _textEditor;

        public MyFoldingStrategy(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            // Guarda la posición del cursor
            int caretOffset = _textEditor.CaretOffset;

            var startOffsets = new Stack<int>();
            var newFoldings = new List<NewFolding>();

            // Expresiones regulares para palabras clave de PPL
            var openingRegex = new Regex(@"^\s*(BEGIN|IFERR|IF|FOR|WHILE|REPEAT|CASE)\b", RegexOptions.IgnoreCase);
            var closingRegex = new Regex(@"^\s*(END|UNTIL)\b", RegexOptions.IgnoreCase);
            var singleLineCaseRegex = new Regex(@"^\s*CASE\b.*END;", RegexOptions.IgnoreCase);
            var singleLineIfRegex = new Regex(@"^\s*IF\b.*END;", RegexOptions.IgnoreCase); // Expresión regular para IF ... END en la misma línea

            for (int i = 0; i < document.LineCount; i++)
            {
                var line = document.GetLineByNumber(i + 1);
                var text = document.GetText(line).Trim();

                // Detectar palabras clave de apertura y cierre
                bool lineHasOpeningKeyword = openingRegex.IsMatch(text);
                bool lineHasClosingKeyword = closingRegex.IsMatch(text);
                bool lineIsSingleLineCase = singleLineCaseRegex.IsMatch(text);
                bool lineIsSingleLineIf = singleLineIfRegex.IsMatch(text); // Verificar si la línea es un IF ... END de una sola línea

                // Si encontramos una línea con IF y END en la misma línea (como IF ... END;)
                if (lineIsSingleLineIf || lineIsSingleLineCase)
                {
                    // Omitir el procesamiento de este bloque de una sola línea
                    continue;
                }

                // Detectar bloque de apertura (por ejemplo, IF) sin su cierre (END) en la misma línea
                if (lineHasOpeningKeyword && !lineHasClosingKeyword)
                {
                    startOffsets.Push(line.EndOffset); // Guarda el offset del bloque de inicio
                }
                else if (lineHasClosingKeyword && !lineHasOpeningKeyword && !lineIsSingleLineCase && !lineIsSingleLineIf)
                {
                    if (startOffsets.Count > 0)
                    {
                        int startOffset = startOffsets.Pop();
                        newFoldings.Add(new NewFolding(startOffset, line.EndOffset));
                    }
                }
            }

            // Manejo de bloques no cerrados (opcional)
            while (startOffsets.Count > 0)
            {

                int startOffset = startOffsets.Pop();
                newFoldings.Add(new NewFolding(startOffset, document.TextLength)); // Hasta el final del documento
            }


            // Ordenar los plegados por posición
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));

            // Actualizar el FoldingManager
            manager.UpdateFoldings(newFoldings, -1);

            // Restaurar la posición del cursor
            _textEditor.CaretOffset = caretOffset;
        }
    }
}
