using System;
using System.Threading.Tasks;
using System.Windows;
using Velopack;

namespace HP_PRIME_CODE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static MemoryLogger Log { get; private set; }

        // Since WPF has an "automatic" Program.Main, we need to create our own.
        // In order for this to work, you must also add the following to your .csproj:
        // <StartupObject>CSharpWpf.App</StartupObject>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                // Logging is essential for debugging! Ideally you should write it to a file.
                Log = new MemoryLogger();

                // It's important to Run() the VelopackApp as early as possible in app startup.
                VelopackApp.Build()
                    .WithFirstRun((v) => { /* Your first run code here */ })
                    .Run(Log);

                // We can now launch the WPF application as normal.
                var app = new App();
                app.InitializeComponent();
                app.Run();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unhandled exception: " + ex.ToString());
            }
        }

        public void ChangeTheme(string themeName)
        {
            ResourceDictionary themeResources = null;

            if (themeName == "Light")
            {
                themeResources = new ResourceDictionary { Source = new Uri("Recursos/Color_Light.xaml", UriKind.Relative) };
                ResourceDictionary iconResources = new ResourceDictionary { Source = new Uri("Recursos/IconLight.xaml", UriKind.Relative) };
                themeResources.MergedDictionaries.Add(iconResources);
            }
            else if (themeName == "Dark")
            {
                themeResources = new ResourceDictionary { Source = new Uri("Recursos/Color_Dark.xaml", UriKind.Relative) };
                ResourceDictionary iconResources = new ResourceDictionary { Source = new Uri("Recursos/IconDark.xaml", UriKind.Relative) };
                themeResources.MergedDictionaries.Add(iconResources);
            }
            // Añade más condiciones para cada tema adicional

            if (themeResources != null)
            {
                Resources.MergedDictionaries.Remove((ResourceDictionary)Resources["CurrentTheme"]);
                Resources["CurrentTheme"] = themeResources;
                Resources.MergedDictionaries.Add(themeResources);

                // Forzar la actualización de los colores en los elementos
                foreach (Window window in Application.Current.Windows)
                {
                    window.InvalidateVisual();
                }
            }
        }


        public void ChangeLanguage(string languageName)
        {
            ResourceDictionary languageResources = null;

            if (languageName == FindResource("es_Es").ToString())
            {
                languageResources = new ResourceDictionary { Source = new Uri("Recursos/Idioma_Es.xaml", UriKind.Relative) };
            }
            else if (languageName == FindResource("en_En").ToString())
            {
                languageResources = new ResourceDictionary { Source = new Uri("Recursos/Idioma_En.xaml", UriKind.Relative) };
            }
            // Añade más condiciones para cada idioma adicional

            if (languageResources != null)
            {
                Resources.MergedDictionaries.Remove((ResourceDictionary)Resources["CurrentLanguage"]);
                Resources["CurrentLanguage"] = languageResources;
                Resources.MergedDictionaries.Add(languageResources);

                // Forzar la actualización de los textos en los elementos
                foreach (Window window in Application.Current.Windows)
                {
                    window.InvalidateVisual();
                }
            }
        }


    }

}
