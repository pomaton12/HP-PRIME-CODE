using System;

namespace HP_PRIME_CODE.UtilityPrime
{
    public static class SnippetProvider
    {
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
                switch (text)
                {
                    case "for":
                        return idiomaSeleccionado == 0
                            ? "local i;" + Environment.NewLine + "for i:=1 to 10 do" + Environment.NewLine + "    //Escriba su código aqui..." + Environment.NewLine + "end;"
                            : "local i;" + Environment.NewLine + "for i:=1 to 10 do" + Environment.NewLine + "    //Write your code here..." + Environment.NewLine + "end;";

                    // Agrega más casos según sea necesario...
                    default:
                        return string.Empty; // Si no coincide, devolver texto vacío
                }
            }

            return string.Empty; // Si el tipo de comando no coincide, devolver texto vacío
        }
    }
}
