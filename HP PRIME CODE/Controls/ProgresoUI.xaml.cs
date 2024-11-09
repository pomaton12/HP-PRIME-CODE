using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HP_PRIME_CODE.Controls
{
    /// <summary>
    /// Lógica de interacción para ProgresoUI.xaml
    /// </summary>
    public partial class ProgresoUI 
    {
        public ProgresoUI()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int progress)
        {
            ProgressBar.Value = progress;
            ProgressLabel.Content = $"Procesando... {progress}%";
        }

    }
}
