using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Utilidades
{

    public static void CreateOrReplaceArchive(string copiedFileName) 
    {

        // Ruta del archivo original en el directorio de salida
        string originalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FilesNuevo", "NewCodeHP.hpprgm");

        // Ruta del directorio donde quieres copiar el archivo
        string targetDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HP Prime", "Calculators", "Prime");


        // Combina la ruta del directorio destino y el nombre del archivo copiado para obtener la ruta completa del archivo copiado
        string copiedFilePath = Path.Combine(targetDirectory, copiedFileName);

        // Verifica si el archivo ya existe
        if (File.Exists(copiedFilePath))
        {
            // Si el archivo existe, lo reemplaza con el archivo de tu proyecto
            File.Copy(originalFilePath, copiedFilePath, true);
        }
        else
        {
            // Si el archivo no existe, lo copia al directorio destino y lo renombra
            File.Copy(originalFilePath, copiedFilePath);
        }


    }




}

