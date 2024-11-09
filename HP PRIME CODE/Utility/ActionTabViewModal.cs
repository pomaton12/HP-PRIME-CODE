using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using ICSharpCode.AvalonEdit;
using Wpf.Ui.Input;

public class ActionTabViewModal : INotifyPropertyChanged
{
    // Colección de pestañas
    public ObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollection<TabViewModel>();

    // Comando para cerrar pestañas
    public RelayCommand<TabItem> CloseTabCommand { get; }

    // Propiedad para la pestaña actual (seleccionada)
    private TabViewModel _currentTab;
    public TabViewModel CurrentTab
    {
        get { return _currentTab; }
        set
        {
            _currentTab = value;
            OnPropertyChanged();
        }
    }

    // Método para seleccionar la última pestaña agregada
    public void SelectLastTab()
    {
        if (Tabs.Count > 0)
        {
            CurrentTab = Tabs.Last();
        }
    }

    // Constructor
    public ActionTabViewModal()
    {
        // Inicializar el comando para cerrar pestañas
        CloseTabCommand = new RelayCommand<TabItem>(CloseTab);

        // Crear una pestaña por defecto
        TextEditor defaultEditor = new TextEditor();
        Tabs.Add(new TabViewModel { Header = "Nuevo archivo 1", Content = defaultEditor, Editor = defaultEditor });
    }

    // Método para cerrar una pestaña
    private void CloseTab(TabItem parameter)
    {
        if (parameter != null)
        {
            // Encuentra el TabViewModel asociado a la pestaña
            TabViewModel tabViewModel = Tabs.FirstOrDefault(t => t.Header == parameter.Header);

            if (tabViewModel != null)
            {
                Tabs.Remove(tabViewModel);
            }
        }
    }

    // Método para crear pestañas iniciales (no necesario si se crea una pestaña por defecto en el constructor)
    public void Populate()
    {
        // No se necesita Populate() ya que la pestaña por defecto se crea en el constructor
    }

    // Implementación de la interfaz INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}