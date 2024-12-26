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

            // Guarda el estado actual de los plegados
            var oldFoldings = manager.AllFoldings.ToDictionary(f => (f.StartOffset, f.EndOffset), f => f.IsFolded);

            var startOffsets = new Stack<int>();
            var newFoldings = new List<NewFolding>();

            // Expresiones regulares para palabras clave de PPL
            var openingRegex = new Regex(@"^\s*(BEGIN|IFERR|IF|FOR|WHILE|REPEAT|CASE)\b", RegexOptions.IgnoreCase);
            var closingRegex = new Regex(@"^\s*(END|UNTIL)\b", RegexOptions.IgnoreCase);
            var singleLineCaseRegex = new Regex(@"^\s*CASE\b.*END;", RegexOptions.IgnoreCase);
            var singleLineIfRegex = new Regex(@"^\s*IF\b.*END;", RegexOptions.IgnoreCase); // IF ... END en una sola línea

            for (int i = 0; i < document.LineCount; i++)
            {
                var line = document.GetLineByNumber(i + 1);
                var text = document.GetText(line).Trim();

                // Separar comentarios del código
                string codeWithoutComments = text.Split(new[] { "//" }, 2, StringSplitOptions.None)[0].Trim();

                // Detectar palabras clave solo en el código sin comentarios
                bool lineHasOpeningKeyword = openingRegex.IsMatch(codeWithoutComments);
                bool lineHasClosingKeyword = closingRegex.IsMatch(codeWithoutComments);
                bool lineIsSingleLineCase = singleLineCaseRegex.IsMatch(codeWithoutComments);
                bool lineIsSingleLineIf = singleLineIfRegex.IsMatch(codeWithoutComments);

                // Ignorar bloques de una sola línea como IF ... END; o CASE ... END;
                if (lineIsSingleLineIf || lineIsSingleLineCase)
                {
                    // No agregar estos bloques al plegado
                    continue;
                }

                // Bloques de apertura
                if (lineHasOpeningKeyword && !lineHasClosingKeyword)
                {
                    startOffsets.Push(line.EndOffset);
                }
                // Bloques de cierre
                else if (lineHasClosingKeyword && !lineHasOpeningKeyword)
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

            // Actualizar los plegados
            manager.UpdateFoldings(newFoldings, -1);

            // Restaurar el estado de plegado
            foreach (var folding in manager.AllFoldings)
            {
                if (oldFoldings.TryGetValue((folding.StartOffset, folding.EndOffset), out bool isFolded))
                {
                    folding.IsFolded = isFolded;
                }
            }

            // Restaurar la posición del cursor
            _textEditor.CaretOffset = caretOffset;
        }
    }



}
