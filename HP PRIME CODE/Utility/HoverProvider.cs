using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class HoverProvider
{
    public class KeywordInfo
    {
        public List<string> Keywords { get; set; }
        public List<string> Info { get; set; }
    }

    public static List<KeywordInfo> KeywordInfos { get; } = new List<KeywordInfo>
    {

        new KeywordInfo
        {
            Keywords = new List<string> { "MAKEMAT"},
            Info = new List<string>
            {
                "MAKEMAT(expresión, filas, columnas)",
                "MAKEMAT(0,3,3)\n\ndevuelve una matriz de cero 3 × 3 → [[0,0,0],[0,0,0],[0,0,0]]",
                "Crea una matriz de filas x columnas de dimensión, utilizando expresiones para calcular cada elemento. Si la expresión contiene las variables I y J, el cálculo de cada elemento sustituye el número de fila actual por I y el número de columna actual por J. También puede crear un vector por el número de elementos (e) en lugar de por el número de filas y columnas."
            }
        },

        new KeywordInfo
        {
            Keywords = new List<string> { "BEGIN", "begin" },
            Info = new List<string>
            {
                "BEGIN comando1; comando2;...; comandoN; END;",
                "Define un comando o el conjunto de comandos que deben ejecutarse juntos. En un programa simple."
            }
        },
        // Agrega más KeywordInfo aquí
    };
}
