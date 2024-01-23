using System;
using System.Security.Cryptography;
using System.Text;

namespace ApiRestCuestionario.Utils
{
    public class Encryptor
    {
        static readonly string clave = "cadenadecifrado";

        public static string Encrypt(string cadena)
        {
            byte[] llave;
            byte[] arreglo = Encoding.UTF8.GetBytes(cadena);
            MD5 md5 =  MD5.Create();
            llave = md5.ComputeHash(Encoding.UTF8.GetBytes(clave));
            md5.Clear();
            TripleDES tripledes =  TripleDES.Create();
            tripledes.Key = llave;
            tripledes.Mode = CipherMode.ECB;
            tripledes.Padding = PaddingMode.PKCS7;
            ICryptoTransform convertir = tripledes.CreateEncryptor(); 
            byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length); 
            tripledes.Clear();
            return Convert.ToBase64String(resultado, 0, resultado.Length);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] llave;
            byte[] arreglo = Convert.FromBase64String(encryptedText);
            MD5 md5 =  MD5.Create();
            llave = md5.ComputeHash(Encoding.UTF8.GetBytes(clave));
            md5.Clear();
            TripleDES tripledes = TripleDES.Create();
            tripledes.Key = llave;
            tripledes.Mode = CipherMode.ECB;
            tripledes.Padding = PaddingMode.PKCS7;
            ICryptoTransform convertir = tripledes.CreateDecryptor();
            byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length);
            tripledes.Clear();
            string cadena_descifrada = Encoding.UTF8.GetString(resultado);
            return cadena_descifrada; 
        }
    }

}
