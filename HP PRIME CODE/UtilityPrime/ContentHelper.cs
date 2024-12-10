using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HP_PRIME_CODE.UtilityPrime
{
    public static class ContentHelper
    {
        /// <summary>
        /// Obtiene la descripción y el ícono para un comando dado.
        /// </summary>
        /// <param name="text">Texto del comando.</param>
        /// <param name="commandType">Tipo del comando ("function", "command").</param>
        /// <returns>Un StackPanel que contiene el ícono y las descripciones.</returns>
        public static StackPanel GetContent(string text, string commandType)
        {
            string textIcon = "";
            string textDescription = "";

            // Obtener el idioma configurado: 0 = Español, 1 = Inglés
            int idiomaSeleccionado = (int)Properties.Settings.Default.IdiomaSettings;

            // Determinar el ícono según el tipo de comando
            if (commandType == "function")
            {
                textIcon = "\ue12f";

                // Descripciones en inglés
                if (idiomaSeleccionado == 1)
                {
                    textDescription = text switch
                    {
                        // Bucle
                        "for" => "for (Loop)",
                        "while" => "while (Loop)",
                        "repeat" => "repeat (Loop)",

                        "FOR" => "FOR (Loop)",
                        "WHILE" => "WHILE (Loop)",
                        "REPEAT" => "REPEAT (Loop)",

                        //Vifurcacion
                        "if" => "if (Branch)",
                        "case" => "case (Branch)",
                        "iferr" => "case (Branch)",

                        "IF" => "IF (Branch)",
                        "CASE" => "CASE (Branch)",
                        "IFERR" => "IFERR (Branch)",

                        //Bloque
                        "begin" => "begin end (Block)",
                        "BEGIN" => "BEGIN END (Block)",

                        //Funcion
                        "EXPORT" => "EXPORT Function_Name(Parameters) (Function)",
                        "Function_Name" => "Function_Name(Parameters) (Function)",

                        _ => "function"
                    };
                }
                // Descripciones en español
                else
                {
                    textDescription = text switch
                    {
                        // Bucle
                        "for" => "for (Bucle)",
                        "while" => "while (Bucle)",
                        "repeat" => "repeat (Bucle)",

                        "FOR" => "FOR (Bucle)",
                        "WHILE" => "WHILE (Bucle)",
                        "REPEAT" => "REPEAT (Bucle)",

                        //Vifurcacion
                        "if" => "if (Condicional)",
                        "case" => "case (Condicional)",
                        "iferr" => "iferr (Condicional)",

                        "IF" => "IF (Condicional)",
                        "CASE" => "CASE (Condicional)",
                        "IFERR" => "IFERR (Condicional)",

                        //Bloque
                        "begin" => "begin end (Bloque)",
                        "BEGIN" => "BEGIN END (Bloque)",

                        //Funcion
                        "EXPORT" => "EXPORT Function_Name(Parameters) (Función)",
                        "Function_Name" => "Function_Name(Parameters) (Función)",
                        _ => "función"
                    };
                }
            }
            else if (commandType == "command")
            {
                textIcon = "\ue84a";
            }

            // Crear un contenedor StackPanel
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Fuente para el ícono (asegúrate de que exista "SegoeFluentIcons" en tus recursos)
            FontFamily fuente = (FontFamily)Application.Current.FindResource("SegoeFluentIcons");

            // Crear el TextBlock para el ícono
            TextBlock text0 = new TextBlock
            {
                Text = textIcon,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = fuente,
                FontSize = 16,
                Margin = new Thickness(-15, 0, 5, 0),
                TextAlignment = TextAlignment.Center,
                Width = 30 // Tamaño fijo para el ícono
            };

            // Crear el TextBlock para el texto principal
            TextBlock text1 = new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0), // Espaciado entre ícono y texto
                Width = 110
            };

            // Crear el TextBlock para la descripción
            TextBlock text2 = new TextBlock
            {
                Text = textDescription,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                Width = 115,
                TextAlignment = TextAlignment.Right
            };

            // Agregar los TextBlocks al StackPanel
            stackPanel.Children.Add(text0);
            stackPanel.Children.Add(text1);
            stackPanel.Children.Add(text2);

            return stackPanel; // Retornar el StackPanel como contenido
        }
    }
}
