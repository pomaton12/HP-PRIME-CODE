using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HP_PRIME_CODE.UtilityPrime
{
    public static class HpprgmExtractor
    {
        // Marcadores fijos (hexadecimal)
        private static readonly byte[] keyMarker = ConvertHexStringToBytes("7C618AB2FEFFFFFF00");
        private static readonly byte[] openingMarker = ConvertHexStringToBytes("9B00C000");
        private static readonly byte[] closingMarker = ConvertHexStringToBytes("0000EC0300008E0240");

        public static string ExtractLegibleText(string filename)
        {
            // Leer archivo completo en bytes
            byte[] data;
            try
            {
                data = File.ReadAllBytes(filename);
            }
            catch (Exception ex)
            {
                return $"Error al leer el archivo: {ex.Message}";
            }

            // Verificar existencia de keyMarker
            if (FindMarker(data, keyMarker) == -1)
            {
                // Si keyMarker no existe, asumir texto legible directo (UTF-8)
                return System.Text.Encoding.UTF8.GetString(data);
            }

            // Buscar openingMarker y closingMarker
            int startIndex = FindMarker(data, openingMarker);
            if (startIndex == -1)
                return "No se encontró el marcador de apertura. Archivo posiblemente corrupto.";

            int endIndex = FindMarker(data, closingMarker, startIndex);
            if (endIndex == -1)
                return "No se encontró el marcador de cierre. Archivo posiblemente corrupto.";

            // Extraer datos entre los marcadores
            int contentLength = endIndex - startIndex - openingMarker.Length;
            if (contentLength <= 0)
                return "Los marcadores encontrados no contienen datos válidos.";

            byte[] extractedData = new byte[contentLength];
            Array.Copy(data, startIndex + openingMarker.Length, extractedData, 0, contentLength);

            // Convertir los datos a texto legible (UTF-16)
            string text = System.Text.Encoding.Unicode.GetString(extractedData);
            
            return text;
                
        }

        private static int FindMarker(byte[] data, byte[] marker, int startIndex = 0)
        {
            for (int i = startIndex; i <= data.Length - marker.Length; i++)
            {
                if (MatchMarker(data, marker, i))
                    return i;
            }
            return -1;
        }

        private static bool MatchMarker(byte[] data, byte[] marker, int position)
        {
            for (int j = 0; j < marker.Length; j++)
            {
                if (data[position + j] != marker[j])
                    return false;
            }
            return true;
        }

        private static byte[] ConvertHexStringToBytes(string hexString)
        {
            if (hexString.Length % 2 != 0)
                throw new ArgumentException("La cadena hexadecimal debe tener una longitud par.");

            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        // Función para limpiar el texto, simulando el proceso de copiar y pegar
        private static string CleanText(string input)
        {
            // 1. Normalizar saltos de línea (convertir todos a '\n')
            input = input.Replace("\r\n", "\n").Replace("\r", "\n");

            // 2. Eliminar espacios o tabulaciones al final de cada línea
            input = string.Join("\n", input.Split('\n').Select(line => line.TrimEnd()));

            // 3. Opcional: Eliminar caracteres invisibles no deseados
            input = Regex.Replace(input, @"[^\x20-\x7E\t\r\n]", ""); // Solo deja ASCII imprimible, tabs y saltos de línea

            return input;
        }
    }
}
