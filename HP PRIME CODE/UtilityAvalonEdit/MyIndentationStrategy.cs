using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HP_PRIME_CODE.UtilityAvalonEdit
{
    //Espacio en condicionales
    public class MyIndentationStrategy : IIndentationStrategy
    {
        private readonly string _indentation; // Define el carácter de indentación

        public MyIndentationStrategy(string indentation = "\t")
        {
            _indentation = indentation;
        }

        public void IndentLine(TextDocument document, DocumentLine line)
        {
            // Texto de la línea actual
            var lineText = document.GetText(line);

            // Verifica si la línea está vacía o solo contiene espacios
            if (string.IsNullOrWhiteSpace(lineText))
                return;

            // Calcula la indentación correcta usando Refactoring
            var lines = document.Lines.Select(l => document.GetText(l)).ToList();
            Refactoring.FormatLines(ref lines, _indentation);

            // Obtiene la indentación calculada para la línea actual
            var targetIndentation = lines[line.LineNumber - 1]
                .TakeWhile(char.IsWhiteSpace)
                .Count();

            // Genera la nueva cadena de indentación
            var newIndentation = new string('\t', targetIndentation);

            // Obtiene la indentación actual de la línea
            var currentIndentation = new string(lineText.TakeWhile(char.IsWhiteSpace).ToArray());

            // Reemplaza la indentación si es diferente
            if (newIndentation != currentIndentation)
            {
                document.Replace(line.Offset, currentIndentation.Length, newIndentation);
            }
        }

        public void IndentLines(TextDocument document, int beginLine, int endLine)
        {
            for (int i = beginLine; i <= endLine; i++)
            {
                IndentLine(document, document.GetLineByNumber(i));
            }
        }
    }

}
