using System.Configuration;
using System.Data;
using System.Windows;

namespace HP_PRIME_CODE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

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
