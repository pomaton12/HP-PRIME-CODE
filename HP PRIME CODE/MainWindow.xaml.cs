using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using Newtonsoft.Json;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System.IO;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using HP_PRIME_CODE.Controls;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using SharpVectors.Converters;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wpf.Ui.Input;
using System.Xml.Linq;
using Wpf.Ui.Appearance;

using HP_PRIME_CODE.Utility;
using HP_PRIME_CODE.UtilityPrime;
using HP_PRIME_CODE.ClasesAdicionales;
using System;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using MenuItem = System.Windows.Controls.MenuItem;
using ICSharpCode.AvalonEdit.Utils;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using TextBlock = System.Windows.Controls.TextBlock;
using System.Diagnostics.Eventing.Reader;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;




namespace HP_PRIME_CODE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

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

    // Para Replegar
    public class MyFoldingStrategy
    {
        private TextEditor _textEditor;

        public MyFoldingStrategy(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            // Guarda la posición del cursor
            int caretOffset = _textEditor.CaretOffset;

            var startOffsets = new Stack<int>();
            var newFoldings = new List<NewFolding>();

            // Expresiones regulares para palabras clave de PPL
            var openingRegex = new Regex(@"^\s*(BEGIN|IFERR|IF|FOR|WHILE|REPEAT|CASE)\b", RegexOptions.IgnoreCase);
            var closingRegex = new Regex(@"^\s*(END|UNTIL)\b", RegexOptions.IgnoreCase);
            var singleLineCaseRegex = new Regex(@"^\s*CASE\b.*END;", RegexOptions.IgnoreCase);
            var singleLineIfRegex = new Regex(@"^\s*IF\b.*END;", RegexOptions.IgnoreCase); // Expresión regular para IF ... END en la misma línea

            for (int i = 0; i < document.LineCount; i++)
            {
                var line = document.GetLineByNumber(i + 1);
                var text = document.GetText(line).Trim();

                // Detectar palabras clave de apertura y cierre
                bool lineHasOpeningKeyword = openingRegex.IsMatch(text);
                bool lineHasClosingKeyword = closingRegex.IsMatch(text);
                bool lineIsSingleLineCase = singleLineCaseRegex.IsMatch(text);
                bool lineIsSingleLineIf = singleLineIfRegex.IsMatch(text); // Verificar si la línea es un IF ... END de una sola línea

                // Si encontramos una línea con IF y END en la misma línea (como IF ... END;)
                if (lineIsSingleLineIf || lineIsSingleLineCase)
                {
                    // Omitir el procesamiento de este bloque de una sola línea
                    continue;
                }

                // Detectar bloque de apertura (por ejemplo, IF) sin su cierre (END) en la misma línea
                if (lineHasOpeningKeyword && !lineHasClosingKeyword)
                {
                    startOffsets.Push(line.EndOffset); // Guarda el offset del bloque de inicio
                }
                else if (lineHasClosingKeyword && !lineHasOpeningKeyword && !lineIsSingleLineCase && !lineIsSingleLineIf)
                {
                    if (startOffsets.Count > 0)
                    {
                        int startOffset = startOffsets.Pop();
                        newFoldings.Add(new NewFolding(startOffset, line.EndOffset));
                    }
                }     
            }   

            // Manejo de bloques no cerrados (opcional)
            while (startOffsets.Count > 0)
            {
          
                int startOffset = startOffsets.Pop();   
                newFoldings.Add(new NewFolding(startOffset, document.TextLength)); // Hasta el final del documento
            }
            

            // Ordenar los plegados por posición
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));

            // Actualizar el FoldingManager
            manager.UpdateFoldings(newFoldings, -1);

            // Restaurar la posición del cursor
            _textEditor.CaretOffset = caretOffset;
        }
    }

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


        public double Priority { get {  return CommandType == "function" ? 1 : 2;  } }

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




    // Pintar linea al escribir
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;
        private readonly Canvas _minimap;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor, Canvas minimap)
        {
            _editor = editor;
            _minimap = minimap;
        }

        public KnownLayer Layer => KnownLayer.Selection;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            double ThemeSelected = Properties.Settings.Default.TemaSettings;
            // Determina el color del borde según el tema
            Pen borderPen = ThemeSelected == 0
                ? new Pen(new SolidColorBrush(Color.FromRgb(230, 230, 242)), 1) // Tema claro
                : new Pen(new SolidColorBrush(Color.FromRgb(50, 50, 50)), 1); // Tema oscuro

            borderPen.Freeze(); // Mejora el rendimiento

            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(null, borderPen, new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
            }

            // Dibuja la posición de la línea en el minimapa
            HighlightCurrentLineInMinimap(currentLine.LineNumber - 1);
        }

        private void HighlightCurrentLineInMinimap(int currentLineIndex)
        {
            // Borra el rectángulo rojo anterior del minimapa
            if (_minimap.Children.Count > 0)
            {
                _minimap.Children.RemoveAt(_minimap.Children.Count - 1);
            }

            // Calcula la posición de la línea actual en el minimapa
            string[] lines = _editor.Text.Split('\n');
            double top = (double)currentLineIndex / lines.Length * (_minimap.Height-37);
            double ThemeSelected = Properties.Settings.Default.TemaSettings;
            // Define el color del rectángulo en el minimapa según el tema
            SolidColorBrush minimapBrush = ThemeSelected == 0
                ? new SolidColorBrush(Color.FromRgb(255, 0, 0)) // Rojo claro para tema claro
                : new SolidColorBrush(Color.FromRgb(255, 85, 85)); // Rojo oscuro para tema oscuro

            // Crea un rectángulo y lo posiciona en la línea actual
            var rect = new Rectangle
            {
                Fill = minimapBrush,
                Width = 20,
                Height = 2,
            };
            Canvas.SetTop(rect, top);
            _minimap.Children.Add(rect);
        }
    }


    //Dibujar alcance de Bloques
    public class BlockScopeRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _textEditor;
        private readonly BlockDetectorGeneral _blockDetector;
        private readonly Brush _guideBrush;
        private readonly Pen _guidePen;

        public BlockScopeRenderer(TextEditor textEditor)
        {
            _textEditor = textEditor;
            _blockDetector = new BlockDetectorGeneral(textEditor);

            // Configuración de apariencia de las guías
            _guideBrush = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0)); // Verde translúcido
            _guideBrush.Freeze();
            _guidePen = Properties.Settings.Default.TemaSettings == 0
                ? new Pen(new SolidColorBrush(Color.FromRgb(211, 211, 211)), 1) // Azul oscuro para el borde en tema claro
                : new Pen(new SolidColorBrush(Color.FromRgb(64, 64, 64)), 1); // Naranja oscuro para el borde en tema oscuro;

            _guidePen.Freeze();

            // Redibujar la capa cuando cambien las líneas o el texto
            _textEditor.TextArea.TextView.ScrollOffsetChanged += (s, e) => _textEditor.TextArea.TextView.InvalidateLayer(Layer);
            if (_textEditor.Document != null)
            {
                _textEditor.Document.Changed += Document_Changed;
            }
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();

            var document = _textEditor.Document;
            if (document == null) return;

            // Obtener todos los bloques
            var allBlocks = _blockDetector.GetAllBlocks(document);
            if (!allBlocks.Any()) return;

            foreach (var block in allBlocks)
            {
                var (startLine, endLine) = block;

                // Dibujar guías de alcance para cada línea visible dentro del bloque
                foreach (var visualLine in textView.VisualLines)
                {
                    int lineNumber = visualLine.FirstDocumentLine.LineNumber;

                    if (lineNumber > startLine && lineNumber < endLine)
                    {
                        // Calcular la posición X de la guía de indentación
                        int indentationLevel = GetIndentationLevel(document, startLine);
                        double guideX = Math.Round(textView.WideSpaceWidth * indentationLevel) - textView.ScrollOffset.X + 0.5;

                        // Dibujar la guía de alcance del bloque
                        drawingContext.DrawLine(
                            _guidePen,
                            new Point(guideX, visualLine.VisualTop - textView.ScrollOffset.Y),
                            new Point(guideX, visualLine.VisualTop + visualLine.Height - textView.ScrollOffset.Y));
                    }
                }
            }
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            // Invalida la capa para forzar el redibujado después de un cambio
            _textEditor.TextArea.TextView.InvalidateLayer(Layer);
        }

        /// <summary>
        /// Obtiene el nivel de indentación de una línea.
        /// </summary>
        private int GetIndentationLevel(TextDocument document, int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > document.LineCount)
                return 0;

            string line = document.GetText(document.GetLineByNumber(lineNumber));

            // Contar los espacios iniciales y calcular el nivel de indentación
            int spaceCount = line.TakeWhile(char.IsWhiteSpace).Count();
            int indentationSize = _textEditor.Options.IndentationSize; // Generalmente 4
            return spaceCount;
        }

    }


    //Pintado de bloque rojo
    public class BlockHighlighter : IBackgroundRenderer
    {
        private readonly TextEditor _textEditor;
        private readonly BlockDetector _blockDetector;
        private Brush _highlightBrush;

        public BlockHighlighter(TextEditor textEditor)
        {
            _textEditor = textEditor;
            _blockDetector = new BlockDetector(textEditor);
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();

            var document = _textEditor.Document;
            if (document == null) return;

            var activeBlock = _blockDetector.GetActiveBlock(document);
            if (activeBlock == null) return;

            var (startLine, endLine) = activeBlock.Value;

            // Verificar si el bloque está en una sola línea (IF ... END en una misma línea)
            if (startLine == endLine)
            {
                // Omitir el resaltado para este bloque, pero no detener el resto del proceso
                //continue; 
            }

            // Obtener la indentación del bloque
            int blockIndentation = GetIndentationLevel(document, startLine);

            // Dibujar el fondo para las líneas dentro del bloque activo
            foreach (var visualLine in textView.VisualLines)
            {
                var lineNumber = visualLine.FirstDocumentLine.LineNumber;

                // Si la línea está dentro del bloque activo y no es la línea de apertura ni de cierre
                if (lineNumber > startLine && lineNumber < endLine)
                {
                    // Calcular la posición X de la guía de indentación solo para las líneas dentro del bloque
                    double guideX = Math.Round(textView.WideSpaceWidth * blockIndentation * _textEditor.Options.IndentationSize) - textView.ScrollOffset.X + 0.5;

                    // Dibujar la guía de indentación activa
                    drawingContext.DrawLine(new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80FF5555")), 1),
                        new Point(guideX, visualLine.VisualTop - textView.ScrollOffset.Y),
                        new Point(guideX, visualLine.VisualTop + visualLine.Height - textView.ScrollOffset.Y));
                }
            }
        }

        // Función auxiliar para obtener el nivel de indentación de una línea
        private int GetIndentationLevel(TextDocument document, int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > document.LineCount)
                return 0;

            string line = document.GetText(document.GetLineByNumber(lineNumber));
            return line.TakeWhile(char.IsWhiteSpace).Count() / _textEditor.Options.IndentationSize;
        }

  
    }



    // Seleccionar un Texto y sus copias
    // El resaltador
    public class WordHighlighter : IBackgroundRenderer
    {
        private TextEditor _editor;
        private string _wordToHighlight;

        public WordHighlighter(TextEditor editor)
        {
            _editor = editor;
        }

        public void SetWordToHighlight(string word)
        {
            _wordToHighlight = word;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Selection; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null || string.IsNullOrEmpty(_wordToHighlight))
                return;

            textView.EnsureVisualLines();

            // Escapa los caracteres de escape en el texto a resaltar
            string escapedWordToHighlight = Regex.Escape(_wordToHighlight);

            var wordToHighlight = new Regex("\\b" + escapedWordToHighlight + "\\b", RegexOptions.IgnoreCase);

            foreach (VisualLine line in textView.VisualLines)
            {
                int lineStartOffset = line.FirstDocumentLine.Offset;
                string text = _editor.Document.GetText(line.StartOffset, line.VisualLength); // Aquí está la corrección
                foreach (Match m in wordToHighlight.Matches(text))
                {
                    int start = lineStartOffset + m.Index;
                    int end = start + m.Length;
                    foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, new TextSegment { StartOffset = start, EndOffset = end }))
                    {
                        drawingContext.DrawRectangle(Brushes.LightGreen, null, new Rect(r.Location, new Size(r.Width, r.Height - 1)));
                    }
                }
            }
        }

    }

    // ==== nuevo 2024 II 
    // para manejo de llaves cieeres y aperturas

    public class BracketHighlighter : IBackgroundRenderer
    {
        private TextEditor _editor;
        private int _openBracketOffset = -1;
        private int _closeBracketOffset = -1;

        public Brush HighlightBrushLight { get; set; } = Brushes.LightBlue; // Color para tema claro
        public Brush HighlightBrushDark { get; set; } = Brushes.DarkBlue;  // Color para tema oscuro

        public Typeface HighlightTypeface { get; set; } = new Typeface(
            new FontFamily("Consolas"),
            FontStyles.Normal,
            FontWeights.Bold,
            FontStretches.Normal
        );

        public BracketHighlighter(TextEditor editor)
        {
            _editor = editor;
            _editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;

            // Agrega el manejador de eventos para TextEntering
            _editor.TextArea.TextEntering += TextArea_TextEntering;
        }

        public KnownLayer Layer => KnownLayer.Selection; // Usa KnownLayer.Selection para que se muestre sobre el texto

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_openBracketOffset < 0 || _closeBracketOffset < 0 || _editor.Document == null)
                return;

            Brush backgroundBrush = Properties.Settings.Default.TemaSettings == 0
                ? new SolidColorBrush(Color.FromArgb(50, 185, 185, 185)) // Fondo azul claro semitransparente para tema claro
                : new SolidColorBrush(Color.FromArgb(0, 255, 69, 0));   // Fondo naranja semitransparente para tema oscuro
            backgroundBrush.Freeze();

            Pen borderPen = Properties.Settings.Default.TemaSettings == 0
                ? new Pen(new SolidColorBrush(Color.FromRgb(185, 185, 185)), 1) // Azul oscuro para el borde en tema claro
                : new Pen(new SolidColorBrush(Color.FromRgb(136, 136, 136)), 1); // Naranja oscuro para el borde en tema oscuro
            borderPen.Freeze();

            // Dibujar apertura de llave
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(
                            textView,
                            new TextSegment { StartOffset = _openBracketOffset, EndOffset = _openBracketOffset + 1 }))
            {
                drawingContext.DrawRectangle(backgroundBrush, borderPen, rect);
            }

            // Dibujar cierre de llave
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(
                            textView,
                            new TextSegment { StartOffset = _closeBracketOffset, EndOffset = _closeBracketOffset + 1 }))
            {
                drawingContext.DrawRectangle(backgroundBrush, borderPen, rect);
            }
        }


        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            // Reinicia los offsets
            _openBracketOffset = -1;
            _closeBracketOffset = -1;

            int offset = _editor.CaretOffset;
            string text = _editor.Text;

            if (offset < 1 || offset > text.Length)
                return;

            char currentChar = text[offset - 1];
            if ("{}[]()".Contains(currentChar))
            {
                // Encuentra el par correspondiente
                _openBracketOffset = offset - 1;
                _closeBracketOffset = FindMatchingBracket(offset - 1, text);
            }

            _editor.TextArea.TextView.Redraw(); // Redibuja el editor para actualizar el resaltado
        }

        private int FindMatchingBracket(int offset, string text)
        {
            Stack<char> stack = new Stack<char>();
            char open = text[offset];
            char close;

            switch (open)
            {
                case '{': close = '}'; break;
                case '[': close = ']'; break;
                case '(': close = ')'; break;
                case '}': close = '{'; break;
                case ']': close = '['; break;
                case ')': close = '('; break;
                default: return -1;
            }

            int direction = "{[(".Contains(open) ? 1 : -1; // Dirección de búsqueda
            for (int i = offset + direction; i >= 0 && i < text.Length; i += direction)
            {
                // Verifica si hay un carácter de escape 
                if (text[i] == '\\' && i + 1 < text.Length)
                {
                    i++; // Salta el carácter de escape
                    continue;
                }

                if (text[i] == open)
                {
                    stack.Push(open);
                }
                else if (text[i] == close)
                {
                    if (stack.Count == 0)
                    {
                        return i; // Par encontrado
                    }
                    else
                    {
                        stack.Pop(); // Elimina la llave de apertura coincidente
                    }
                }
            }

            return -1; // No encontrado
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "{")
            {
                e.Handled = true;
                _editor.Document.Insert(_editor.TextArea.Caret.Offset, e.Text + "}"); // Inserta la llave de cierre
                _editor.TextArea.Caret.Offset--; // Mueve el cursor hacia atrás
            }
            else if (e.Text == "[")
            {
                e.Handled = true;
                _editor.Document.Insert(_editor.TextArea.Caret.Offset, e.Text + "]"); // Inserta la llave de cierre
                _editor.TextArea.Caret.Offset--; // Mueve el cursor hacia atrás
            }
            else if (e.Text == "(")
            {
                e.Handled = true;
                _editor.Document.Insert(_editor.TextArea.Caret.Offset, e.Text + ")"); // Inserta el paréntesis de cierre
                _editor.TextArea.Caret.Offset--; // Mueve el cursor hacia atrás
            }
        }
    }


    // Para poner colores a llaves
    public class BracketColorizer : DocumentColorizingTransformer
    {
        private List<Color> BracketColors { get; set; }

        public BracketColorizer(double themeSelec)
        {
            BracketColors = GetBracketColorsByTheme(themeSelec);
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            string text = CurrentContext.Document.GetText(line.Offset, line.Length);
            Stack<int> bracketStack = new Stack<int>();
            bool isInsideComment = false;

            for (int i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];
                int offset = line.Offset + i;

                // Detectar comentarios "//" y omitir el resto de la línea
                if (currentChar == '/' && i + 1 < text.Length && text[i + 1] == '/')
                {
                    isInsideComment = true;
                    break; // Detenemos el análisis en el caso de "//"
                }

                // Detectar inicio de comentario multilinea "/*"
                if (currentChar == '/' && i + 1 < text.Length && text[i + 1] == '*')
                {
                    isInsideComment = true;
                }

                // Detectar fin de comentario multilinea "*/"
                if (currentChar == '*' && i + 1 < text.Length && text[i + 1] == '/')
                {
                    isInsideComment = false;
                    i++; // Saltamos el '/' de cierre
                    continue;
                }

                // Si estamos dentro de un comentario, no procesamos las llaves
                if (isInsideComment)
                    continue;

                // Procesar llaves de apertura
                if ("{[(".Contains(currentChar))
                {
                    bracketStack.Push(offset); // Agregar el offset al stack
                    int level = bracketStack.Count - 1; // Determinar nivel de anidación
                    ApplyColor(offset, level);
                }
                // Procesar llaves de cierre
                else if ("}])".Contains(currentChar))
                {
                    if (bracketStack.Count > 0)
                    {
                        int openOffset = bracketStack.Pop(); // Quitar el offset del stack
                        int level = bracketStack.Count; // Determinar nivel de anidación
                        ApplyColor(offset, level);
                    }
                }
            }
        }

        private void ApplyColor(int offset, int level)
        {
            // Determina el color basado en el nivel, con un ciclo para niveles mayores que el número de colores definidos
            Color color = BracketColors[level % BracketColors.Count];

            ChangeLinePart(
                offset, offset + 1, // Rango de caracteres (la llave)
                element => element.TextRunProperties.SetForegroundBrush(new SolidColorBrush(color))
            );
        }

        private List<Color> GetBracketColorsByTheme(double themeSelec)
        {
            // Define los colores para el tema claro
            if (themeSelec == 0) // Tema Claro
            {
                return new List<Color>
            {
                Colors.DarkBlue,  // Nivel 0
                Colors.DarkGreen, // Nivel 1
                Colors.DarkRed,   // Nivel 2
            };
            }
            // Define los colores para el tema oscuro
            else if (themeSelec == 1) // Tema Oscuro
            {
                return new List<Color>
            {
                Colors.Gold,  // Nivel 0
                Colors.Violet, // Nivel 1
                Colors.DodgerBlue,       // Nivel 2
            };
            }
            else
            {
                // Si no hay un tema definido, usar una lista predeterminada
                return new List<Color>
            {
                Colors.Gray
            };
            }
        }
    }



    public partial class MainWindow 
    {
        private Popup tooltipPopup; // para popup tooltip

        CompletionWindow completionWindow;

        // para detectar seleccion de text y activar copy o cut
        private int previousSelectionStart;
        private int previousSelectionLength;

        // para seleccionar un texto y sus derivados
        private DispatcherTimer _highlightingTimer;
        private WordHighlighter _wordHighlighter;
        private int _previousSelectionStart;
        private int _previousSelectionLength;

        //para agregar pestañas
        // Declara una ObservableCollection para almacenar las pestañas
        public ObservableCollection<TabItem> Tabs { get; set; } = new ObservableCollection<TabItem>();

        // Para cambio de thema y Idioma
        //private double ThemeSelected = Properties.Settings.Default.TemaSettings; // 0 = ligth   1 = Dark   // O defaulti install
        private double IdiomaSelected = Properties.Settings.Default.IdiomaSettings; // 0 = esp   1 = ingles  // O defaulti install


        public MainWindow()
        {
            InitializeComponent();

            // Cargar Colores y Estylo ============
            


 


            // =========== AGregados 2024 ===============
 


            //[2]Actualizar busqueda al cambiar pestaña 
            tabControl.SelectionChanged += tabControl_SelectionChanged;

            //[3] Cerrar pestañas
            tabControl.ItemsSource = Tabs;
            CloseTabCommand = new RelayCommand(CloseTab);
            DataContext = this;

            //[4] Boton Menu por defecto
            // Inicializa el temporizador de resaltado
            _highlightingTimer = new DispatcherTimer();
            _highlightingTimer.Interval = TimeSpan.FromMilliseconds(100); // Comprueba cada 100 milisegundos
            _highlightingTimer.Tick += HighlightingTimer_Tick;
            _highlightingTimer.Start();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try // Cargamos el CSV 
            {
                // Cargar los snippets al cargar la ventana
                SnippetManager.CargarSnippets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los snippets: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            //[0] convierte theme1 a un índice entero y selecciona el ítem correspondiente en el ComboBox
            ComboBoxIdioma.SelectedIndex = (int)IdiomaSelected;
            //ComboBox1.SelectedIndex = (int)ThemeSelected;

            // Obtener el valor guardado como double
            double ThemeSelected = Properties.Settings.Default.TemaSettings;

            // Convertir el valor a int antes de asignarlo al ComboBox
            int selectedIndex = Convert.ToInt32(ThemeSelected);

            // Validar que el índice sea válido antes de asignarlo
            if (selectedIndex >= 0 && selectedIndex < ComboBoxTema.Items.Count)
            {
                ComboBoxTema.SelectedIndex = selectedIndex;
            }
            else
            {
                ComboBoxTema.SelectedIndex = 0; // Por defecto, selecciona el primer elemento
            }


            WindowBackdropType backgroundSelect = new WindowBackdropType();
            ApplicationTheme themeSelect = new ApplicationTheme();

            // cambia el tema basándote en el valor almacenado
            if (ThemeSelected == 0)
            {
                // código para aplicar el tema Light

                themeSelect = ApplicationTheme.Light;
                backgroundSelect = WindowBackdropType.Tabbed;

                ((App)Application.Current).ChangeTheme("Light");
            }
            else if (ThemeSelected == 1)
            {

                themeSelect = ApplicationTheme.Dark;
                backgroundSelect = WindowBackdropType.Tabbed;


                // Obtén la referencia al recurso SolidColorBrush
                ((App)Application.Current).ChangeTheme("Dark");
            }

            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
            this,                                    // Window class
            backgroundSelect, // Background type
            true                                     // Whether to change accents automatically
            );

            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
            themeSelect, // Theme type
            backgroundSelect,  // Background type
            true);

            //[1]Cargar Pestañas 
            LoadTabsState();

            //  [2] PARA CARGAR FUENTE
            // Cargar la fuente predeterminada desde la configuración
            string fuenteSeleccionada = Properties.Settings.Default.EditorFont;

            // Verifica si la fuente está definida en los recursos de App.xaml
            if (Application.Current.Resources.Contains(fuenteSeleccionada))
            {
                FontFamily fuente = (FontFamily)Application.Current.FindResource(fuenteSeleccionada);

                ApplyFontToAllEditors(fuente);


            }

            // Seleccionar la fuente actual en el ComboBox
            ComboBoxFuente.SelectedItem = ComboBoxFuente.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == fuenteSeleccionada);

            // [2]   PARA CARGAR TAMAÑO DE TEXTO
            // Cargar el tamaño de fuente desde la configuración
            double fontSize = Properties.Settings.Default.EditorFontSize;

            // Aplicar el tamaño de fuente a todos los editores
            ApplyFontSizeToAllEditors(fontSize);

            // Seleccionar el tamaño de fuente actual en el ComboBox
            ComboBoxFontSize.SelectedItem = ComboBoxFontSize.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == fontSize.ToString());


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveTabsState(); // Guardar el estado de las pestañas antes de cerrar
        }


        //=========================================================================================
        //
        //                                  PARA BARRA DE MENUS
        //
        //=========================================================================================

        //<!--Menu Buttons Inicio  -->
        private void BotonMenu_Archivo(object sender, RoutedEventArgs e)  // BOTON AUTOR
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor.WordWrap)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Guardar",
                    Content = "¿Deseas guardar los cambios antes de Cerrar la Pestaña?",
                    PrimaryButtonText = "Si",
                    SecondaryButtonText = "No",
                    CloseButtonText = "Cancelar",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                uiMessageBox.ShowDialogAsync();
            }
            else {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Guardar",
                    Content = "No desactivado",
                    CloseButtonText = "ok",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                uiMessageBox.ShowDialogAsync();
            }


        }


        //<!-- Toogle para mostrar Barra de Herramientas  -->
        private void buttonToogleHerraCheck(object sender, RoutedEventArgs e)
        {
            // Cambia el texto del TextBlock al activarse (Checked)
            if (sender is ToggleButton toggleButton)
            {
                
                toggleButton.ToolTip = "Mostrar Barra de Herramientas";
                // Busca el TextBlock dentro del contenido del ToggleButton
                if (toggleButton.Content is System.Windows.Controls.TextBlock textBlock)
                {
                    textBlock.Text = "\ue900"; // Nuevo icono o texto (Ejemplo: cambiar el icono)
                }

                BH_Inicio.Visibility = Visibility.Collapsed;
            }
        }

        private void buttonToogleHerraUncheck(object sender, RoutedEventArgs e)
        {
            // Cambia el texto del TextBlock al desactivarse (Unchecked)
            if (sender is ToggleButton toggleButton)
            {
                toggleButton.ToolTip = "Ocultar Barra de Herramientas";
                // Busca el TextBlock dentro del contenido del ToggleButton
                if (toggleButton.Content is System.Windows.Controls.TextBlock textBlock)
                {
                    textBlock.Text = "\ue901"; // Vuelve al texto original
                }
                BH_Inicio.Visibility = Visibility.Visible;
            }
        }

        //=========================================================================================
        //
        //                            PARA CONFIGURACION DE EDITOR
        //
        //=========================================================================================

        private void BotonMenu_ConfigOpen(object sender, RoutedEventArgs e)  // BOTON AUTOR
        {

            Panel_ConfigHP.Visibility = Visibility.Visible;

        }
        private void BotonMenu_ConfigClose(object sender, RoutedEventArgs e)  // BOTON AUTOR
        {

            Panel_ConfigHP.Visibility = Visibility.Collapsed;

        }






        //=========================================================================================
        //
        //                            PARA BARRA DE HERRAMIENTAS 
        //
        //=========================================================================================

        // BOTONES HERRAMIENTAS ========================================

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor puede deshacer, deshacer la acción
            if (editor != null && editor.CanUndo)
            {
                editor.Undo();
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor puede rehacer, rehacer la acción
            if (editor != null && editor.CanRedo)
            {
                editor.Redo();
            }
        }

        private void CortarButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor tiene una selección válida, corta el texto
            if (editor != null )
            {
                editor.Cut();
            }
        }

        private void CopiarButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor tiene una selección válida, copia el texto
            if (editor != null )
            {
                editor.Copy();
            }
        }



        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor es válido, selecciona todo el texto
            if (editor != null)
            {
                editor.SelectAll();
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si hay texto en el portapapeles, pega el texto en la posición actual del cursor
            if (Clipboard.ContainsText() && editor != null)
            {
                editor.Paste();
            }
        }

        // Método para obtener el TextEditor de la pestaña activa
        private TextEditor GetActiveTextEditor()
        {
            // Obtén la pestaña activa
            TabItem activeTab = tabControl.SelectedItem as TabItem;

            // Si hay una pestaña activa, obtiene el TextEditor
            if (activeTab != null)
            {
                return activeTab.Content as TextEditor;
            }

            return null; // Si no hay una pestaña activa, devuelve null
        }

        private void BorrarButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor tiene una selección válida, borra el texto seleccionado
            if (editor != null )
            {
                editor.Delete();
                // Borra el texto seleccionado
                //editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
            }
        }


        private void AlinearButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                var code = editor.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                var openedBlock = Refactoring.FormatLines(ref code, new string(' ', editor.Options.IndentationSize));

                if (openedBlock != null)
                {
                    System.Windows.MessageBox.Show("Can't format the code because missing closing statements after:\n" + (openedBlock.Line + 1) + ": '" +
                        code[openedBlock.Line].Trim().Trim(new[] { '\n', '\r' }) + "'\n\nPlease check your code and retry.", "Format document", System.Windows.MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);

                    editor.Select(editor.Document.GetLineByNumber(openedBlock.Line + 1).Offset, editor.Document.GetLineByNumber(openedBlock.Line + 1).Length);
                    editor.ScrollToLine(openedBlock.Line + 1);
                }
                else
                {
                    int selectionStart = editor.SelectionStart, selectionEnd = editor.SelectionLength;

                    // Start a new undo group
                    editor.Document.UndoStack.StartUndoGroup();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < code.Count; i++)
                    {
                        sb.AppendLine(code[i]);
                    }

                    editor.Document.Text = sb.ToString();

                    // End the undo group
                    editor.Document.UndoStack.EndUndoGroup();

                    editor.Select(selectionStart, selectionEnd);
                }
            }
        }


        // Seleccion de Texto y sus variantes
        private string _lastSelectedWord;

        private void HighlightingTimer_Tick(object sender, EventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Comprueba si la selección ha cambiado
                if (editor.SelectionStart != _previousSelectionStart || editor.SelectionLength != _previousSelectionLength)
                {
                    // Actualiza las variables de la selección anterior
                    _previousSelectionStart = editor.SelectionStart;
                    _previousSelectionLength = editor.SelectionLength;

                    // Obtén la palabra actualmente seleccionada
                    string selectedText = editor.SelectedText;

                    // Si no hay texto seleccionado, borra la palabra a resaltar
                    if (string.IsNullOrWhiteSpace(selectedText))
                    {
                        _wordHighlighter.SetWordToHighlight(null);
                        // Borra los resaltados del minimapa
                        minimap.Children.Clear();
                        // Borra la última palabra seleccionada
                        _lastSelectedWord = null;
                    }
                    else
                    {
                        // Configura el resaltador para resaltar la palabra seleccionada
                        _wordHighlighter.SetWordToHighlight(selectedText);

                        // Guarda la última palabra seleccionada
                        _lastSelectedWord = selectedText;

                        // Redibuja el minimapa
                        UpdateMinimap(selectedText);
                    }

                    // Redibuja la vista de texto para aplicar el resaltado solo en la pestaña activa
                    editor.TextArea.TextView.Redraw();
                }
            }
        }

        private void ClearWordHighlighter(TextEditor editor)
        {
            if (editor != null && editor.TextArea.TextView.BackgroundRenderers.Contains(_wordHighlighter))
            {
                _wordHighlighter.SetWordToHighlight(null); // Limpia la palabra a resaltar
                editor.TextArea.TextView.Redraw();        // Fuerza el redibujado para borrar los resaltados
            }
        }


        // Ver minimapa en color Esto es test se puede quitar // Incluso podemos agregarlo en la opcion de Configuracion del Programa
        private void UpdateMinimap(string selectedWord)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                minimap.Children.Clear();

                var text = editor.Text;
                var lines = text.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    // Verificar que la línea y la palabra seleccionada no sean nulas o vacías
                    if (!string.IsNullOrEmpty(lines[i]) && !string.IsNullOrEmpty(selectedWord) && lines[i].Contains(selectedWord))
                    {
                        var rect = new Rectangle
                        {
                            Fill = Brushes.Blue,
                            Width = 5,
                            Height = 2,
                        };

                        Canvas.SetTop(rect, (double)i / lines.Length * (minimap.Height-37));
                        minimap.Children.Add(rect);
                    }
                }
            }
        }

        //Actualizar altura del minimapa en tiempo real
        private void Editor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Obtén el editor que generó el evento
            TextEditor editor = sender as TextEditor;

            if (editor != null)
            {
                // Encuentra el minimapa correspondiente (si es único)
                if (minimap != null)
                {
                    // Ajusta la altura del minimapa al nuevo tamaño del editor
                    minimap.Height = editor.ActualHeight;
                }
            }
        }


        // redibuja el canvas cada vez que se maximiza o minimiza
        private DispatcherTimer resizeTimer;

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (resizeTimer == null)
            {
                resizeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(200)
                };
                resizeTimer.Tick += (s, args) =>
                {
                    resizeTimer.Stop();
                    RedrawMinimap();
                };
            }

            resizeTimer.Stop();
            resizeTimer.Start();
        }

        private void RedrawMinimap()
        {
            TextEditor editor = GetActiveTextEditor();
            if (editor != null)
            {
                UpdateMinimap(_lastSelectedWord);
                AddHighlightRenderer(editor);
            }
        }

        // Manejo de Resaltado de Línea Actual
        private void AddHighlightRenderer(TextEditor editor)
        {
            var existingRenderer = editor.TextArea.TextView.BackgroundRenderers
                .OfType<HighlightCurrentLineBackgroundRenderer>()
                .FirstOrDefault();

            if (existingRenderer != null)
            {
                editor.TextArea.TextView.BackgroundRenderers.Remove(existingRenderer);
            }
            
            var renderer = new HighlightCurrentLineBackgroundRenderer(editor, minimap);
            editor.TextArea.TextView.BackgroundRenderers.Add(renderer);
        }



        //=========================================================================================
        //
        //                            COMANDOS DE COMBINACION DE TECLAS
        //
        //=========================================================================================

        private void CodeEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control) { e.Handled = true; CrearPestana(); }
                if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control) { e.Handled = true; FuncionAbrir(); }
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control) { e.Handled = true; FuncionGuardar(); }
                if (e.Key == Key.S && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt)){e.Handled = true;FuncionGuardarComo();}
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.CapsLock)){e.Handled = true;FuncionGuardarTodo();}

                // Comprueba si el usuario presionó Ctrl + F
                if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control){e.Handled = true;FuncionBuscar();}
                if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control) { e.Handled = true; FuncionReemplazar(); }


            }
        }
          
        //=========================================================================================
        //
        //                            PARA FUNCION DE BUSCAR Y REEMPLAZAR
        //
        //=========================================================================================

        //boton buscar
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FuncionBuscar();
        }

        //Boton reemplazar
        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            FuncionReemplazar();
        }


        private void FuncionBuscar() 
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                if (BorderPanelBuscar.Visibility == Visibility.Collapsed)
                {
                    MiTextBoxBuscar.Text = "";
                    MiTextBlockBuscar.Visibility = Visibility.Visible;

                    // Si hay texto seleccionado en el editor de código, cópialo en el TextBox de búsqueda
                    if (!string.IsNullOrEmpty(editor.SelectedText))
                    {
                        MiTextBlockBuscar.Visibility = Visibility.Collapsed;
                        MiTextBoxBuscar.Text = editor.SelectedText;

                    }



                    BorderPanelBuscar.Visibility = Visibility.Visible;

                    //Restaura Boton collapse
                    TextSearchCollapse.Text = "\uE972";
                    PanelReplaceVisibility.Visibility = Visibility.Collapsed;

                    //botones toogle
                    UpdateMatchCaseButtonColor(); // mayuscula y minusculas
                    UpdateWholeWordSearchButtonColor(); // Solo palabras completas
                    UpdateUseRegexButtonColor(); // usar expresion regular
                }
            }

        }

        private void FuncionReemplazar()
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                if (BorderPanelBuscar.Visibility == Visibility.Collapsed)
                {
                    MiTextBoxBuscar.Text = "";
                    MiTextBlockBuscar.Visibility = Visibility.Visible;

                    MiTextBoxReemplazar.Text = "";
                    MiTextBlockReemplazar.Visibility = Visibility.Visible;

                    // Si hay texto seleccionado en el editor de código, cópialo en el TextBox de búsqueda
                    if (!string.IsNullOrEmpty(editor.SelectedText))
                    {
                        MiTextBlockBuscar.Visibility = Visibility.Collapsed;
                        MiTextBoxBuscar.Text = editor.SelectedText;

                    }

                    BorderPanelBuscar.Visibility = Visibility.Visible;

                    TextSearchCollapse.Text = "\uE971";
                    PanelReplaceVisibility.Visibility = Visibility.Visible;

                    //botones toogle
                    UpdateMatchCaseButtonColor(); // mayuscula y minusculas
                    UpdateWholeWordSearchButtonColor(); // Solo palabras completas
                    UpdateUseRegexButtonColor(); // usar expresion regular
                }
            }

        }


        //Cerrar panel de busqueda
        private void SearchClosedButton_Click(object sender, RoutedEventArgs e)
        {
            if (BorderPanelBuscar.Visibility == Visibility.Visible)
            {
                BorderPanelBuscar.Visibility = Visibility.Collapsed;

                //Restaura Boton collapse
                TextSearchCollapse.Text = "\uE972";
                PanelReplaceVisibility.Visibility = Visibility.Collapsed;

                MiTextBoxBuscar.Text = "";
                MiTextBoxReemplazar.Text = "";

                MiTextBlockBuscar.Visibility = Visibility.Visible;
                MiTextBlockReemplazar.Visibility = Visibility.Visible;

                TextCountSearch.Text = "No hay resultados";
            }

        }

        //boton para alternar buscar y reemplazar
        private void SearchReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (PanelReplaceVisibility.Visibility == Visibility.Collapsed)
            {
                TextSearchCollapse.Text = "\uE971";
                PanelReplaceVisibility.Visibility = Visibility.Visible;
            }
            else 
            {
                TextSearchCollapse.Text = "\uE972";
                PanelReplaceVisibility.Visibility = Visibility.Collapsed;

            }
        }


        private void TextBoxBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            MiTextBlockBuscar.Visibility = Visibility.Collapsed;
            BorderSearchContent.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0090F1"));
        }

        private void TextBoxBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            BorderSearchContent.BorderBrush = Brushes.Transparent;

            if (string.IsNullOrEmpty(MiTextBoxBuscar.Text))
            {
                MiTextBlockBuscar.Visibility = Visibility.Visible;
            }
        }


        // Lista para almacenar todas las coincidencias
        List<int> matches = new List<int>();

        // Índice de la coincidencia actual
        int currentMatchIndex = 0;

        // Variable para almacenar el TextEditor actual
        TextEditor currentEditor;

        // Variable para almacenar el texto de búsqueda actual
        string currentSearchText = "";

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Obtén el texto de búsqueda actual
            currentSearchText = MiTextBoxBuscar.Text;

            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = getActiveTextEditor();

            // Actualiza la búsqueda en la pestaña activa
            UpdateSearch(editor);
        }

        private void UpdateSearch(TextEditor editor)
        {
            if (editor != null)
            {
                // Guarda la posición de la selección
                int selectionStart = editor.SelectionStart;
                int selectionLength = editor.SelectionLength;

                // Obtén el texto de búsqueda
                string searchText = currentSearchText;

                // Si el texto de búsqueda está vacío, no hagas nada
                if (string.IsNullOrEmpty(searchText))
                {
                    TextCountSearch.Text = "No hay resultados";
                    PreviousButton.IsEnabled = false;
                    NextButton.IsEnabled = false;
                    ErrorLabel.Visibility = Visibility.Collapsed;
                    return;
                }

                // Limpia la lista de coincidencias
                matches.Clear();

                // Define la expresión regular
                Regex regex;

                if (useRegex)
                {
                    try
                    {
                        // Crea la expresión regular
                        regex = new Regex(searchText, matchCase ? RegexOptions.Multiline : RegexOptions.IgnoreCase);

                        // Realiza la búsqueda y almacena todas las coincidencias
                        Match match = regex.Match(editor.Document.Text);
                        while (match.Success)
                        {
                            matches.Add(match.Index);
                            match = match.NextMatch();
                        }

                        // Si la expresión regular es válida, oculta el Label
                        ErrorLabel.Visibility = Visibility.Collapsed;
                    }
                    catch (RegexParseException)
                    {
                        // Si la expresión regular no es válida, muestra el Label
                        ErrorLabel.Visibility = Visibility.Visible;
                        return; // Añade esto para salir del método si la expresión regular no es válida
                    }
                }
                else
                {
                    // Si el usuario no ha activado el uso de expresiones regulares, escapa el texto de búsqueda y busca palabras completas si es necesario
                    string pattern = wholeWordSearch ? $@"\b{Regex.Escape(searchText)}\b" : Regex.Escape(searchText);
                    regex = new Regex(pattern, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);

                    // Realiza la búsqueda y almacena todas las coincidencias
                    Match match = regex.Match(editor.Document.Text);
                    while (match.Success)
                    {
                        matches.Add(match.Index);
                        match = match.NextMatch();
                    }
                }

                // Si hay al menos una coincidencia, selecciona la más cercana a la posición actual del cursor
                if (matches.Count > 0)
                {
                    // Encuentra la coincidencia más cercana a la posición actual del cursor
                    int closestMatchIndex = matches.Select((m, i) => new { Index = i, Difference = Math.Abs(m - editor.CaretOffset) })
                                                   .OrderBy(m => m.Difference)
                                                   .First()
                                                   .Index;

                    currentMatchIndex = closestMatchIndex;

                    // Calcula la longitud de la coincidencia
                    int matchLength = regex.Match(editor.Document.Text, matches[currentMatchIndex]).Length;

                    if (matchLength > 0)
                    {
                        editor.Select(matches[currentMatchIndex], matchLength);
                        editor.ScrollToLine(editor.Document.GetLineByOffset(matches[currentMatchIndex]).LineNumber);
                        TextCountSearch.Text = $"{currentMatchIndex + 1} de {matches.Count}";
                        PreviousButton.IsEnabled = true;
                        NextButton.IsEnabled = true;
                    }
                    else
                    {
                        TextCountSearch.Text = "No hay resultados";
                        PreviousButton.IsEnabled = false;
                        NextButton.IsEnabled = false;
                    }
                }
                else
                {
                    TextCountSearch.Text = "No hay resultados";
                    PreviousButton.IsEnabled = false;
                    NextButton.IsEnabled = false;
                }

                // Guarda el TextEditor actual
                currentEditor = editor;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Si hay coincidencias y un TextEditor actual, ve a la siguiente coincidencia
            if (matches.Count > 0 && currentEditor != null)
            {
                currentMatchIndex = (currentMatchIndex + 1) % matches.Count;

                // Obtén la línea y la columna de la coincidencia
                int matchLine = currentEditor.Document.GetLineByOffset(matches[currentMatchIndex]).LineNumber;
                // Obtén la ubicación del offset y luego la columna
                var matchLocation = currentEditor.Document.GetLocation(matches[currentMatchIndex]);
                int matchColumn = matchLocation.Column;

                // Selecciona la coincidencia y desplázate a la línea y la columna
                currentEditor.Select(matches[currentMatchIndex], MiTextBoxBuscar.Text.Length);
                currentEditor.ScrollTo(matchLine, matchColumn);

                // Actualiza el TextBlock con la posición y el total de coincidencias
                TextCountSearch.Text = $" {currentMatchIndex + 1} de {matches.Count}";
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            // Si hay coincidencias y un TextEditor actual, ve a la anterior coincidencia
            if (matches.Count > 0 && currentEditor != null)
            {
                currentMatchIndex = (currentMatchIndex - 1 + matches.Count) % matches.Count;

                // Obtén la línea y la columna de la coincidencia
                int matchLine = currentEditor.Document.GetLineByOffset(matches[currentMatchIndex]).LineNumber;
                // Obtén la ubicación del offset y luego la columna
                var matchLocation = currentEditor.Document.GetLocation(matches[currentMatchIndex]);
                int matchColumn = matchLocation.Column;

                // Selecciona la coincidencia y desplázate a la línea y la columna
                currentEditor.Select(matches[currentMatchIndex], MiTextBoxBuscar.Text.Length);
                currentEditor.ScrollTo(matchLine, matchColumn);

                // Actualiza el TextBlock con la posición y el total de coincidencias
                TextCountSearch.Text = $" {currentMatchIndex + 1} de {matches.Count}";
            }
        }

        //para activar busqueda exacta de Mayusculas y minusculas
        bool matchCase = Properties.Settings.Default.CaseSensitiveSearch;
        private void UpdateMatchCaseButtonColor()
        {
 
            double ThemeSelected = Properties.Settings.Default.TemaSettings;

            // Determina el color del borde según el tema
            SolidColorBrush ColorFondoToogleSearcha = ThemeSelected == 0
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CDCFE")) // Tema claro
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E9BFA")); // Tema oscuro

            // Cambiar el grosor del borde del botón basándote en el valor de matchCase
            MatchCaseButton.BorderThickness = new Thickness(0);

            MatchCaseButton.Background = matchCase ? ColorFondoToogleSearcha : Brushes.Transparent;

        }

        private void MatchCaseButton_Click(object sender, RoutedEventArgs e)
        {
            // Alternar el estado de matchCase
            matchCase = !matchCase;

            // Guardar el estado en la configuración de la aplicación
            Properties.Settings.Default.CaseSensitiveSearch = matchCase;
            Properties.Settings.Default.Save();

            // Actualizar el color del botón
            UpdateMatchCaseButtonColor();

            // Resto del código...
            // Llamar a la función de búsqueda
            SearchTextBox_TextChanged(sender, null);
        }


        // Para activar la búsqueda de palabras completas
        bool wholeWordSearch = Properties.Settings.Default.WholeWordSearch;

        private void UpdateWholeWordSearchButtonColor()
        {
            double ThemeSelected = Properties.Settings.Default.TemaSettings;

            // Determina el color del borde según el tema
            SolidColorBrush ColorFondoToogleSearcha = ThemeSelected == 0
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CDCFE")) // Tema claro
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E9BFA")); // Tema oscuro

            // Cambiar el grosor del borde del botón basándote en el valor de matchCase
            MatchCaseButton.BorderThickness = new Thickness(0);

            WholeWordSearchButton.Background = wholeWordSearch ? ColorFondoToogleSearcha : Brushes.Transparent;
        }

        private void WholeWordSearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Alternar el estado de wholeWordSearch
            wholeWordSearch = !wholeWordSearch;

            // Guardar el estado en la configuración de la aplicación
            Properties.Settings.Default.WholeWordSearch = wholeWordSearch;
            Properties.Settings.Default.Save();

            // Actualizar el color del botón
            UpdateWholeWordSearchButtonColor();

            // Llamar a la función de búsqueda
            SearchTextBox_TextChanged(sender, null);
        }


        // Para activar el uso de expresiones regulares
        bool useRegex = Properties.Settings.Default.UseRegex;

        private void UpdateUseRegexButtonColor()
        {
            double ThemeSelected = Properties.Settings.Default.TemaSettings;

            // Determina el color del borde según el tema
            SolidColorBrush ColorFondoToogleSearcha = ThemeSelected == 0
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CDCFE")) // Tema claro
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E9BFA")); // Tema oscuro

            // Cambiar el grosor del borde del botón basándote en el valor de matchCase
            MatchCaseButton.BorderThickness = new Thickness(0);

            UseRegexButton.Background = useRegex ? ColorFondoToogleSearcha : Brushes.Transparent;
        }

        private void UseRegexButton_Click(object sender, RoutedEventArgs e)
        {
            // Alternar el estado de useRegex
            useRegex = !useRegex;

            // Guardar el estado en la configuración de la aplicación
            Properties.Settings.Default.UseRegex = useRegex;
            Properties.Settings.Default.Save();

            // Actualizar el color del botón
            UpdateUseRegexButtonColor();

            // Llamar a la función de búsqueda
            SearchTextBox_TextChanged(sender, null);
        }

        // Método para obtener el TextEditor de la pestaña activa
        private TextEditor getActiveTextEditor()
        {
            // Obtén la pestaña activa
            TabItem activeTab = tabControl.SelectedItem as TabItem;

            // Si hay una pestaña activa, obtiene el TextEditor
            if (activeTab != null)
            {
                return activeTab.Content as TextEditor;
            }

            return null; // Si no hay una pestaña activa, devuelve null
        }

        // Suscribirse al evento SelectionChanged del TabControl
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            // Guarda la posición actual del caret en la pestaña activa antes de cambiar de pestaña
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is TabItem previousTab && previousTab.Tag is TabFileData previousFileData)
            {



                // Obtén el TextEditor de la pestaña anterior
                if (previousTab.Content is TextEditor previousEditor)
                {

                    // Limpia el resaltado en la pestaña anterior
                    ClearWordHighlighter(previousEditor);

                    // Actualiza la posición del caret en los datos de la pestaña
                    previousFileData.CaretLine = previousEditor.TextArea.Caret.Line;
                    previousFileData.CaretColumn = previousEditor.TextArea.Caret.Column;
                }
            }

            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null && tabControl.SelectedItem is TabItem selectedTab && selectedTab.Tag is TabFileData fileData)
            {
                // Actualizar la búsqueda al cambiar de pestaña
                UpdateSearch(editor);

                

                // Restaurar la posición del caret y el scroll para la pestaña activa
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Asegúrate de que los datos corresponden a la pestaña activa
                    if (tabControl.SelectedItem == selectedTab)
                    {
                        editor.TextArea.Caret.Line = fileData.CaretLine;
                        editor.TextArea.Caret.Column = fileData.CaretColumn;

                        // Desplázate a la posición correcta
                        editor.ScrollTo(fileData.CaretLine, fileData.CaretColumn);


                    }
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);

                _wordHighlighter = new WordHighlighter(editor);
                editor.TextArea.TextView.BackgroundRenderers.Add(_wordHighlighter);
            }
        }


        // para panel reemplazar
        private void TextBoxReemplazar_GotFocus(object sender, RoutedEventArgs e)
        {
            MiTextBlockReemplazar.Visibility = Visibility.Collapsed;
            BorderReplaceContent.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0090F1"));
        }

        private void TextBoxReemplazar_LostFocus(object sender, RoutedEventArgs e)
        {
            BorderReplaceContent.BorderBrush = Brushes.Transparent;

            if (string.IsNullOrEmpty(MiTextBoxReemplazar.Text))
            {
                MiTextBlockReemplazar.Visibility = Visibility.Visible;
            }
        }


        // para boton toogle de conservar mayusculas y minusculas al remmplazar
        // Para activar la conservación de mayúsculas y minúsculas
        bool preserveCase = Properties.Settings.Default.PreserveCase;

        private void UpdatePreserveCaseButtonColor()
        {
            double ThemeSelected = Properties.Settings.Default.TemaSettings;

            // Determina el color del borde según el tema
            SolidColorBrush ColorFondoToogleSearcha = ThemeSelected == 0
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CDCFE")) // Tema claro
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E9BFA")); // Tema oscuro

            // Cambiar el grosor del borde del botón basándote en el valor de matchCase
            MatchCaseButton.BorderThickness = new Thickness(0);

            PreserveCaseButton.Background = preserveCase ? ColorFondoToogleSearcha : Brushes.Transparent;
        }

        private void PreserveCaseButton_Click(object sender, RoutedEventArgs e)
        {
            // Alternar el estado de preserveCase
            preserveCase = !preserveCase;

            // Guardar el estado en la configuración de la aplicación
            Properties.Settings.Default.PreserveCase = preserveCase;
            Properties.Settings.Default.Save();

            // Actualizar el color del botón
            UpdatePreserveCaseButtonColor();

            // Llamar a la función de búsqueda
            SearchTextBox_TextChanged(sender, null);
        }


        //reemplazar siguiente palabra
        private void ReplaceNextButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Obtén el texto de búsqueda y el texto de reemplazo
                string searchText = MiTextBoxBuscar.Text;
                string replaceText = MiTextBoxReemplazar.Text;

                // Si el texto de búsqueda está vacío o no hay coincidencias, no hagas nada
                if (string.IsNullOrEmpty(searchText) || matches.Count == 0)
                {
                    return;
                }

                // Define la expresión regular
                string pattern = wholeWordSearch ? $@"\b{Regex.Escape(searchText)}\b" : Regex.Escape(searchText);
                Regex regex = new Regex(pattern, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);

                // Reemplaza la siguiente ocurrencia del texto de búsqueda con el texto de reemplazo
                int start = matches[currentMatchIndex];
                Match match = regex.Match(editor.Document.Text, start);
                int length = match.Length;

                if (preserveCase)
                {
                    // Si se debe conservar el caso, ajusta el caso del texto de reemplazo para que coincida con el caso del texto de búsqueda
                    replaceText = match.Value.All(char.IsUpper) ? replaceText.ToUpper() :
                                  match.Value.All(char.IsLower) ? replaceText.ToLower() :
                                  CultureInfo.CurrentCulture.TextInfo.ToTitleCase(replaceText);
                }

                editor.Document.Replace(start, length, replaceText);

                // Incrementa currentMatchIndex, y reinícialo a 0 si alcanza el final de la lista de coincidencias
                currentMatchIndex = (currentMatchIndex + 1) % matches.Count;

                // Llama a la función de búsqueda para actualizar la lista de coincidencias
                SearchTextBox_TextChanged(sender, null);
            }
        }



        //reemplazar todo
        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Obtén el texto de búsqueda y el texto de reemplazo
                string searchText = MiTextBoxBuscar.Text;
                string replaceText = MiTextBoxReemplazar.Text;

                // Si el texto de búsqueda está vacío, no hagas nada
                if (string.IsNullOrEmpty(searchText))
                {
                    return;
                }

                // Define la expresión regular
                string pattern = wholeWordSearch ? $@"\b{Regex.Escape(searchText)}\b" : Regex.Escape(searchText);
                Regex regex = new Regex(pattern, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);

                // Reemplaza todas las ocurrencias del texto de búsqueda con el texto de reemplazo
                string newText = regex.Replace(editor.Document.Text, match =>
                {
                    if (preserveCase)
                    {
                        // Si se debe conservar el caso, ajusta el caso del texto de reemplazo para que coincida con el caso del texto de búsqueda
                        return match.Value.All(char.IsUpper) ? replaceText.ToUpper() :
                               match.Value.All(char.IsLower) ? replaceText.ToLower() :
                               CultureInfo.CurrentCulture.TextInfo.ToTitleCase(replaceText);
                    }
                    else
                    {
                        return replaceText;
                    }
                });

                // Actualiza el texto del editor de código con el nuevo texto
                editor.Document.Text = newText;

                // Llama a la función de búsqueda para actualizar la lista de coincidencias
                SearchTextBox_TextChanged(sender, null);
            }
        }



        //=========================================================================================
        //
        //                           FUNCIONES ADICIONALES DE AVALON EDIT
        //
        //=========================================================================================
        //==============================================================
        // para autocompletado
        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                HandleAutocomplete(editor, e);

                // Manejo de la indentación automática
                HandleAutoIndentation(editor, e);
            }
        }
        
        // Método para manejar el autocompletado
        private void HandleAutocomplete(TextEditor editor, TextCompositionEventArgs e)
        {
            if (editor != null)
            {
                // Si el texto ingresado no es una letra o un número, y la ventana de autocompletado está abierta, ciérrala
                if (!char.IsLetterOrDigit(e.Text, 0))
                {
                    if (completionWindow != null)
                    {
                        completionWindow.Close();
                        completionWindow = null;
                    }
                    return;
                }

                // Obtén la posición del cursor
                int caretPosition = editor.TextArea.Caret.Offset;

                // Obtén el texto hasta la posición del cursor
                string textUntilCaret = editor.TextArea.Document.GetText(0, caretPosition);

                // Si el último carácter ingresado es un espacio en blanco o salto de línea, cierra la ventana de autocompletado
                if (e.Text == " " || e.Text == "\n" || e.Text == "\r")
                {
                    if (completionWindow != null)
                    {
                        completionWindow.Close();
                        completionWindow = null;
                    }
                    return;
                }

                // Utiliza una expresión regular para obtener la última palabra en textUntilCaret
                Match matchLastWord = Regex.Match(textUntilCaret, @"\b(\w+)$");
                string lastWord = matchLastWord.Groups[1].Value;

                // Si la ventana de autocompletado ya está abierta, ciérrala
                if (completionWindow != null)
                {
                    completionWindow.Close();
                    completionWindow = null;
                }

                // Abrir la ventana de autocompletado
                completionWindow = new CompletionWindow(editor.TextArea);
                completionWindow.Width = 300;

                // Cambiar el fondo de la lista de sugerencias

                completionWindow.Style = (Style)Application.Current.Resources["CustomWindowStyle"];
                completionWindow.CompletionList.ListBox.Style = (Style)Application.Current.Resources["CustomListBoxStyle"];
                completionWindow.CompletionList.ListBox.ItemContainerStyle = (Style)Application.Current.FindResource("CompletionListBoxItemStyle");

                List<MyCompletionData> data = new List<MyCompletionData>();

                // Limpia los datos de autocompletado existentes
                data.Clear();

                // Define los comandos y palabras clave predefinidos que deseas mostrar
                string[] predefinedCommands = new string[] 
                { 
                    "for","while","repeat", "FOR","WHILE","REPEAT",
                    "if", "case", "iferr","IF", "CASE", "IFERR",
                    "begin", "BEGIN", "EXPORT","Function_Name", 

                }; // Agrega más comandos aquí

                string[] predefinedFunctions = new string[] 
                {
                    "for","while","repeat", "FOR","WHILE","REPEAT",
                    "if", "case", "iferr","IF", "CASE", "IFERR",
                    "begin", "BEGIN", "EXPORT","Function_Name",

                }; // Agrega tus funciones aquí



                // Combina los comandos y funciones en una sola lista con categorías
                foreach (string command in predefinedCommands)
                {
                    data.Add(new MyCompletionData(command, "command")); // Agrega como comando

                }
                
                foreach (string function in predefinedFunctions)
                {
                    data.Add(new MyCompletionData(function, "function")); // Agrega como función

                }

                // Filtrar y ordenar las palabras según la entrada del usuario
                string userInput = lastWord; // Lo que el usuario ha escrito hasta ahora

                List<MyCompletionData> filteredData = data
                    // Priorizar coincidencias exactas sensibles a mayúsculas/minúsculas
                    .OrderBy(word => word.Text.StartsWith(userInput, StringComparison.Ordinal) ? 0 : 1)
                    // Priorizar coincidencias exactas ignorando mayúsculas/minúsculas
                    .ThenBy(word => word.Text.StartsWith(userInput, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    // Priorizar por tipo (comando primero, función después)
                    .ThenBy(word => word.Priority)
                    // Finalmente, ordena alfabéticamente
                    .ThenBy(word => word.Text)
                    // Filtrar solo las palabras que comienzan con el texto ingresado (ignorar mayúsculas para incluir todo)
                    .Where(word => word.Text.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
                    .ToList();


                // Si hay elementos en la lista de autocompletado, muestra la ventana de autocompletado
                if (filteredData.Count > 0)
                {
                    completionWindow.Show();

                    // Limpia la lista de autocompletado antes de agregar los nuevos datos
                    completionWindow.CompletionList.CompletionData.Clear();

                    // Agrega los elementos filtrados y ordenados al CompletionList
                    foreach (var item in filteredData)
                    {
                        completionWindow.CompletionList.CompletionData.Add(item);
                    }

                    // Selecciona el primer elemento de la lista visible
                    completionWindow.CompletionList.SelectedItem = completionWindow.CompletionList.CompletionData[0];

                    // Evento para cerrar la ventana cuando se cierre
                    completionWindow.Closed += delegate { completionWindow = null; };

                    // Agrega un controlador de eventos para el doble clic
                    completionWindow.CompletionList.ListBox.MouseDoubleClick += (sender, e) =>
                    {
                        completionWindow.CompletionList.RequestInsertion(e);
                    };
                }
                else
                {
                    // Si no hay opciones para mostrar, cierra la ventana de autocompletado
                    if (completionWindow != null)
                    {
                        completionWindow.Close();
                        completionWindow = null;
                    }
                }



            }
        }


        // Método para manejar la indentación automática
        private void HandleAutoIndentation(TextEditor editor, TextCompositionEventArgs e)
        {
            if (e.Text == "\n") // Detectar cuando se presiona Enter
            {
                if (editor == null) return;

                // Obtener la posición del cursor después del salto
                var caret = editor.TextArea.Caret;
                var currentLine = editor.Document.GetLineByNumber(caret.Line);
                var previousLine = editor.Document.GetLineByNumber(caret.Line - 1);

                // Obtener el texto de la línea anterior
                var previousLineText = editor.Document.GetText(previousLine);

                // Obtener la información del bloque actual
                var blockDetector = new BlockAnalyzer();
                var blockState = blockDetector.GetBlockState(editor.Document.Lines.Select(l => editor.Document.GetText(l)).ToList(), caret.Line - 1);

                // Calcular la indentación basada en la profundidad del bloque
                var indentationLevel = blockState?.depth ?? 0;  // Si no hay bloque, la indentación es 0
                var leadingWhitespace = new string(' ', indentationLevel * 4); // Ajusta el tamaño de la indentación según tus preferencias

                // Aplicar la indentación calculada a la línea actual
                editor.Document.Insert(currentLine.Offset, leadingWhitespace);
                editor.TextArea.Caret.Offset = currentLine.Offset + leadingWhitespace.Length;
            }
        }


        //========================================


        // edicion COPIAR seleccion



        // HOOVER TOOLTIP
        private void TextView_MouseMove(object sender, MouseEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Obtener la posición del mouse en el editor de texto
                var position = editor.TextArea.TextView.GetPositionFloor(e.GetPosition(editor.TextArea.TextView));
                if (position.HasValue)
                {
                    int offset = editor.Document.GetOffset(position.Value.Line, position.Value.Column);

                    if (offset >= 0 && offset < editor.Document.TextLength) // Verifica si el offset está dentro del rango del documento
                    {
                        string text = editor.Document.GetText(offset, 1);

                        if (!string.IsNullOrWhiteSpace(text) && Regex.IsMatch(text, @"\w")) // Verifica si el carácter en la posición del cursor es una letra
                        {
                            var start = TextUtilities.GetNextCaretPosition(editor.Document, offset, LogicalDirection.Backward, CaretPositioningMode.WordBorder);
                            var end = TextUtilities.GetNextCaretPosition(editor.Document, offset, LogicalDirection.Forward, CaretPositioningMode.WordBorder);
                            if (start >= 0 && end >= 0 && start < editor.Document.TextLength && end <= editor.Document.TextLength)
                            {
                                string word = editor.Document.GetText(start, end - start);

                                HoverProvider.KeywordInfo keywordInfo = HoverProvider.KeywordInfos.FirstOrDefault(ki => ki.Keywords.Contains(word));

                                if (keywordInfo != null)
                                {
                                    Panel_Hover(keywordInfo.Info);
                                }
                            }
                        }
                        else
                        {
                            // Si el cursor no está sobre una palabra, oculta el Popup
                            CloseTooltip();
                        }
                    }
                    else
                    {
                        // Si el cursor no está sobre una línea de texto, oculta el Popup
                        CloseTooltip();
                    }
                }
                else
                {
                    // Si el cursor no está sobre el editor de texto, oculta el Popup
                    CloseTooltip();
                }
            }
        }


        //crear panel hoover si existe comando 
        private void Panel_Hover(List<string> infoList)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Crear un nuevo Popup si no existe
                if (tooltipPopup == null)
                {
                    tooltipPopup = new Popup();
                    tooltipPopup.Placement = PlacementMode.Mouse; // Cambia el Placement a Mouse

                }

                var stackPanel = new StackPanel()
                {
                    Width = 500,
                    Margin = new Thickness(0, 5, 0, 5),

                };

                foreach (var info in infoList)
                {
                    // Agregar texto
                    // Crear un TextEditor
                    var textEditor = new TextEditor
                    {


                        IsReadOnly = true, // Hace que el TextEditor sea de solo lectura
                        //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F6F6F6")), // Cambia el color de fondo a blanco
                        Background = (Brush)Application.Current.Resources["PanelHooverColor"],
                        Foreground = (Brush)Application.Current.Resources["PanelHooverTextColor"], // Cambia el color del texto a negro
                        BorderThickness = new Thickness(0), // Elimina el borde
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, // Establece la visibilidad de la barra de desplazamiento horizontal
                        VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, // Establece la visibilidad de la barra de desplazamiento vertical
                        Margin = new Thickness(10, 0, 10, 0), // Establece un relleno alrededor del TextEditor
                        Text = info,
                        FontSize = 12,
                        FontFamily = (FontFamily)Application.Current.Resources["Cascadia Mono Light"],


                    };

                    // Configurar opciones de renderizado de texto para mejor claridad y nitidez
                    TextOptions.SetTextFormattingMode(textEditor, TextFormattingMode.Ideal);
                    TextOptions.SetTextRenderingMode(textEditor, TextRenderingMode.ClearType);
                    TextOptions.SetTextHintingMode(textEditor, TextHintingMode.Fixed);


                    // Cargar el archivo XSHD personalizado
                    LoadSyntaxHighlighting(textEditor); // aplica Syntasix color
                    FillLavesAperurasYcierres(textEditor);// Pinta llaves

                    TextOptions.SetTextRenderingMode(textEditor, TextRenderingMode.ClearType);
                    RenderOptions.SetClearTypeHint(textEditor, ClearTypeHint.Enabled);
                    stackPanel.Children.Add(textEditor);

                    // Agregar un separador
                    stackPanel.Children.Add(new Separator { Margin = new Thickness(0, 5, 0, 5) });
                }

                // Eliminar el último separador
                if (stackPanel.Children.Count > 0)
                {
                    stackPanel.Children.RemoveAt(stackPanel.Children.Count - 1);
                }



                // Crear un Border y agregar el TextEditor a él
                var border = new Border
                {
                    Child = stackPanel,
                    Background = (Brush)Application.Current.Resources["PanelHooverColor"],
                    BorderBrush = (Brush)Application.Current.Resources["ColorContextBorde"], // Cambia el color del borde a negro
                    BorderThickness = new Thickness(1), // Establece el grosor del borde
                    Margin = new Thickness(2),
                    CornerRadius = new CornerRadius(2), // Establece el CornerRadius

                };

                // Establecer el contenido del Popup
                tooltipPopup.Child = border;
                tooltipPopup.AllowsTransparency = true;


                // Mostrar el Popup
                tooltipPopup.IsOpen = true;
            }
        }

        private void Editor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseTooltip();
        }

        // Método para ocultar el tooltip
        private void CloseTooltip()
        {
            if (tooltipPopup != null && tooltipPopup.IsOpen)
            {
                tooltipPopup.IsOpen = false;
            }
        }




        //=========================================================================================
        //
        //                            PARA CONFIGURACION INICIAL DE EDITOR
        //
        //=========================================================================================
        string currentFile = null;
        string lastSavedContent = "";

        // BOTON NUEVA PESTAÑA
        // Método para configurar el editor
        private void ConfigureEditor(TextEditor editor)
        {

            // [1] Configura el editor con las propiedades Esteticas
            editor.Margin = new Thickness(0, 0, 0, 0);
            editor.Options.ConvertTabsToSpaces = true; // Usar espacios en lugar de tabulaciones
            editor.Options.IndentationSize = 4;
            


            editor.Background = (Brush)Application.Current.Resources["EditorFillColor"];
            editor.Foreground = (Brush)Application.Current.Resources["EditorTextColor"];
            editor.LineNumbersForeground = (Brush)Application.Current.Resources["EditorLineNumberColor"];
            editor.TextArea.SelectionBrush = (Brush)Application.Current.Resources["EditorSelectionColor"];  // Cambia "LightBlue" al color que prefieras.                                                                                                                       // Elimina el borde de la selección
            editor.TextArea.SelectionBorder = null; //quita borde al selecionar algo
            editor.TextArea.SelectionForeground = null; // quita color de texto al selecionar

            // Cargar el tamaño de fuente desde la configuración
            double fontSize = Properties.Settings.Default.EditorFontSize;
            editor.FontSize = fontSize;

            // Cargar el tipo de fuente desde la configuración
            string fuenteSeleccionada = Properties.Settings.Default.EditorFont;
            if (Application.Current.Resources.Contains(fuenteSeleccionada))
            {
                FontFamily fuente = (FontFamily)Application.Current.FindResource(fuenteSeleccionada);
                editor.FontFamily = fuente;
            }

            editor.ShowLineNumbers = true;

            //==============================================================
            // Obtener el ScrollViewer desde el TextView
            // Desabilitamos Scroll
            editor.WordWrap = false;
            editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible; // Asegura que el scroll horizontal sea visible
            editor.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;     // Ajusta el scroll vertical según el contenido
                                                                                  // Obtener el ancho de la línea más larga

            //============================================================

            editor.KeyDown += CodeEditor1_KeyDown; // Para usar comandos con teclas 

            //============================================================
            //[1.1] Esto es PARA MENU EN EDITOR copiar pegar cortar etc.
            Funcion_MenuContent(editor); // Menu flotante en editor cut copy paste

            //==========================================
            //[2] Panel hoover al pasar mouse encima de Comandos
            Funcion_PanelHoover(editor); // Muestra panel hoover

            // ==============================
            // Ancho de Numeracion del editor
            foreach (var margin in editor.TextArea.LeftMargins)
            {
                if (margin is LineNumberMargin lineNumberMargin)
                {

                    lineNumberMargin.Width = 50; // Ajusta este número a tu gusto
                    break;
                }
            }

            //==========================================
            // Cargar Colores ============
            LoadSyntaxHighlighting(editor); // aplica Syntasix color

            //==========================================
            // Identacion de Editor
            editor.TextArea.IndentationStrategy = new MyIndentationStrategy();

            //==========================================
            //  REPLEGAR  ===========
            var foldingManager = FoldingManager.Install(editor.TextArea);
            var foldingStrategy = new MyFoldingStrategy(editor);

            editor.TextArea.Document.Changed += (sender, e) =>
            {
                foldingStrategy.UpdateFoldings(foldingManager, editor.Document);
            };

            //==========================================
            //AUTO COMPLETADO ====================
            // Agrega los manejadores de eventos para TextEntering y TextEntered
            editor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            //==========================================
            //PARA PINTAR LINEA AL ESCRIBIR ================ EVENTO

            Funcion_LineaMarcador(editor); // Resaltar linea

            //==========================================
            //PARA GUIDE LINE ALCANCE DE CODIGO  =============== EVENTO
            Funcion_LineaRango(editor);// Linea de Alcance

            //==========================================
            //PARA ACTIVAR O DESACTIVAR CUT Y COPY =============== EVENTO
            // Inicializa las variables de la selección anterior
            previousSelectionStart = editor.SelectionStart;
            previousSelectionLength = editor.SelectionLength;

            //==========================================


            //==========================================
            //PARA MOSTRAR REFERENCIAS DE UNA SELECCION =============== EVENTO
            // Instala el resaltador de palabras en el TextArea
            _wordHighlighter = new WordHighlighter(editor);
            editor.TextArea.TextView.BackgroundRenderers.Add(_wordHighlighter);

            //==========================================
            //      AGREGADO 2024 - II
            // [1]Resaltado básico de pares de llaves/corchetes
            Funcion_Resaltarllaves(editor); // Resaltar llaves
            //==========================================
            // [2] Pinta de colores diferentes a las llaves
            FillLavesAperurasYcierres(editor);// Pinta llaves

            //==========================================
            // [3] Agregar el evento TextChanged para condicion de modificacion y mostrar **
            editor.TextChanged += TextEditor_TextChanged;

            //==========================================
            // [4] Agregar el evento actualizar minimapa altura
            editor.SizeChanged += Editor_SizeChanged;

        }

        //=========================================================================================
        //
        //             PARA CREAR ABRIR GUARDAR GUARDAR COMO TODO, IMPRIMIR
        //
        //=========================================================================================
        // Define Crear nuevo
 
        // Método para crear una nueva pestaña
        private void NuevoButton_Click(object sender, RoutedEventArgs e)
        {
            CrearPestana();
        }

        private void AbrirButton_Click(object sender, RoutedEventArgs e)
        {
            FuncionAbrir();
        }

        private void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            FuncionGuardar();
        }

        private void GuardarComoButton_Click(object sender, RoutedEventArgs e)
        {
            FuncionGuardarComo();
        }

        private void GuardarTodoButton_Click(object sender, RoutedEventArgs e)
        {
            FuncionGuardarTodo();
        }





        private void CrearPestana(string? filePath = null)
        {
            // Verificar si el archivo ya está abierto (si tiene una ruta válida)
            if (!string.IsNullOrEmpty(filePath))
            {
                foreach (TabItem tab in Tabs)
                {
                    if (tab.Tag is TabFileData fileData && fileData.FilePath == filePath)
                    {
                        tabControl.SelectedItem = tab; // Selecciona la pestaña si ya está abierta
                        return;
                    }
                }
            }

            // Configurar nombre de la pestaña
            string header = filePath != null
                ? System.IO.Path.GetFileName(filePath) // Nombre del archivo
                : $"Nuevo {GetNextAvailableTabNumber()}"; // Nombre único para una nueva pestaña

            // Crear un nuevo TabItem y configurar el editor
            TabItem newTab = new TabItem { Header = header };
            TextEditor newEditor = new TextEditor();
            ConfigureEditor(newEditor);

            // Si se proporciona un archivo, cargar su contenido
            if (!string.IsNullOrEmpty(filePath))
            {
                newEditor.Text = IndentarCodigo(HpprgmExtractor.ExtractLegibleText(filePath),"    ");
                
            }

            // Guardar la información en el Tag
            newTab.Content = newEditor;
            newTab.Tag = new TabFileData
            {
                FilePath = filePath,
                Header = header,
                IsModified = false
            };

            // Añadir la pestaña al TabControl y seleccionarla
            Tabs.Add(newTab);
            tabControl.SelectedItem = newTab;

        }

        private void CopyClearAndPaste(TextEditor editor)
        {
            try
            {
                // Obtener el texto actual del editor
                string currentText = editor.Text;

                // Intentar copiar el texto al portapapeles
                if (!TrySetClipboardText(currentText))
                {
                    return;
                }

                // Seleccionar todo el texto en el editor
                editor.SelectAll();

                // Limpiar el editor (elimina el texto seleccionado)
                editor.Text = string.Empty;

                // Intentar pegar el texto desde el portapapeles
                if (!TryPasteClipboardText(editor))
                {
                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool TrySetClipboardText(string text, int maxRetries = 5, int delayBetweenRetries = 100)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return true; // Operación exitosa
                }
                catch (COMException)
                {
                    // Esperar antes de reintentar
                    System.Threading.Thread.Sleep(delayBetweenRetries);
                }
            }
            return false; // Falló después de varios intentos
        }

        private bool TryPasteClipboardText(TextEditor editor, int maxRetries = 5, int delayBetweenRetries = 100)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                   
                    editor.Paste();
                    return true; // Operación exitosa
                }
                catch (COMException)
                {
                    // Esperar antes de reintentar
                    System.Threading.Thread.Sleep(delayBetweenRetries);
                }
            }
            return false; // Falló después de varios intentos
        }

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


        private void FuncionAbrir()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "HP Prime Program Files (*.hpprgm)|*.hpprgm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CrearPestana(filePath: openFileDialog.FileName);
            }

        }


        private void FuncionGuardar() 
        {
            // Obtener la pestaña activa
            TabItem activeTab = (TabItem)tabControl.SelectedItem;
            if (activeTab == null) return;

            // Obtener el editor y la información del archivo de la pestaña activa
            TextEditor editor = activeTab.Content as TextEditor;
            TabFileData fileData = activeTab.Tag as TabFileData;

            // Si no hay un editor o los datos del archivo, no hacer nada
            if (editor == null || fileData == null) return;

            // Verificar si el archivo ha sido modificado
            if (!fileData.IsModified)
            {
                // Si no hay modificaciones, opcionalmente puedes mostrar un mensaje al usuario
                return;
            }

            // Si es un archivo nuevo, preguntar por la ubicación para guardar (Save As)
            if (string.IsNullOrEmpty(fileData.FilePath))
            {
                SaveAsFile(editor, fileData);
            }
            else
            {
                // Si el archivo ya tiene una ruta, solo guardarlo
                SaveFile(editor, fileData);
            }

        }

        private void FuncionGuardarComo()
        {
            // Obtener la pestaña activa
            TabItem activeTab = (TabItem)tabControl.SelectedItem;
            if (activeTab == null) return;

            // Obtener el editor y la información del archivo de la pestaña activa
            TextEditor editor = activeTab.Content as TextEditor;
            TabFileData fileData = activeTab.Tag as TabFileData;

            // Si no hay editor o datos de archivo, no hacer nada
            if (editor == null || fileData == null) return;

            // Abrir cuadro de diálogo para guardar como
            SaveAsFile(editor, fileData);

        }

        private void FuncionGuardarTodo()
        {
            foreach (TabItem tabItem in tabControl.Items)
            {
                // Obtener editor y datos del archivo de cada pestaña
                TextEditor editor = tabItem.Content as TextEditor;
                TabFileData fileData = tabItem.Tag as TabFileData;

                // Si no hay editor o datos de archivo, pasar a la siguiente pestaña
                if (editor == null || fileData == null) continue;

                // Guardar solo si está modificado
                if (fileData.IsModified)
                {
                    if (string.IsNullOrEmpty(fileData.FilePath))
                    {
                        // Si el archivo no tiene ruta, usar "Guardar Como"
                        SaveAsFile(editor, fileData);
                    }
                    else
                    {
                        // Si ya tiene ruta, guardar directamente
                        SaveFile(editor, fileData);
                    }
                }
            }

        }


        // Método para guardar el archivo de la pestaña activa
        // Guarda el archivo existente
        private void SaveFile(TextEditor editor, TabFileData fileData)
        {
            if (fileData.FilePath == null) return;

            // Escribe el texto en el archivo
            File.WriteAllText(fileData.FilePath, editor.Text);

            // Marca como no modificado
            fileData.IsModified = false;

            // Actualiza el nombre del archivo en el encabezado de la pestaña
            fileData.Header = System.IO.Path.GetFileName(fileData.FilePath); // Establece el encabezado con el nombre del archivo

            // Elimina el asterisco del encabezado si es necesario
            if (fileData.Header.EndsWith("*"))
            {
                fileData.Header = fileData.Header.TrimEnd('*');
            }

            // Actualiza el nombre de la pestaña
            TabItem tabItem = Tabs.FirstOrDefault(t => t.Tag == fileData);
            if (tabItem != null)
            {
                tabItem.Header = fileData.Header;
            }
        }

        // Abre un cuadro de diálogo para guardar como un nuevo archivo
        private bool SaveAsFile(TextEditor editor, TabFileData fileData)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "HP Prime Program Files (*.hpprgm)|*.hpprgm";

            // Establecer el nombre predeterminado en el cuadro de diálogo como el nombre de la pestaña
            if (!string.IsNullOrEmpty(fileData.Header) && fileData.Header.StartsWith("Nuevo"))
            {
                // Solo establecer el nombre si la pestaña es nueva y no tiene un archivo guardado
                saveFileDialog.FileName = fileData.Header.Replace("*", "");  // Establece el nombre de la pestaña sin el asterisco
            }

            if (saveFileDialog.ShowDialog() == true)
            {
                // Antes de guardar, libera el número de la pestaña del conjunto usedTabs
                string originalHeader = fileData.Header;
                if (originalHeader.StartsWith("Nuevo"))
                {
                    int? tabNumber = GetTabNumberFromHeader(originalHeader);
                    if (tabNumber.HasValue)
                    {
                        usedTabs.Remove(tabNumber.Value); // Liberar el número de la pestaña antigua
                    }
                }

                // Guarda el archivo con el nuevo nombre
                fileData.FilePath = saveFileDialog.FileName;
                File.WriteAllText(fileData.FilePath, editor.Text);

                // Marca como no modificado
                fileData.IsModified = false;

                // Actualiza el encabezado con el nombre del archivo (sin el asterisco)
                fileData.Header = System.IO.Path.GetFileName(fileData.FilePath); // Nombre del archivo
                if (fileData.Header.EndsWith("*"))
                {
                    fileData.Header = fileData.Header.TrimEnd('*');
                }

                // Asigna el número de la pestaña actual al conjunto usedTabs
                int? newTabNumber = GetTabNumberFromHeader(fileData.Header);
                if (newTabNumber.HasValue)
                {
                    usedTabs.Add(newTabNumber.Value); // Marca el número como utilizado
                }

                // Actualiza el nombre de la pestaña
                TabItem tabItem = Tabs.FirstOrDefault(t => t.Tag == fileData);
                if (tabItem != null)
                {
                    tabItem.Header = fileData.Header;
                }

                return true; // Guardado completado
            }

            return false; // Guardado cancelado
        }

        
        //BOTON IMPRIMIR
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Crear un FlowDocument a partir del contenido del editor
            FlowDocument flowDocument = DocumentPrinter.CreateFlowDocumentForEditor(editor);

            // Mostrar el cuadro de diálogo de impresión
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Crear un contenedor para el FlowDocument
                IDocumentPaginatorSource paginator = flowDocument;

                // Enviar el documento a la impresora
                printDialog.PrintDocument(paginator.DocumentPaginator, "Impresión del editor");
            }
        }
        





        //=========================================================================================
        //
        //            PARA FUNCION DE CERRAR PESTAÑAS
        //
        //=========================================================================================
        // Conjunto para llevar el registro de los números de pestañas ya utilizadas
        private HashSet<int> usedTabs = new HashSet<int>();
        private int GetNextAvailableTabNumber()
        {
            // Comienza en 1 y busca el primer número que no esté en usedTabs
            int nextTabNumber = 1;
            while (usedTabs.Contains(nextTabNumber))
            {
                nextTabNumber++;  // Aumenta el número hasta encontrar uno libre
            }
            usedTabs.Add(nextTabNumber); // Marca el número como usado
            return nextTabNumber;
        }


        // Método para obtener el número de la pestaña desde el Header
        private int? GetTabNumberFromHeader(string header)
        {
            // Eliminar el asterisco (si existe)
            header = header.Replace("*", "").Trim();

            // Verificar si el encabezado tiene el formato exacto "Nuevo X"
            if (header.StartsWith("Nuevo ") && header.Length > 6) // "Nuevo " tiene 6 caracteres
            {
                string numberPart = header.Substring(6); // Extraemos todo después de "Nuevo "

                // Verificar si la parte extraída es un número (sin ninguna extensión)
                if (int.TryParse(numberPart, out int tabNumber))
                {
                    // Verificar si el número extraído tiene una extensión como ".hpcode"
                    if (!header.Contains("."))
                    {
                        return tabNumber; // Si no contiene extensión, devolver el número
                    }
                }
            }

            return null; // No es un formato válido
        }



        // Define the CloseTabCommand
        public ICommand CloseTabCommand { get; private set; }

        private async void CloseTab(object parameter)
        {
            if (parameter is not TabItem tabToClose) return;

            // Obtener el editor y los datos del archivo
            TextEditor editor = tabToClose.Content as TextEditor;
            TabFileData fileData = tabToClose.Tag as TabFileData;

            if (editor == null || fileData == null) return;

            // Verificar si es un archivo nuevo o tiene cambios sin guardar
            bool isNewFile = string.IsNullOrEmpty(fileData.FilePath);
            bool hasUnsavedChanges = fileData.IsModified;

            // Si es un archivo nuevo y no tiene cambios, cerrar sin preguntar
            if (isNewFile && string.IsNullOrEmpty(editor.Text))
            {
                // Liberar el número de la pestaña (sin cambios)
                LiberarNumeroPestana(tabToClose);

                Tabs.Remove(tabToClose); // Eliminar la pestaña
                return;
            }

            // Si hay cambios sin guardar, mostrar el diálogo de confirmación
            if (hasUnsavedChanges)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Guardar",
                    Content = "¿Deseas guardar los cambios antes de cerrar la pestaña?",
                    PrimaryButtonText = "Sí",
                    SecondaryButtonText = "No",
                    CloseButtonText = "Cancelar",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var result = await uiMessageBox.ShowDialogAsync();

                if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                {
                    // Guardar el archivo
                    if (isNewFile)
                    {
                        if (!SaveAsFile(editor, fileData)) return; // Cancelar si no se guarda
                    }
                    else
                    {
                        SaveFile(editor, fileData);
                    }
                }
                else if (result == Wpf.Ui.Controls.MessageBoxResult.None)
                {
                    return; // Cancelar cierre
                }
            }

            // Liberar el número de la pestaña (incluso si tiene cambios no guardados)
            LiberarNumeroPestana(tabToClose);

            // Finalmente, elimina la pestaña de la colección
            Tabs.Remove(tabToClose);
        }


        private void LiberarNumeroPestana(TabItem tabToClose)
        {
            string tabHeader = tabToClose.Header.ToString();

            // Eliminar el asterisco (si existe) y extraer el número de la pestaña
            tabHeader = tabHeader.Replace("*", "").Trim();

            int? tabNumber = GetTabNumberFromHeader(tabHeader);
            if (tabNumber.HasValue)
            {
                usedTabs.Remove(tabNumber.Value); // Liberar el número de la pestaña
            }
        }





        //=========================================================================================
        //
        //            PARA PANEL CONFIGURACION SETTINGS DE EDITOR
        //
        //=========================================================================================
        //Selecion de Fuente
        private void ComboBoxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtén el nombre de la fuente seleccionada
            var selectedFontName = ((ComboBoxItem)(sender as ComboBox).SelectedItem).Content.ToString();

            if (Application.Current.Resources.Contains(selectedFontName))
            {
                // Encuentra y asigna la fuente desde los recursos
                FontFamily selectedFont = (FontFamily)Application.Current.FindResource(selectedFontName);

                // Aplica la fuente a todos los editores
                ApplyFontToAllEditors(selectedFont);

                // Guarda la selección en la configuración
                Properties.Settings.Default.EditorFont = selectedFontName;
                Properties.Settings.Default.Save();
            }
        }

        private void ApplyFontToAllEditors(FontFamily selectedFont)
        {
            foreach (TabItem tab in Tabs)
            {
                // Verifica si el contenido de la pestaña es un TextEditor
                if (tab.Content is TextEditor editor)
                {
                    // Asigna la fuente seleccionada al editor de la pestaña
                    editor.FontFamily = selectedFont;
                }
            }
        }

        //Selecion de tamaño
        private void ComboBoxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxFontSize.SelectedItem is ComboBoxItem selectedItem)
            {
                // Obtener el tamaño de fuente seleccionado
                if (double.TryParse(selectedItem.Content.ToString(), out double selectedFontSize))
                {
                    // Aplicar el tamaño de fuente a todos los editores
                    ApplyFontSizeToAllEditors(selectedFontSize);

                    // Guardar el tamaño de fuente en la configuración
                    Properties.Settings.Default.EditorFontSize = selectedFontSize;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void ApplyFontSizeToAllEditors(double fontSize)
        {
            foreach (TabItem tab in Tabs)
            {
                // Verifica si el contenido de la pestaña es un TextEditor
                if (tab.Content is TextEditor editor)
                {
                    // Asigna el tamaño de fuente seleccionado al editor
                    editor.FontSize = fontSize;
                }
            }
        }

        //Selecion de Idioma
        private void ComboBoxIdioma_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtén el ComboBox.
            var comboBox = sender as ComboBox;

            // Obtén el elemento seleccionado.
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            // Comprueba el contenido del elemento seleccionado.
            if (selectedItem.Content.ToString() == FindResource("es_Es").ToString())
            {
                // Si el elemento seleccionado es "es_Es", haz algo.
                IdiomaSelected = 0;
                Properties.Settings.Default.IdiomaSettings = IdiomaSelected; // Almacena valor en Settings
                Properties.Settings.Default.Save();

                ((App)Application.Current).ChangeLanguage(FindResource("es_Es").ToString());
            }
            else if (selectedItem.Content.ToString() == FindResource("en_En").ToString())
            {
                // Si el elemento seleccionado es "en_En", haz algo diferente.
                IdiomaSelected = 1;
                Properties.Settings.Default.IdiomaSettings = IdiomaSelected; //Almacena valor en Settings
                Properties.Settings.Default.Save();

                ((App)Application.Current).ChangeLanguage(FindResource("en_En").ToString());
            }
            // Fuerza un redibujado de la ventana para reflejar los cambios
            var currentWindow = Window.GetWindow(this);
            if (currentWindow != null)
            {
                currentWindow.DataContext = null;
                currentWindow.DataContext = this;
            }
        }

        //Selecion de Thema
        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox != null)
            {
                // Obtener el índice del elemento seleccionado
                int selectedIndex = comboBox.SelectedIndex;

                // Realizar acciones basadas en el índice
                switch (selectedIndex)
                {
                    case 0:
                        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(ApplicationTheme.Light);

                        Properties.Settings.Default.TemaSettings = 0; //Almacena valor en Settings
                                                                      //Properties.Settings.Default.TransparenciaSettings = 0; //Almacena valor en Settings
                        Properties.Settings.Default.Save();

                        ((App)Application.Current).ChangeTheme("Light");

                        //UpdateButtons();
                        //Repintado de editor manualmente
                        UpdateEditorsTheme();
                        break;
                    case 1:
                        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(ApplicationTheme.Dark);

                        Properties.Settings.Default.TemaSettings = 1; //Almacena valor en Settings
                                                                      //Properties.Settings.Default.TransparenciaSettings = 1; //Almacena valor en Settings
                        Properties.Settings.Default.Save();


                        ((App)Application.Current).ChangeTheme("Dark");

                        //UpdateButtons();
                        UpdateEditorsTheme();
                        break;
                    default:
                        break;
                }
            }
        }



        private void UpdateEditorsTheme()
        {
            foreach (var item in tabControl.Items) // Asegúrate de usar el nombre correcto de tu TabControl
            {
                if (item is TabItem tabItem && tabItem.Content is TextEditor editor)
                {
                    // Aplica los nuevos colores desde los recursos
                    editor.Background = (Brush)Application.Current.Resources["EditorFillColor"];
                    editor.Foreground = (Brush)Application.Current.Resources["EditorTextColor"];
                    editor.LineNumbersForeground = (Brush)Application.Current.Resources["EditorLineNumberColor"];
                    editor.TextArea.SelectionBrush = (Brush)Application.Current.Resources["EditorSelectionColor"];  // Cambia "LightBlue" al color que prefieras.                                                                                                                       // Elimina el borde de la selección
                    editor.TextArea.SelectionBorder = null;
                    editor.TextArea.SelectionForeground = null;
                    

                    LoadSyntaxHighlighting(editor); // aplica Syntasix color
                    FillLavesAperurasYcierres(editor);// Pinta llaves
                    Funcion_LineaRango(editor);// Linea de Alcance
                    Funcion_LineaMarcador(editor); // Resaltar linea 
                    Funcion_Resaltarllaves(editor); // Resaltar llaves
                    Funcion_PanelHoover(editor); // Muestra panel hoover
                    Funcion_MenuContent(editor); // Menu flotante en editor cut copy paste

                }
            }
        }



        // Funciones De Editor
        public void LoadSyntaxHighlighting(TextEditor actualEditor)
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

        private void FillLavesAperurasYcierres(TextEditor actualEditor) 
        {
            double ThemeSelec = Properties.Settings.Default.TemaSettings;
            BracketColorizer bracketColorizer = new BracketColorizer(ThemeSelec);
            actualEditor.TextArea.TextView.LineTransformers.Add(bracketColorizer);
        } // carga colores de llaves

        private void Funcion_LineaRango(TextEditor actualEditor) 
        {
            actualEditor.TextArea.TextView.BackgroundRenderers.Add(new BlockScopeRenderer(actualEditor));
            actualEditor.TextArea.TextView.BackgroundRenderers.Add(new BlockHighlighter(actualEditor));

            
        }// Indentation rango de coddigo

        private void Funcion_LineaMarcador(TextEditor actualEditor)
        {
            var renderer = new HighlightCurrentLineBackgroundRenderer(actualEditor, minimap);
            actualEditor.TextArea.TextView.BackgroundRenderers.Add(renderer);
        }// Resaltar linea con marcador

        private void Funcion_Resaltarllaves(TextEditor actualEditor)
        {
            BracketHighlighter highlighter = new BracketHighlighter(actualEditor);
            actualEditor.TextArea.TextView.BackgroundRenderers.Add(highlighter);
        }// Resaltar apertura de llaves

        private void Funcion_PanelHoover(TextEditor actualEditor)
        {
            actualEditor.TextArea.TextView.MouseMove += TextView_MouseMove; // funcion ver hover  popup
            actualEditor.PreviewMouseDown += Editor_PreviewMouseDown; // Cerrar popup an hacer click en el editor

            actualEditor.MouseLeave += (sender, e) => // oculta hover popup si se sale del editor
            {
                // Cerrar el Popup cuando el mouse sale de codeEditor1
                CloseTooltip();
            };
        }// Mostrar panel hoover de comandos


        private void Funcion_MenuContent(TextEditor actualEditor) 
        {
            // Configura el ContextMenu
            actualEditor.ContextMenu = new ContextMenu();
            //actualEditor.ContextMenu.Width = 400;
            FontFamily fuente = (FontFamily)Application.Current.FindResource("SegoeFluentIcons");

            // Crea los elementos del menú
            MenuItem menuItemCut = new MenuItem() { Header = "Cortar", InputGestureText = "Ctrl + X", Style = FindResource("StyleMenuItem") as Style };
            menuItemCut.Icon = new System.Windows.Controls.TextBlock { Text = "\ue16b", FontFamily = fuente, FontSize = 18 };
            menuItemCut.Click += CortarButton_Click;

            MenuItem menuItemCopy = new MenuItem() { Header = "Copiar", InputGestureText = "Ctrl + C", Style = FindResource("StyleMenuItem") as Style };
            menuItemCopy.Icon = new System.Windows.Controls.TextBlock { Text = "\ue16f", FontFamily = fuente, FontSize = 18 };
            menuItemCopy.Click += CopiarButton_Click;

            MenuItem menuItemPaste = new MenuItem() { Header = "Pegar", InputGestureText = "Ctrl + V", Style = FindResource("StyleMenuItem") as Style };
            menuItemPaste.Icon = new System.Windows.Controls.TextBlock { Text = "\ue16d", FontFamily = fuente, FontSize = 18 };
            menuItemPaste.Click += PasteButton_Click;

            MenuItem menuItemDelete = new MenuItem() { Header = "Eliminar", InputGestureText = "Delete", Style = FindResource("StyleMenuItem") as Style };
            menuItemDelete.Icon = new System.Windows.Controls.TextBlock { Text = "\ued62", FontFamily = fuente, FontSize = 18 };
            menuItemDelete.Click += BorrarButton_Click;

            MenuItem menuItemSelectAll = new MenuItem() { Header = "Selecionar Todo", InputGestureText = "Ctrl + A", Style = FindResource("StyleMenuItem") as Style };
            menuItemSelectAll.Icon = new System.Windows.Controls.TextBlock { Text = "\uef20", FontFamily = fuente, FontSize = 18 };
            menuItemSelectAll.Click += SelectAllButton_Click;

            // Agrega los elementos al ContextMenu
            actualEditor.ContextMenu.Items.Add(menuItemCut);
            actualEditor.ContextMenu.Items.Add(menuItemCopy);
            actualEditor.ContextMenu.Items.Add(menuItemPaste);
            actualEditor.ContextMenu.Items.Add(menuItemDelete);
            actualEditor.ContextMenu.Items.Add(new Separator());
            actualEditor.ContextMenu.Items.Add(menuItemSelectAll);

        }















        //********************************************************************************************

        //=========================================================================================
        //
        //    ******** UPPER            PARA GUARDAR DATOS Y CARGARLOS AL ABRIRI PROGRAMA OTRA VEZ
        //
        //=========================================================================================
        //********************************************************************************************

        //Evento guardar pestañas al cerrar programa

        private void SaveTabsState()
        {
            // Crear una lista para almacenar el estado de las pestañas
            var savedTabs = Tabs.Select(tab =>
            {
                if (tab.Content is TextEditor editor && tab.Tag is TabFileData fileData)
                {
                    return new TabFileData
                    {
                        Header = tab.Header.ToString(),
                        FilePath = fileData.FilePath,
                        Text = editor.Text,
                        IsModified = fileData.IsModified,
                        CaretLine = editor.TextArea.Caret.Line, // Guardar línea del cursor
                        CaretColumn = editor.TextArea.Caret.Column // Guardar columna del cursor
                    };
                }
                return null;
            }).Where(data => data != null).ToList(); // Excluir pestañas sin datos

            // Serializar y guardar en un archivo JSON
            string json = JsonConvert.SerializeObject(savedTabs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("TabsState.json", json);

            // Guardar el índice de la pestaña activa en otro archivo o como parte del estado
            File.WriteAllText("ActiveTabIndex.json", tabControl.SelectedIndex.ToString());
        }

        // Evento cargar pestañas Al abrir programa
        private void LoadTabsState()
        {
            if (File.Exists("TabsState.json"))
            {
                string json = File.ReadAllText("TabsState.json");
                List<TabFileData> savedTabs = JsonConvert.DeserializeObject<List<TabFileData>>(json);

                usedTabs.Clear(); // Reinicia el conjunto de números utilizados

                foreach (var fileData in savedTabs)
                {
                    TabItem restoredTab = new TabItem { Header = fileData.Header };

                    TextEditor restoredEditor = new TextEditor();
                    ConfigureEditor(restoredEditor);
                    restoredEditor.Text = fileData.Text;

                    // Establecer el Header de la pestaña
                    restoredTab.Header = fileData.Header;

                    // Restaurar caret y desplazamiento al abrir la pestaña
                    restoredEditor.Loaded += (sender, e) =>
                    {
                        if (restoredTab.Tag is TabFileData data && !data.IsInitialized)
                        {
                            restoredEditor.TextArea.Caret.Line = data.CaretLine;
                            restoredEditor.TextArea.Caret.Column = data.CaretColumn;
                            restoredEditor.ScrollTo(data.CaretLine, data.CaretColumn);

                            data.IsInitialized = true; // Marcar como inicializada
                        }
                    };

                    // Asignar los datos al Tag
                    fileData.IsInitialized = false; // Inicialmente no está inicializada
                    restoredTab.Content = restoredEditor;
                    restoredTab.Tag = fileData;

                    Tabs.Add(restoredTab);

                    // Si el nombre de la pestaña es "Nuevo X", agrega X a `usedTabs`
                    if (fileData.Header.StartsWith("Nuevo"))
                    {
                        string[] parts = fileData.Header.Split(' ');
                        if (int.TryParse(parts[1], out int tabNumber))
                        {
                            usedTabs.Add(tabNumber);
                        }
                    }
                }

                // Restaurar la pestaña activa
                if (File.Exists("ActiveTabIndex.json"))
                {
                    string activeTabIndexStr = File.ReadAllText("ActiveTabIndex.json");
                    if (int.TryParse(activeTabIndexStr, out int activeTabIndex) &&
                        activeTabIndex >= 0 && activeTabIndex < Tabs.Count)
                    {


                        tabControl.SelectedIndex = activeTabIndex;


                        // Asegurar el foco en el editor de la pestaña activa
                        if (Tabs[activeTabIndex].Content is TextEditor activeEditor)
                        {

                            activeEditor.Focus();

                            RedrawMinimap();
                        }



                    }
                }


            }
        }


        //Evento actualizar TabItem si se modifica texto o no *
        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            TextEditor editor = (TextEditor)sender;
            TabItem tabItem = editor.Parent as TabItem;
            if (tabItem != null && tabItem.Tag is TabFileData fileData)
            {
                // Verificar si la pestaña ya tiene un asterisco
                if (!fileData.Header.EndsWith("*"))
                {
                    // Agregar el asterisco al nombre de la pestaña
                    fileData.Header += "*";
                    tabItem.Header = fileData.Header;
                }

                // Marcar la pestaña como modificada
                fileData.IsModified = true;
            }


        }




        //********************************************************************************************

        //=========================================================================================
        //
        //    ******** FUNCION PRUEBA PARA AUTOCOMPLETATION
        //
        //=========================================================================================
        //********************************************************************************************













        /*



        //==========================================

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int VK_CONTROL = 0x11;
        public const int VK_V = 0x56;





        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, Int16 nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        const uint WM_KEYDOWN = 0x100, WM_KEYUP = 0x0101, WM_CHAR = 0x0102, WM_PASTE = 0x0302, WM_APPCOMMAND = 0x0319,
            APPCOMMAND_PASTE = 38, WM_SYSKEYDOWN = 0x0104, WM_SYSKEYUP = 0x0105, WM_COMMAND = 0x0111;
        const string processName = "HPPrime";
        private Random _random = new Random();
        private static string _commandsFile;


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si el archivo es nuevo o ya existente
            if (currentFile == null)
            {
                // Si el archivo es nuevo, solicitar al usuario que lo guarde antes de continuar
                SaveFile();
            }
            else
            {
                // Si el archivo ya existe, guardar las modificaciones automáticamente
                File.WriteAllText(currentFile, codeEditor1.Text);
                lastSavedContent = codeEditor1.Text;
            }

            // Crear una nueva ventana de progreso
            ProgresoUI progressWindow = new ProgresoUI();

            // Mostrar la ventana de progreso
            progressWindow.Show();

            // Obtén el texto del editor
            codeEditor1.SelectAll();

            // Copiar el texto seleccionado al portapapeles
            string selectedText = codeEditor1.SelectedText;
            string textWithSpaces = selectedText.Replace("\t", "    "); // Reemplaza cada tabulación con cuatro espacios

            bool clipboardSet = false;
            while (!clipboardSet)
            {
                try
                {
                    Clipboard.SetText(textWithSpaces);
                    clipboardSet = true;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    // Espera 100 ms antes de intentarlo de nuevo
                    await Task.Delay(100);
                }
            }
            codeEditor1.Select(0, 0); // deseleccionar

            // Iniciar una nueva tarea para actualizar la ventana de progreso
            await Task.Run(() =>
            {
                // Actualizar la ventana de progreso
                progressWindow.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateProgress(10); // Ajusta el valor según tu necesidad
                });

                // Obtén el identificador de la ventana del emulador
                IntPtr emulator;
                emulator = EmuladorEval(); //  cierra y abre emulador

                // Actualizar la ventana de progreso
                progressWindow.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateProgress(20); // Ajusta el valor según tu necesidad
                });

                // Nombre del archivo Crear
                string NameArchive = System.IO.Path.GetFileNameWithoutExtension(currentFile) + ".hpprgm";

                Utilidades.CreateOrReplaceArchive(NameArchive);

                // Enviar la tecla al emulador
                SendKeyToWindow(emulator, (IntPtr)KeyInterop.VirtualKeyFromKey(Key.LeftCtrl), char.MinValue);

                SendKeyToWindow(emulator, (IntPtr)KeyInterop.VirtualKeyFromKey(Key.D1), char.MinValue);

                //SendKeyToWindow(emulator, (IntPtr)KeyInterop.VirtualKeyFromKey(Key.LeftCtrl), char.MinValue, false, true);

                emulator = EmuladorEval(); //  cierra y abre emulador

                // Actualizar la ventana de progreso
                progressWindow.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateProgress(60); // Ajusta el valor según tu necesidad
                });

                string key = NameArchive.Replace(".hpprgm", "");

                foreach (var c in key)
                    SendKeyToWindow(emulator, IntPtr.Zero, c);

                SendKeyToWindow(emulator, (IntPtr)KeyInterop.VirtualKeyFromKey(Key.Enter), char.MinValue);

                keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
                Thread.Sleep(10);
                // Simular la pulsación de la tecla V
                keybd_event(VK_V, 0, KEYEVENTF_EXTENDEDKEY, 0);
                Thread.Sleep(10);
                // Simular la liberación de la tecla V
                keybd_event(VK_V, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
                Thread.Sleep(10);
                // Simular la liberación de la tecla Ctrl
                keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);

                Thread.Sleep(200);

                // Actualizar la ventana de progreso
                progressWindow.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateProgress(100); // Ajusta el valor según tu necesidad
                });

                Thread.Sleep(300);
            });



            // Cerrar la ventana de progreso cuando se complete la operación
            progressWindow.Close();
        }


        private static void SendKeyToWindow(IntPtr emulator, IntPtr key, char character, bool keyDown = true, bool keyUp = true)
        {
            // Trae la ventana del emulador al frente
            //SetForegroundWindow(emulator);

            if (key == IntPtr.Zero)
            {
                PostMessage(emulator, WM_CHAR, (IntPtr)character, IntPtr.Zero);
                Thread.Sleep(0);
            }
            else
            {
                if (keyDown)
                    PostMessage(emulator, WM_KEYDOWN, key, IntPtr.Zero);
                Thread.Sleep(20);
                if (keyUp)
                    PostMessage(emulator, WM_KEYUP, key, IntPtr.Zero);


            }
        }


        private static IntPtr EmuladorEval()
        {
            // Comprobar si el emulador está en ejecución
            var emulatorProcess = Process.GetProcessesByName(processName).FirstOrDefault();

            // Si el emulador está en ejecución, cerrarlo
            if (emulatorProcess != null)
            {
                emulatorProcess.CloseMainWindow();
                emulatorProcess.WaitForExit();
            }

            // Iniciar el emulador
            emulatorProcess = Process.Start(@"C:\Program Files\HP\HP Prime Virtual Calculator\HPPrime.exe");
            emulatorProcess.WaitForInputIdle();

            emulatorProcess.WaitForInputIdle();

            // Obtén el identificador de la ventana del emulador
            IntPtr emulator = emulatorProcess.MainWindowHandle;

            return emulator;

        }

        */








    }
}