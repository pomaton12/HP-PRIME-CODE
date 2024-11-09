using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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


// ...



namespace HP_PRIME_CODE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //Espacio en condicionales
    public class MyIndentationStrategy : IIndentationStrategy
    {
        public void IndentLine(TextDocument document, DocumentLine line)
        {
            var lines = document.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            var indentation = "\t";
            var openedBlock = Refactoring.FormatLines(ref lines, indentation);

            if (line.LineNumber <= lines.Count)
            {
                var indentations = lines[line.LineNumber - 1].TakeWhile(char.IsWhiteSpace).Count() / indentation.Length;
                var newIndentation = new string('\t', indentations);

                var lineText = document.GetText(line);
                var lineIndentation = new string(lineText.TakeWhile(char.IsWhiteSpace).ToArray());

                if (newIndentation != lineIndentation)
                {
                    document.Replace(line.Offset, lineIndentation.Length, newIndentation);
                }
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

            for (int i = 0; i < document.LineCount; i++)
            {
                var line = document.GetLineByNumber(i + 1);
                var text = document.GetText(line).Trim().ToUpper(); // Convertir el texto a mayúsculas

                bool lineHasOpeningKeyword = text.StartsWith("BEGIN") || text.StartsWith("FOR") || text.StartsWith("WHILE") || text.StartsWith("CASE") || text.StartsWith("IFERR") || text.StartsWith("REPEAT") || (text.StartsWith("IF") && !text.EndsWith("END;"));
                bool lineHasClosingKeyword = text.StartsWith("END") || text.StartsWith("UNTIL");
                bool lineIsSingleIfEnd = text.StartsWith("IF") && text.EndsWith("END;");

                if (lineHasOpeningKeyword && !lineHasClosingKeyword)
                {
                    startOffsets.Push(line.EndOffset); // Empieza el plegado después de la primera línea
                }
                else if (lineHasClosingKeyword && !lineHasOpeningKeyword && !lineIsSingleIfEnd)
                {
                    if (startOffsets.Count > 0)
                    {
                        int startOffset = startOffsets.Pop();
                        newFoldings.Add(new NewFolding(startOffset, line.EndOffset));
                    }
                }
            }
            // Ordena newFoldings por el offset de inicio
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));

            manager.UpdateFoldings(newFoldings, -1);

            // Restaura la posición del cursor
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
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Text));
                textBlock.Inlines.Add(new Run("  " + CommandType) { Foreground = Brushes.Gray, FontStyle = FontStyles.Italic });
                return textBlock;
            }
        }

        public object Description { get { return null; } } // Devuelve siempre null

        public double Priority { get { return 0; } }

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

            // Inserta la selección de autocompletado en lugar de la última palabra
            if (this.Text == "for")
            {
                string snippet = 
                    "local i;" + 
                    Environment.NewLine + 
                    Environment.NewLine + 
                    "for i:=1 to 10 do" + 
                    Environment.NewLine + 
                    Environment.NewLine + 
                    "\t//Escriba su código aqui..." + 
                    Environment.NewLine + 
                    Environment.NewLine + 
                    "end;";
                textArea.Document.Replace(segment, snippet);
            }
            else if (this.Text == "if")
            {
                string snippet = 
                    "if /*condition*/ then" + 
                    Environment.NewLine + 
                    Environment.NewLine + 
                    "\t//Escriba su código aqui..." + 
                    Environment.NewLine + 
                    Environment.NewLine + 
                    "end;";
                textArea.Document.Replace(segment, snippet);
            }
            else if (this.Text == "while")
            {
                string snippet =
                    "while /*condition*/ do" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "\t//Escriba su código aqui..." +
                    Environment.NewLine +
                    Environment.NewLine +
                    "end;";
                textArea.Document.Replace(segment, snippet);
            }
            else if (this.Text == "EXPORT")
            {
                string snippet =
                    "EXPORT FunctionName(Parameters)" +
                    Environment.NewLine +
                    "begin" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "\t//Cambia Name por algun nombre que quieras Ejm: FunctionVIGAS()" +
                    Environment.NewLine +
                    "\t//Escriba su código aqui..." +
                    Environment.NewLine +
                    Environment.NewLine +
                    "end;";
                textArea.Document.Replace(segment, snippet);
            }
            else if (this.Text == "begin")
            {
                string snippet =
                    "begin" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "\t//crea una funcion antes del begin Ejm: FunctionVIGAS()" +
                    Environment.NewLine +
                    "\t//Escriba su código aqui..." +
                    Environment.NewLine +
                    Environment.NewLine +
                    "end;";
                textArea.Document.Replace(segment, snippet);
            }
            else
            {
                textArea.Document.Replace(segment, this.Text);
            }
        }
    }

    // Pintar linea al escribir
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private TextEditor _editor;
        private Canvas _minimap;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor, Canvas minimap)
        {
            _editor = editor;
            _minimap = minimap;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Selection; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                var pen = new Pen(new SolidColorBrush(Color.FromArgb(255, 230, 230, 242)), 1);
                drawingContext.DrawRectangle(null, pen, new Rect(new Point(rect.Location.X + textView.ScrollOffset.X, rect.Location.Y), new Size(textView.ActualWidth, rect.Height)));
            }

            // Dibujar la posición de la línea en el minimapa
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
            double top = (double)currentLineIndex / lines.Length *( _minimap.Height - 46);

            // Crea un rectángulo rojo y lo posiciona en la línea actual
            var rect = new Rectangle
            {
                Fill = Brushes.Red,
                Width = 20,
                Height = 2,
            };
            Canvas.SetTop(rect, top);
            _minimap.Children.Add(rect);
        }
    }


    // Guide lines alcance de codigo
    public class IndentationGuidesRenderer : IBackgroundRenderer
    {
        private readonly TextView _textView;

        public IndentationGuidesRenderer(TextView textView)
        {
            _textView = textView;
            _textView.ScrollOffsetChanged += (sender, e) => _textView.InvalidateLayer(Layer);
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            var pen = new Pen(Brushes.LightGray, 1)
            {
                DashStyle = DashStyles.Solid
            };
            pen.Freeze();

            var indentation = new string(' ', textView.Options.IndentationSize);
            var lines = textView.Document.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            var openedBlock = Refactoring.FormatLines(ref lines, indentation);

            textView.EnsureVisualLines();
            foreach (var visualLine in textView.VisualLines)
            {
                var lineNumber = visualLine.FirstDocumentLine.LineNumber - 1;
                if (lineNumber < lines.Count)
                {
                    var indentations = lines[lineNumber].TakeWhile(char.IsWhiteSpace).Count() / indentation.Length;
                    for (var i = 0; i < indentations; i++)
                    {
                        var guideX = Math.Round(textView.WideSpaceWidth * i * indentation.Length) - textView.ScrollOffset.X + 0.5;
                        drawingContext.DrawLine(pen, new Point(guideX, visualLine.VisualTop - textView.ScrollOffset.Y), new Point(guideX, visualLine.VisualTop + visualLine.Height - textView.ScrollOffset.Y));
                    }
                }
            }
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
    // para boton cerrar de pestañas

    public partial class MainWindow 
    {
        private Popup tooltipPopup; // para popup tooltip

        CompletionWindow completionWindow;

        // para detectar seleccion de text y activar copy o cut
        private int previousSelectionStart;
        private int previousSelectionLength;
        private DispatcherTimer selectionChangedTimer;

        // para seleccionar un texto y sus derivados
        private DispatcherTimer _highlightingTimer;
        private WordHighlighter _wordHighlighter;
        private int _previousSelectionStart;
        private int _previousSelectionLength;

        //para agregar pestañas
        private ActionTabViewModal vmd;

        public MainWindow()
        {
            InitializeComponent();

            StateChanged += MainWindow_StateChanged;// para tooltop de maximizar


            // Cargar Colores y Estylo ============

            // Registrar la definición de resaltado de sintaxis
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string xshdFilePath = System.IO.Path.Combine(appDirectory, "Estilo", "HPPPL.xshd");

            using (Stream s = File.OpenRead(xshdFilePath))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    // Carga la definición XSHD
                    IHighlightingDefinition definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                    // Registra la definición en el HighlightingManager
                    HighlightingManager.Instance.RegisterHighlighting("HPPPL", new string[] { ".hppl" }, definition); // Agrega la extensión
                }
            }


            // =========== AGregados 2024 ===============
 


            //[2]Actualizar busqueda al cambiar pestaña 
            tabControl.SelectionChanged += tabControl_SelectionChanged;

            //[3] Cerrar pestañas

        }


        private void Button_Exit(object sender, RoutedEventArgs e)
            {

                this.Close();
            }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MaximizeToolTip.Content = "Restored";
            }
            else
            {
                MaximizeToolTip.Content = "Maximized";
            }

            
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


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
            if (editor != null && editor.SelectionLength > 0)
            {
                editor.Cut();
            }
        }

        private void CopiarButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            // Si el TextEditor tiene una selección válida, copia el texto
            if (editor != null && editor.SelectionLength > 0)
            {
                editor.Copy();
            }
        }

        // Método que se ejecuta cuando el temporizador se activa
        private void SelectionChangedTimer_Tick(object sender, EventArgs e)
        {
            // Obtén el TextEditor asociado al temporizador
            TextEditor editor = (sender as DispatcherTimer).Tag as TextEditor;

            // Comprueba si la selección ha cambiado
            if (editor.SelectionLength > 0) // Verifica si hay una selección
            {
                // Actualiza las variables de la selección anterior
                previousSelectionStart = editor.SelectionStart;
                previousSelectionLength = editor.SelectionLength;

                // Habilita o deshabilita los botones de copiar y pegar en función de si hay texto seleccionado
                bool isTextSelected = editor.SelectionLength > 0;
                cbbtn_cut.IsEnabled = isTextSelected;
                cbbtn_copy.IsEnabled = isTextSelected;

                // Obtén los elementos del menú del ContextMenu
                MenuItem menuItemCut = editor.ContextMenu.Items.OfType<MenuItem>().FirstOrDefault(item => item.Header.ToString() == "Cortar (Ctrl + X)");
                MenuItem menuItemCopy = editor.ContextMenu.Items.OfType<MenuItem>().FirstOrDefault(item => item.Header.ToString() == "Copiar (Ctrl + C)");
                MenuItem menuItemBorrar = editor.ContextMenu.Items.OfType<MenuItem>().FirstOrDefault(item => item.Header.ToString() == "Borrar");

                // Habilita o deshabilita los elementos del menú
                menuItemCut.IsEnabled = isTextSelected;
                menuItemCopy.IsEnabled = isTextSelected;
                menuItemBorrar.IsEnabled = isTextSelected;

                // Cambia la opacidad de los botones en función de si están habilitados o deshabilitados
                cbbtn_cut.Opacity = cbbtn_cut.IsEnabled ? 1 : 0.35;
                cbbtn_copy.Opacity = cbbtn_copy.IsEnabled ? 1 : 0.35;

                menuItemCut.Opacity = menuItemCut.IsEnabled ? 1 : 0.35;
                menuItemCopy.Opacity = menuItemCopy.IsEnabled ? 1 : 0.35;
                menuItemBorrar.Opacity = menuItemBorrar.IsEnabled ? 1 : 0.35;
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
            if (editor != null && editor.SelectionLength > 0)
            {
                // Borra el texto seleccionado
                editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
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
                    MessageBox.Show("Can't format the code because missing closing statements after:\n" + (openedBlock.Line + 1) + ": '" +
                        code[openedBlock.Line].Trim().Trim(new[] { '\n', '\r' }) + "'\n\nPlease check your code and retry.", "Format document", MessageBoxButton.OK,
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

                    // Redibuja la vista de texto para aplicar el resaltado
                    editor.TextArea.TextView.Redraw();
                }
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
                    if (lines[i].Contains(selectedWord))
                    {
                        var rect = new Rectangle
                        {
                            Fill = Brushes.Blue,
                            Width = 5,
                            Height = 2,
                        };

                        Canvas.SetTop(rect, (double)i / lines.Length * (minimap.Height - 46));
                        minimap.Children.Add(rect);
                    }
                }
            }
        }

        // redibuja el canvas cada vez que se maximiza o minimiza
        private async void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Redibuja el minimapa con la última palabra seleccionada
                if (_lastSelectedWord != null)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        minimap.Height = GridGeneralEditor.ActualHeight;
                        UpdateMinimap(_lastSelectedWord);

                        var renderer = new HighlightCurrentLineBackgroundRenderer(editor, minimap);
                        editor.TextArea.TextView.BackgroundRenderers.Add(renderer);
                    }, System.Windows.Threading.DispatcherPriority.Render);
                }
            }
        }



        // FUNCIONES PARA BUSQUEDA  =============================================================
        //evento para controlar Ctrl + F en avalonedit para abrir buscar
        private void CodeEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Comprueba si el usuario presionó Ctrl + F
                if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
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

                // Comprueba si el usuario presionó Ctrl + H
                if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control)
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

        //boton buscar
        private void SearchButton_Click(object sender, RoutedEventArgs e)
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

        //Boton reemplazar
        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
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
            // Cambiar el color del botón basándote en el valor de matchCase
            SolidColorBrush ColorBordeToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0090F1"));
            SolidColorBrush ColorFondoToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AADAFA"));

            // Cambiar el grosor del borde del botón basándote en el valor de matchCase
            MatchCaseButton.BorderThickness = matchCase ? new Thickness(1) : new Thickness(0);

            MatchCaseButton.BorderBrush = matchCase ? ColorBordeToogleSearcha : Brushes.Transparent;
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
            // Cambiar el color del botón basándote en el valor de wholeWordSearch
            SolidColorBrush ColorBordeToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0090F1"));
            SolidColorBrush ColorFondoToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AADAFA"));

            // Cambiar el grosor del borde del botón basándote en el valor de wholeWordSearch
            WholeWordSearchButton.BorderThickness = wholeWordSearch ? new Thickness(1) : new Thickness(0);

            WholeWordSearchButton.BorderBrush = wholeWordSearch ? ColorBordeToogleSearcha : Brushes.Transparent;
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
            // Cambiar el color del botón basándote en el valor de useRegex
            SolidColorBrush ColorBordeToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0090F1"));
            SolidColorBrush ColorFondoToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AADAFA"));

            // Cambiar el grosor del borde del botón basándote en el valor de useRegex
            UseRegexButton.BorderThickness = useRegex ? new Thickness(1) : new Thickness(0);

            UseRegexButton.BorderBrush = useRegex ? ColorBordeToogleSearcha : Brushes.Transparent;
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
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = getActiveTextEditor();

            // Actualizar la búsqueda al cambiar de pestaña
            UpdateSearch(editor);
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
            // Cambiar el color del botón basándote en el valor de preserveCase
            SolidColorBrush ColorBordeToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0090F1"));
            SolidColorBrush ColorFondoToogleSearcha = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AADAFA"));

            // Cambiar el grosor del borde del botón basándote en el valor de preserveCase
            PreserveCaseButton.BorderThickness = preserveCase ? new Thickness(1) : new Thickness(0);

            PreserveCaseButton.BorderBrush = preserveCase ? ColorBordeToogleSearcha : Brushes.Transparent;
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



        // FUNCIONES PARA MANEJAR EDITOR AVALON EDIT  =============================================================

        // para autocompletado
        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

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

                // Si el último carácter ingresado es un espacio en blanco y la ventana de autocompletado está abierta, ciérrala
                if (e.Text == " " && completionWindow != null)
                {
                    completionWindow.Close();
                    completionWindow = null;
                    return;
                }

                // Si el último carácter ingresado es un salto de línea y la ventana de autocompletado está abierta, ciérrala
                if ((e.Text == "\n" || e.Text == "\r") && completionWindow != null)
                {
                    completionWindow.Close();
                    completionWindow = null;
                    return;
                }

                // Utiliza una expresión regular para obtener la última palabra en textUntilCaret
                Match matchLastWord = Regex.Match(textUntilCaret, @"\b(\w+)$");
                string lastWord = matchLastWord.Groups[1].Value;

                // Utiliza lastWord como userInput
                string userInput = lastWord;

                // Open code completion after any letter is entered:
                if (completionWindow != null)
                {
                    // Si la ventana de autocompletado ya está abierta, ciérrala
                    completionWindow.Close();
                    completionWindow = null;
                }
                completionWindow = new CompletionWindow(editor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                // Limpia los datos de autocompletado existentes
                data.Clear();

                // Obtén todo el texto del editor
                string allText = editor.TextArea.Document.Text;

                // Divide el texto en líneas
                string[] lines = allText.Split('\n');

                // Crea una lista para almacenar las líneas sin comentarios
                List<string> linesWithoutComments = new List<string>();

                foreach (string line in lines)
                {
                    // Busca la posición de "//" en la línea
                    int commentPos = line.IndexOf("//");

                    if (commentPos >= 0)
                    {
                        // Si "//" se encuentra en la línea, guarda solo la parte de la línea antes de "//"
                        linesWithoutComments.Add(line.Substring(0, commentPos));
                    }
                    else
                    {
                        // Si "//" no se encuentra en la línea, guarda toda la línea
                        linesWithoutComments.Add(line);
                    }
                }

                // Une linesWithoutComments en un solo string
                string textWithoutComments = string.Join("\n", linesWithoutComments);

                // Ahora, utiliza textWithoutComments en lugar de allText para obtener las declaraciones de variables y funciones
                MatchCollection matchesLocal = Regex.Matches(textWithoutComments, @"(?i)local ([\w, ]+)");
                MatchCollection matchesGlobal = Regex.Matches(textWithoutComments, @"(?i)(?<!end;)\b(\w+);");
                MatchCollection matchesFunctions = Regex.Matches(textWithoutComments, @"(?i)(?<!end;)\b(\w+)\(");

                // Crea listas para almacenar los nombres de las variables y funciones
                List<string> userVariablesLocal = new List<string>();
                List<string> userVariablesGlobal = new List<string>();
                List<string> userFunctions = new List<string>();

                foreach (Match match in matchesLocal)
                {
                    // Divide la declaración de variables en nombres individuales
                    string[] variables = match.Groups[1].Value.Split(',');

                    foreach (string variable in variables)
                    {
                        // Agrega el nombre de la variable a la lista, eliminando los espacios en blanco y cualquier asignación de valor
                        userVariablesLocal.Add(variable.Split(':')[0].Trim());
                    }
                }

                foreach (Match match in matchesGlobal)
                {
                    userVariablesGlobal.Add(match.Groups[1].Value);
                }

                foreach (Match match in matchesFunctions)
                {
                    userFunctions.Add(match.Groups[1].Value);

                }


                // Agregar todas las palabras que comienzan con lo que el usuario ha escrito hasta ahora
                string[] allWords = new string[] { "for", "if", "while", "begin", "EXPORT", "amigo", /* agregar todas las palabras aquí */};
                allWords = allWords.Concat(userVariablesLocal).Concat(userVariablesGlobal).Concat(userFunctions).ToArray();

                string[] wordsWithSnippets = new string[] { "for", "if", "while", "begin", "EXPORT" }; // Agregar las palabras que tienen un snippet asociado aquí

                foreach (string word in allWords)
                {
                    if (word.ToLower().StartsWith(userInput.ToLower()))
                    {
                        string description = "abc"; // Descripción por defecto para palabras sin un snippet asociado
                        if (Array.IndexOf(wordsWithSnippets, word) >= 0)
                        {
                            description = "comando"; // Si la palabra tiene un snippet asociado, cambia la descripción a "comando"
                        }
                        else if (userVariablesLocal.Contains(word))
                        {
                            description = "variable local"; // Si la palabra es una variable local definida por el usuario, cambia la descripción a "variable local"
                        }
                        else if (userVariablesGlobal.Contains(word))
                        {
                            description = "variable global"; // Si la palabra es una variable global definida por el usuario, cambia la descripción a "variable global"
                        }
                        else if (userFunctions.Contains(word))
                        {
                            description = "función"; // Si la palabra es una función definida por el usuario, cambia la descripción a "función"
                        }
                        data.Add(new MyCompletionData(word, description)); // Agregar la palabra a la lista de autocompletado con la descripción apropiada
                    }
                }

                if (data.Count > 0)
                {
                    // Solo muestra la ventana de autocompletado si hay opciones para mostrar
                    completionWindow.Show();
                    // Preselecciona el primer elemento en la ventana de autocompletado
                    completionWindow.CompletionList.SelectedItem = data[0];
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };

                    // Agrega un controlador de eventos para el doble clic
                    completionWindow.CompletionList.ListBox.MouseDoubleClick += (sender, e) =>
                    {
                        completionWindow.CompletionList.RequestInsertion(e);
                    };
                }
                else
                {
                    // Si no hay opciones para mostrar, cierra la ventana de autocompletado y borra userInput
                    userInput = "";
                    if (completionWindow != null)
                    {
                        completionWindow.Close();
                        completionWindow = null;
                    }
                }
            }
        }


        void codeEditor1_TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                if ((e.Key == Key.Tab || e.Key == Key.Enter) && completionWindow != null)
                {
                    // Obtén la posición del cursor
                    int caretPosition = editor.TextArea.Caret.Offset;

                    // Obtén el texto hasta la posición del cursor
                    string textUntilCaret = editor.TextArea.Document.GetText(0, caretPosition);

                    // Utiliza una expresión regular para obtener la última palabra en textUntilCaret
                    Match match = Regex.Match(textUntilCaret, @"\b(\w+)$");
                    string lastWord = match.Groups[1].Value;

                    // Calcula la posición de inicio de la última palabra
                    int start = caretPosition - lastWord.Length;

                    // Crea un nuevo segmento que representa la última palabra
                    ISegment segment = new TextSegment { StartOffset = start, EndOffset = caretPosition };

                    // Inserta la selección de autocompletado en lugar de la última palabra
                    ((MyCompletionData)completionWindow.CompletionList.SelectedItem).Complete(editor.TextArea, segment, e);

                    // Cierra la ventana de autocompletado
                    completionWindow.Close();
                    completionWindow = null;

                    // Evita que se procese más la tecla presionada
                    e.Handled = true;
                }
            }
        }

        // edicion COPIAR seleccion
        private void codeEditor1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Obtén el TextEditor de la pestaña activa
            TextEditor editor = GetActiveTextEditor();

            if (editor != null)
            {
                // Verificar si se hizo clic con el botón izquierdo
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // Verificar si hay texto seleccionado
                    if (editor.SelectionLength > 0)
                    {
                        // Copiar solo el texto seleccionado
                        editor.Copy();
                        e.Handled = true; // Evitar que se seleccione toda la línea
                    }
                }
            }
        }


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
                            if (tooltipPopup != null)
                            {
                                tooltipPopup.IsOpen = false;
                            }
                        }
                    }
                    else
                    {
                        // Si el cursor no está sobre una línea de texto, oculta el Popup
                        if (tooltipPopup != null)
                        {
                            tooltipPopup.IsOpen = false;
                        }
                    }
                }
                else
                {
                    // Si el cursor no está sobre el editor de texto, oculta el Popup
                    if (tooltipPopup != null)
                    {
                        tooltipPopup.IsOpen = false;
                    }
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
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F6F6F6")), // Cambia el color de fondo a blanco
                        Foreground = Brushes.Black, // Cambia el color del texto a negro
                        BorderThickness = new Thickness(0), // Elimina el borde
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, // Establece la visibilidad de la barra de desplazamiento horizontal
                        VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, // Establece la visibilidad de la barra de desplazamiento vertical
                        Margin = new Thickness(10, 0, 10, 0), // Establece un relleno alrededor del TextEditor
                        Text = info,
                        FontFamily = (FontFamily)Application.Current.Resources["ConsolasC"],


                    };

                    // Cargar el archivo XSHD personalizado
                    string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string xshdFilePath = System.IO.Path.Combine(appDirectory, "Estilo", "HPPPL.xshd");

                    using (var stream = File.OpenRead(xshdFilePath))
                    {
                        using (var reader = new XmlTextReader(stream))
                        {
                            textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        }
                    }

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
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F6F6F6")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCEDB")), // Cambia el color del borde a negro
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









        // BOTONES DE ARCHIVOS GUARDAR ABRIR CREAR ETC
        // Variable para almacenar el nombre del archivo actualmente abierto
        string currentFile = null;
        string lastSavedContent = "";

        // BOTON NUEVA PESTAÑA
        // Método para configurar el editor
        private void ConfigureEditor(TextEditor editor)
        {

            // [1] Configura el editor con las propiedades Esteticas
            editor.Margin = new Thickness(0);
            editor.Foreground = new SolidColorBrush(Color.FromArgb(255, 11, 11, 59)); // Foreground
            editor.FontSize = 13;
            editor.FontFamily = new FontFamily("PrimeSansMono"); // FontFamily
            editor.ShowLineNumbers = true;
            // Obtener el ScrollViewer desde el TextView
            ScrollViewer scrollViewer = FindScrollViewer(editor.TextArea.TextView);

            // Configurar el ScrollViewer
            if (scrollViewer != null)
            {
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            }

            editor.WordWrap = false;
            editor.MouseLeftButtonDown += codeEditor1_MouseLeftButtonDown;
            editor.KeyDown += CodeEditor1_KeyDown;

            // Configura el ContextMenu
            editor.ContextMenu = new ContextMenu();
            editor.ContextMenu.Width = 250;

            // Crea los elementos del menú
            MenuItem menuItemCut = new MenuItem() { Header = "Cortar (Ctrl + X)", Style = FindResource("StyleMenuItem") as Style, IsEnabled = false, Opacity = 0.35 };
            menuItemCut.Icon = new SvgViewbox() { Source = new Uri("/Imagen/Inicio/cut.svg", UriKind.Relative), Width = 16, Height = 16, Margin = new Thickness(10, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            menuItemCut.Click += CortarButton_Click;

            MenuItem menuItemCopy = new MenuItem() { Header = "Copiar (Ctrl + C)", Style = FindResource("StyleMenuItem") as Style, IsEnabled = false, Opacity = 0.35 };
            menuItemCopy.Icon = new SvgViewbox() { Source = new Uri("/Imagen/Inicio/copy.svg", UriKind.Relative), Width = 16, Height = 16, Margin = new Thickness(10, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            menuItemCopy.Click += CopiarButton_Click;

            MenuItem menuItemPaste = new MenuItem() { Header = "Pegar (Ctrl + V)", Style = FindResource("StyleMenuItem") as Style };
            menuItemPaste.Icon = new SvgViewbox() { Source = new Uri("/Imagen/Inicio/paste.svg", UriKind.Relative), Width = 16, Height = 16, Margin = new Thickness(10, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            menuItemPaste.Click += PasteButton_Click;

            MenuItem menuItemDelete = new MenuItem() { Header = "Borrar", Style = FindResource("StyleMenuItem") as Style, IsEnabled = false, Opacity = 0.35 };
            menuItemDelete.Icon = new SvgViewbox() { Source = new Uri("/Imagen/Inicio/borrar.svg", UriKind.Relative), Width = 16, Height = 16, Margin = new Thickness(10, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            menuItemDelete.Click += BorrarButton_Click;

            MenuItem menuItemSelectAll = new MenuItem() { Header = "Selecionar Todo (Ctrl + A)", Style = FindResource("StyleMenuItem") as Style };
            menuItemSelectAll.Icon = new SvgViewbox() { Source = new Uri("/Imagen/Inicio/selected.svg", UriKind.Relative), Width = 16, Height = 16, Margin = new Thickness(10, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            menuItemSelectAll.Click += SelectAllButton_Click;

            // Agrega los elementos al ContextMenu
            editor.ContextMenu.Items.Add(menuItemCut);
            editor.ContextMenu.Items.Add(menuItemCopy);
            editor.ContextMenu.Items.Add(menuItemPaste);
            editor.ContextMenu.Items.Add(menuItemDelete);
            editor.ContextMenu.Items.Add(new Separator());
            editor.ContextMenu.Items.Add(menuItemSelectAll);


            //[2] Propiedades netamente del editor
            editor.TextArea.TextView.MouseMove += TextView_MouseMove; // funcion ver hover  popup

            editor.MouseLeave += (sender, e) => // oculta hover popup si se sale del editor
            {
                // Cerrar el Popup cuando el mouse sale de codeEditor1
                if (tooltipPopup != null)
                {
                    tooltipPopup.IsOpen = false;
                }
            };

            // numeracion  =============
            // Asume que "textEditor" es el nombre de tu TextEditor
            foreach (var margin in editor.TextArea.LeftMargins)
            {
                if (margin is LineNumberMargin lineNumberMargin)
                {

                    lineNumberMargin.Width = 50; // Ajusta este número a tu gusto
                    break;
                }
            }

            // Cargar Colores ============
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string xshdFilePath = System.IO.Path.Combine(appDirectory, "Estilo", "HPPPL.xshd");

            using (Stream s = File.OpenRead(xshdFilePath))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            // Asume que "codeEditor1" es el nombre de tu TextEditor
            editor.TextArea.IndentationStrategy = new MyIndentationStrategy();

            //  REPLEGAR  ===========
            var foldingManager = FoldingManager.Install(editor.TextArea);
            var foldingStrategy = new MyFoldingStrategy(editor);

            editor.TextArea.Document.Changed += (sender, e) =>
            {
                foldingStrategy.UpdateFoldings(foldingManager, editor.Document);
            };

            //AUTO COMPLETADO ====================
            // Agrega los manejadores de eventos para TextEntering y TextEntered
            editor.TextArea.PreviewKeyDown += codeEditor1_TextArea_PreviewKeyDown;
            editor.TextArea.TextEntered += textEditor_TextArea_TextEntered;


            //PARA PINTAR LINEA AL ESCRIBIR ================ EVENTO

            var renderer = new HighlightCurrentLineBackgroundRenderer(editor, minimap);
            editor.TextArea.TextView.BackgroundRenderers.Add(renderer);


            //PARA GUIDE LINE ALCANCE DE CODIGO  =============== EVENTO
            editor.TextArea.TextView.BackgroundRenderers.Add(new IndentationGuidesRenderer(editor.TextArea.TextView));

            //PARA ACTIVAR O DESACTIVAR CUT Y COPY =============== EVENTO
            // Inicializa las variables de la selección anterior
            previousSelectionStart = editor.SelectionStart;
            previousSelectionLength = editor.SelectionLength;

            // Crea un temporizador para comprobar regularmente si la selección ha cambiado
            selectionChangedTimer = new DispatcherTimer();
            selectionChangedTimer.Interval = TimeSpan.FromMilliseconds(100); // Comprueba cada 100 milisegundos
            selectionChangedTimer.Tick += SelectionChangedTimer_Tick;
            selectionChangedTimer.Tag = editor; // Asigna el TextEditor correcto al temporizador
            selectionChangedTimer.Start();

            //PARA MOSTRAR REFERENCIAS DE UNA SELECCION =============== EVENTO
            // Instala el resaltador de palabras en el TextArea
            _wordHighlighter = new WordHighlighter(editor);
            editor.TextArea.TextView.BackgroundRenderers.Add(_wordHighlighter);

            // Inicializa el temporizador de resaltado
            _highlightingTimer = new DispatcherTimer();
            _highlightingTimer.Interval = TimeSpan.FromMilliseconds(100); // Comprueba cada 100 milisegundos
            _highlightingTimer.Tick += HighlightingTimer_Tick;
            _highlightingTimer.Start();

            //PARA BUSQUEDA O REEMPLAZO DE TEXTO=============== EVENTO
            // Inicializa el temporizador de resaltado



        }

        // Método para buscar el ScrollViewer dentro de un elemento visual
        private ScrollViewer FindScrollViewer(Visual visual)
        {
            if (visual is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i) as Visual;
                if (child != null)
                {
                    var foundScrollViewer = FindScrollViewer(child);
                    if (foundScrollViewer != null)
                    {
                        return foundScrollViewer;
                    }
                }
            }

            return null;
        }


        // Aplicando el boton nuevo para creacion de nuevas pestañas
        private void NuevoButton_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva pestaña
            TabItem newTab = new TabItem() { Header = "Nuevo archivo" };

            // Crea un nuevo editor de código
            TextEditor newEditor = new TextEditor();
            ConfigureEditor(newEditor); // Configura el nuevo editor

            // Agrega el editor de código como contenido de la pestaña
            newTab.Content = newEditor;

            // Agrega la pestaña al TabControl
            tabControl.Items.Add(newTab);

            // Selecciona la nueva pestaña
            tabControl.SelectedItem = newTab;
        }
        //BOTON GUARDAR COMO
        private void GuardarComoButton_Click(object sender, RoutedEventArgs e)
        {
            // Restablecer el nombre del archivo actual para forzar al método SaveFile a mostrar el cuadro de diálogo de guardar archivo
            currentFile = null;
            //SaveFile();
        }

        //BOTON GUARDAR
        private void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            //SaveFile();
        }

        
        // Método para guardar el archivo
        void SaveFile()
        {
            if (currentFile == null)
            {
                // Mostrar el cuadro de diálogo de guardar archivo si no hay un archivo actualmente abierto
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "HP Code Files (*.hpcode)|*.hpcode|HP Prime Program Files (*.hpprgm)|*.hpprgm";
                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFile = saveFileDialog.FileName;
                }
                else
                {
                    // El usuario canceló el cuadro de diálogo de guardar archivo, así que no hacemos nada
                    return;
                }
            }

            // Guardar el contenido del TextEditor en el archivo actual
            File.WriteAllText(currentFile, codeEditor1.Text);

            // Actualizar lastSavedContent con el contenido actual del editor
            lastSavedContent = codeEditor1.Text;
        }
        

        
        //BOTON ABRIR
        private void AbrirButton_Click(object sender, RoutedEventArgs e)
        {
            // Comprobar si el contenido del editor ha cambiado desde la última vez que se guardó
            if (codeEditor1.Text != lastSavedContent)
            {
                // Preguntar al usuario si desea guardar los cambios
                MessageBoxResult result = MessageBox.Show("¿Desea guardar los cambios en el archivo actual antes de abrir uno nuevo?", "Guardar cambios", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    // El usuario quiere guardar los cambios, así que llamamos al método SaveFile
                    SaveFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // El usuario ha cancelado la apertura de un nuevo archivo, así que no hacemos nada
                    return;
                }
            }

            // Mostrar el cuadro de diálogo de abrir archivo
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "HP Code Files (*.hpcode)|*.hpcode|HP Prime Program Files (*.hpprgm)|*.hpprgm";
            if (openFileDialog.ShowDialog() == true)
            {
                // Leer el contenido del archivo y ponerlo en el editor
                codeEditor1.Text = File.ReadAllText(openFileDialog.FileName);

                // Actualizar las variables currentFile y lastSavedContent
                currentFile = openFileDialog.FileName;
                lastSavedContent = codeEditor1.Text;
            }
        }
        
        //BOTON IMPRIMIR
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

        }


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










    }
}