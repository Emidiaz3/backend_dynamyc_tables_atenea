using System.IO;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Utils
{
    public class FileReader
    {

        public static async Task<string> LoadTemplateAsync(string templatePath)
        {
            // Cargar la plantilla HTML desde el archivo
            using (var reader = new StreamReader(templatePath))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
