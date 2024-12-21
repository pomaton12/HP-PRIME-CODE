using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HP_PRIME_CODE.UtilityPrime
{
    public class MySyntaxAnalyzer
    {
        /// <summary>
        /// Analiza el código proporcionado y devuelve una lista de errores sintácticos.
        /// </summary>
        /// <param name="codigo">El código fuente a analizar.</param>
        /// <returns>Lista de errores sintácticos encontrados.</returns>
        public List<SyntaxError> AnalizarCodigo(string codigo)
        {
            var errores = new List<SyntaxError>();

            // Dividir el código en líneas para analizar cada una
            var lineas = codigo.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();

                // Ignorar líneas vacías
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                // Validar la sintaxis de LOCAL
                if (linea.StartsWith("LOCAL", StringComparison.OrdinalIgnoreCase))
                {
                    if (!ValidarLocal(linea))
                    {
                        errores.Add(new SyntaxError
                        {
                            Line = i + 1,
                            Column = 1,
                            Length = linea.Length,
                            Message = "Error de sintaxis en LOCAL: asegúrate de que las variables estén separadas por comas y termine con ';'."
                        });
                    }
                }
                // Validar la sintaxis de PRINT
                else if (linea.StartsWith("PRINT", StringComparison.OrdinalIgnoreCase))
                {
                    if (!ValidarPrint(linea))
                    {
                        errores.Add(new SyntaxError
                        {
                            Line = i + 1,
                            Column = 1,
                            Length = linea.Length,
                            Message = "Error de sintaxis en PRINT: asegúrate de que el comando esté correctamente formado."
                        });
                    }
                }
            }

            return errores;
        }

        /// <summary>
        /// Valida la sintaxis del comando LOCAL.
        /// </summary>
        private bool ValidarLocal(string linea)
        {
            // Expresión regular para validar LOCAL (variables separadas por comas y terminadas con ;)
            var regex = new Regex(@"^LOCAL\s+([a-zA-Z_][a-zA-Z0-9_]*\s*(,\s*[a-zA-Z_][a-zA-Z0-9_]*)*)\s*;$");
            return regex.IsMatch(linea);
        }

        /// <summary>
        /// Valida la sintaxis del comando PRINT.
        /// </summary>
        private bool ValidarPrint(string linea)
        {
            // Expresión regular para validar PRINT (con o sin paréntesis)
            var regex = new Regex(@"^PRINT\s*(\((.*)\))?\s*;$");
            return regex.IsMatch(linea);
        }
    }

    /// <summary>
    /// Representa un error sintáctico en el código.
    /// </summary>
    public class SyntaxError
    {
        public int Line { get; set; }        // Línea del error (1-based)
        public int Column { get; set; }     // Columna del error (1-based)
        public int Length { get; set; }     // Longitud del texto que causó el error
        public string Message { get; set; } // Mensaje descriptivo del error
    }
}
