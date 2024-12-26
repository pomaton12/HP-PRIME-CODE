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
        private readonly string _indentation;

        public MyIndentationStrategy(string indentation = "\t")
        {
            _indentation = indentation;
        }

        public void IndentLine(TextDocument document, DocumentLine line)
        {
            string lineText = document.GetText(line);
            if (string.IsNullOrWhiteSpace(lineText)) return;

            // Obtener la indentación de la línea actual
            int currentIndentation = lineText.TakeWhile(char.IsWhiteSpace).Count();

            // Determinar la indentación correcta en base a la línea anterior (o por defecto a 0)
            int newIndentation = 0;
            DocumentLine prevLine = document.GetLineByNumber(line.LineNumber - 1);
            if (prevLine != null)
            {
                string prevLineText = document.GetText(prevLine);
                newIndentation = prevLineText.TakeWhile(char.IsWhiteSpace).Count() + _indentation.Length; // Agregar indentación en base a la línea anterior
                                                                                                          //Agregar lógica aquí para manejar corchetes de apertura/cierre y otros casos
            }

            // Generar la nueva cadena de indentación
            string newIndentationString = new string(' ', newIndentation);

            // Reemplazar la indentación si es diferente
            if (newIndentationString.Length != currentIndentation)
            {
                document.Replace(line.Offset, currentIndentation, newIndentationString);
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
