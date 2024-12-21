using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HP_PRIME_CODE.Utility
{
    public class TabFileData
    {
        public string Header { get; set; } // Título de la pestaña
        public string FilePath { get; set; } // Ruta del archivo
        public string Text { get; set; } // Texto actual del editor

        // Propiedades para la posición del cursor
        public int CaretLine { get; set; }
        public int CaretColumn { get; set; }

        // Nueva propiedad: indica si la pestaña fue inicializada
        public bool IsInitialized { get; set; } = false;

        // Indica si el archivo ha sido modificado
        public bool IsModified { get; set; } = false;

        // Nueva propiedad: lista de plegados (posición inicial y final)
        public List<NewFolding> Foldings { get; set; } = new List<NewFolding>(); // Add Foldings property
    }

}