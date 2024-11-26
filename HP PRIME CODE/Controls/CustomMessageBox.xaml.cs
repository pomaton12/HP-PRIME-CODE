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
    /// Lógica de interacción para CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox
    {
        public CustomMessageBox()
        {
            InitializeComponent();
            // Suscribirse al evento Closing para manejar cierres inesperados
            this.Closing += CustomMessageBox_Closing;
        }

        public enum MessageType
        {
            None,
            Information,
            Interrogation,
            Afirmation,
            Error,
            Caution
        }

        public static bool? Show(
            string message,
            string title = "",
            string yesText = "Sí",
            string noText = "No",
            string cancelText = "Cancelar",
            MessageType messageType = MessageType.None)
        {
            // Crear instancia del CustomMessageBox
            var messageBox = new CustomMessageBox();

            // Configurar el mensaje, título e ícono
            messageBox.CustomMessageContenido.Text = message;
            messageBox.MessageTitle.Title = title;
            messageBox.SetMessageImage(messageType);

            // Personalizar el texto de los botones
            messageBox.OkButton.Content = yesText;
            messageBox.NoButton.Content = noText;
            messageBox.NameCancel.Text = cancelText;

            // Mostrar el diálogo y retornar el resultado
            return messageBox.ShowDialog();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Representa "Sí"
            
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Representa "No"
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = null; // Indicar que el usuario ha presionado "Cancelar"
            this.Close();  // Asegúrate de cerrar la ventana después de asignar el DialogResult
        }

        private void SetMessageImage(MessageType type)
        {
            if (type == MessageType.None)
            {

                MessageImage.Source = new BitmapImage(new Uri("/Imagen/MessageBox/none.png", UriKind.Relative));
                ColumnaIcono.Width = new GridLength(0);

            }
            if (type == MessageType.Information)
            {
                MessageImage.Source = new BitmapImage(new Uri("/Imagen/MessageBox/info.png", UriKind.Relative));
            }
            if (type == MessageType.Interrogation)
            {
                MessageImage.Source = new BitmapImage(new Uri("/Imagen/MessageBox/interrogation.png", UriKind.Relative));
            }
            if (type == MessageType.Afirmation)
            {
                MessageImage.Source = new BitmapImage(new Uri("/Imagen/MessageBox/afirmation.png", UriKind.Relative));
            }
            if (type == MessageType.Error)
            {
                MessageImage.Source = new BitmapImage(new Uri("/Imagen/MessageBox/error.png", UriKind.Relative));
            }
            if (type == MessageType.Caution)
            {
                MessageImage.Source = new BitmapImage(new Uri("/Imagen/MessageBox/caution.png", UriKind.Relative));
            }
        }


        private void CustomMessageBox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Si DialogResult ya ha sido asignado (true, false o null), entonces dejamos que la ventana se cierre.
            if (!this.DialogResult.HasValue)
            {
                e.Cancel = true; // Cancela el cierre si DialogResult aún no ha sido asignado
            }
        }





    }
}
