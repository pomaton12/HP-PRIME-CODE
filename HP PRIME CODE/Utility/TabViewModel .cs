using ICSharpCode.AvalonEdit;
using System.ComponentModel;
using System.Runtime.CompilerServices;


    public class TabViewModel : INotifyPropertyChanged
    {
        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged(); // Notificar a las vistas cuando el valor de Header cambie
            }
        }

        private object _content;
        public object Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged(); // Notificar a las vistas cuando el valor de Content cambie
            }
        }

        private TextEditor _editor;
        public TextEditor Editor
        {
            get { return _editor; }
            set
            {
                _editor = value;
                OnPropertyChanged(); // Notificar a las vistas cuando el valor de Editor cambie
            }
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
