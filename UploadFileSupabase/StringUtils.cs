using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace UploadFileSupabase
{
    public class StringUtils
    {
        public static string RemoveSpecialCharactersFromFileName(string fileName)
        {
            // Separa o nome do arquivo e a extensão
            var extension = Path.GetExtension(fileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            // Remove caracteres especiais do nome do arquivo
            var sanitizedFileName = RemoveSpecialCharacters(nameWithoutExtension);

            // Retorna o nome do arquivo com a extensão preservada
            return sanitizedFileName + extension;
        }

        private static string RemoveSpecialCharacters(string input)
        {
            // Remove diacríticos (acentos)
            string normalizedString = RemoveDiacritics(input);

            // Define a expressão regular para substituir todos os caracteres que não são letras, números ou espaço
            string pattern = @"[^a-zA-Z0-9\s]";
            string replacement = "";

            // Cria o regex e faz a substituição
            Regex regex = new Regex(pattern);
            string sanitizedString = regex.Replace(normalizedString, replacement);

            // Remove espaços extras
            sanitizedString = Regex.Replace(sanitizedString, @"\s+", " ").Trim();

            return sanitizedString;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }

}
