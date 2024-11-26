using System;
using System.Collections.Generic;
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


        // Nuevas propiedades para guardar la posición del cursor
        public int CaretLine { get; set; }
        public int CaretColumn { get; set; }


        // Nueva propiedad para marcar si ya fue inicializado
        public bool IsInitialized { get; set; } = false;

        //
        public bool IsModified { get; set; } = false;
    }

}
