using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HP_PRIME_CODE.UtilityPrime
{
    public static class SnippetManager
    {
        /// <summary>
        /// Idioma seleccionado (0 = Español, 1 = Inglés).
        /// </summary>
        public static int IdiomaSeleccionado { get; set; } = (int)Properties.Settings.Default.IdiomaSettings;

        /// <summary>
        /// Diccionario para almacenar los snippets cargados del archivo CSV.
        /// </summary>
        private static Dictionary<string, (string DescripcionEs, string DescripcionEn, string PlantillaEs, string PlantillaEn)> Snippets;


        public static void CargarSnippets()
        {
            // Ruta al archivo CSV
            string snippetFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "Snippets.csv");

            // Verificar si el archivo existe
            if (!File.Exists(snippetFilePath))
            {
                throw new FileNotFoundException($"No se encontró el archivo de snippets en {snippetFilePath}");
            }

            Snippets = new Dictionary<string, (string DescripcionEs, string DescripcionEn, string PlantillaEs, string PlantillaEn)>();

            // Usar CsvHelper para leer el archivo CSV
            using (var reader = new StreamReader(snippetFilePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";", // Coma como delimitador estándar para CSV
                Quote = '"',     // Asegura que los campos con saltos de línea o comas estén entre comillas
                HasHeaderRecord = true, // El archivo tiene cabecera
                HeaderValidated = null, // Ignorar la validación de cabeceras
                MissingFieldFound = null // Desactivar advertencias de campos faltantes
            }))
            {
                // Leer todos los registros dinámicos
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    string palabraClave = record.PalabraClave?.ToString().Trim();
                    string descripcionEs = record.DescripcionEs?.ToString().Trim();
                    string descripcionEn = record.DescripcionEn?.ToString().Trim();
                    string plantillaEs = record.PlantillaEs?.ToString().Trim().Replace("\\n", Environment.NewLine);
                    string plantillaEn = record.PlantillaEn?.ToString().Trim().Replace("\\n", Environment.NewLine);

                    if (!string.IsNullOrEmpty(palabraClave))
                    {
                        Snippets[palabraClave] = (descripcionEs, descripcionEn, plantillaEs, plantillaEn);
                    }
                }
            }
        }
        /// <summary>
        /// Obtiene el snippet correspondiente basado en el texto y el tipo de comando.
        /// </summary>
        /// <param name="text">Texto del comando.</param>
        /// <param name="commandType">Tipo de comando.</param>
        /// <returns>Snippet en el idioma especificado.</returns>
        public static string GetSnippet(string text)
        {
            if (Snippets == null)
            {
                CargarSnippets(); // Cargar los snippets si no están en memoria
            }

            if (Snippets.TryGetValue(text, out var snippetData))
            {
                // Obtener la plantilla en el idioma seleccionado
                string plantilla = IdiomaSeleccionado == 1 ? snippetData.PlantillaEn : snippetData.PlantillaEs;

                // Aplicar indentación
                string indentacion = "    "; // 4 espacios para la indentación
                return IndentarCodigo(plantilla, indentacion);
            }

            return string.Empty; // Retorna vacío si no se encuentra el snippet
        }

        /// <summary>
        /// Obtiene la descripción del comando en el idioma seleccionado.
        /// </summary>
        /// <param name="text">Texto del comando.</param>
        /// <returns>Descripción en el idioma especificado.</returns>
        public static string GetDescripcion(string text)
        {
            if (Snippets == null)
            {
                CargarSnippets(); // Cargar snippets si no están en memoria
            }

            if (Snippets.TryGetValue(text, out var snippetData))
            {
                // Retorna la descripción en el idioma seleccionado
                return IdiomaSeleccionado == 1 ? snippetData.DescripcionEn : snippetData.DescripcionEs;
            }

            return string.Empty; // Retorna vacío si no encuentra la descripción
        }


        /// <summary>
        /// Obtiene el texto correspondiente basado en el comando y el tipo.
        /// </summary>
        /// <param name="text">El texto del comando.</param>
        /// <param name="commandType">El tipo de comando.</param>
        /// <returns>El texto correspondiente.</returns>
        public static string GetSnippetText(string text, string commandType)
        {
            int idiomaSeleccionado = (int)Properties.Settings.Default.IdiomaSettings; // 0 = Español, 1 = Inglés


            if (commandType == "function")
            {
                if (Snippets == null)
                {
                    CargarSnippets(); // Cargar snippets si no están en memoria
                }

                if (Snippets.TryGetValue(text, out var snippetData))
                {
                    // Retorna la descripción en el idioma seleccionado
                    return IdiomaSeleccionado == 1 ? snippetData.PlantillaEn : snippetData.PlantillaEs;
                }

                return string.Empty; // Retorna vacío si no encuentra la descripción
            }

            return string.Empty; // Si el tipo de comando no coincide, devolver texto vacío
        }


        /// <summary>
        /// Necesario para identar correctamente, resetea texto formato
        /// </summary>
        public static string IndentarCodigo(string codigo, string indentacion)
        {
            // Normalizar saltos de línea a '\n'
            codigo = codigo.Replace("\r\n", "\n").Replace("\r", "\n");

            // Dividir el código en líneas
            var lineas = codigo.Split(new[] { '\n' }, StringSplitOptions.None);

            // Construir el resultado indentado
            var resultado = new StringBuilder();
            foreach (var linea in lineas)
            {
                // Agregar indentación y reconstruir las líneas
                resultado.AppendLine(indentacion + linea.TrimStart());
            }

            return resultado.ToString();
        }


    }
}
