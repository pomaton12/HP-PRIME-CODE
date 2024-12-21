using HP_PRIME_CODE.UtilityPrime;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace HP_PRIME_CODE.UtilityAvalonEdit
{
    //para autocompletado
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text, string commandType)
        {
            this.Text = text;
            this.CommandType = commandType;
        }

        public ImageSource Image { get { return null; } }

        public string Text { get; private set; }

        public string CommandType { get; private set; }

        public object Content
        {
            get
            {
                // Usa la clase de utilidad para generar el contenido
                return ContentHelper.GetContent(Text, CommandType);
            }
        }

        public object Description
        {
            get
            {
                var textEditor = new TextEditor
                {
                    IsReadOnly = true,
                    Background = (Brush)Application.Current.Resources["ColorToolTip"],
                    Foreground = (Brush)Application.Current.Resources["PanelHooverTextColor"],
                    BorderThickness = new Thickness(0),
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Margin = new Thickness(10, 0, 10, 0),
                    FontSize = 12,
                    FontFamily = (FontFamily)Application.Current.Resources["Cascadia Mono Light"],
                };

                // Configurar opciones de renderizado de texto
                TextOptions.SetTextFormattingMode(textEditor, TextFormattingMode.Ideal);
                TextOptions.SetTextRenderingMode(textEditor, TextRenderingMode.ClearType);
                TextOptions.SetTextHintingMode(textEditor, TextHintingMode.Fixed);

                // Aplicar la sintaxis resaltada
                LoadSyntaxHighlightingTool(textEditor);

                // Obtener el texto del snippet
                string snippetText = SnippetManager.GetSnippetText(Text, CommandType);

                // Si se encontró texto, asignarlo al TextEditor
                if (!string.IsNullOrEmpty(snippetText))
                {
                    textEditor.Text = snippetText;
                    return textEditor;
                }

                return null; // Si no hay texto, retornar nulo
            }
        }


        public double Priority { get { return CommandType == "function" ? 1 : 2; } }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            // Obtén el texto hasta la posición del cursor
            string textUntilCaret = textArea.Document.GetText(0, completionSegment.EndOffset);

            // Utiliza una expresión regular para obtener la última palabra en textUntilCaret
            Match match = Regex.Match(textUntilCaret, @"\b(\w+)$");
            string lastWord = match.Groups[1].Value;

            // Calcula la posición de inicio de la última palabra
            int start = completionSegment.EndOffset - lastWord.Length;

            // Crea un nuevo segmento que representa la última palabra
            ISegment segment = new TextSegment { StartOffset = start, EndOffset = completionSegment.EndOffset };

            string snippet = SnippetManager.GetSnippet(this.Text);
            //string descripcion = SnippetManager.GetDescripcion(this.Text);

            if (!string.IsNullOrEmpty(snippet))
            {
                textArea.Document.Replace(segment, snippet);
            }
            else
            {
                textArea.Document.Replace(segment, this.Text); // Inserta el texto tal como está si no hay snippet
            }

        }

        // Funciones De Editor
        public void LoadSyntaxHighlightingTool(TextEditor actualEditor)
        {
            double ThemeSelec = Properties.Settings.Default.TemaSettings;

            try
            {
                // Obtén el directorio de la aplicación
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Define el archivo XSHD según el tema seleccionado
                string xshdFilePath = ThemeSelec switch
                {
                    0 => System.IO.Path.Combine(appDirectory, "Estilo", "HPPPL_Light.xshd"), // Tema claro
                    1 => System.IO.Path.Combine(appDirectory, "Estilo", "HPPPL_Dark.xshd"),  // Tema oscuro
                    _ => throw new InvalidOperationException("Tema no soportado.")
                };

                // Verifica si el archivo existe
                if (!File.Exists(xshdFilePath))
                {
                    throw new FileNotFoundException($"No se encontró el archivo de estilo: {xshdFilePath}");
                }

                // Carga el archivo XSHD y aplícalo al editor
                using (Stream s = File.OpenRead(xshdFilePath))
                {
                    using (XmlTextReader reader = new XmlTextReader(s))
                    {
                        actualEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error al cargar el resaltado de sintaxis: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } //Carga color de texto con thema



    }


}
