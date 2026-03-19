using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScriptDataTool
{
    public class Util
    {
        public static string ConvertStringToCamelCase(string input)
        {

            string[] specialCharacters = { "(", ")", ",", "'", "\"", "?", "!", ":", "<", ">", "*", "{", "}", "%", "|", "&", "#", "~", "`", "-", ".", "’" };

            foreach (var character in specialCharacters)
            {
                input = input.Replace(character, "");
            }

            input = RemoveVietnameseDiacritics(input);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            // ký tự sau khoảng trắng viết hoa
            if (input.Contains(" "))
            {
                input = CapitalizeFirstLetterAfterSpace(input);
                input = input.Replace(" ", "_");
            }
            input = Encoding.UTF8.GetString(Encoding.GetEncoding("utf-8").GetBytes(input));
            if (!string.IsNullOrEmpty(input))
            {
                input = char.ToLower(input[0]) + input.Substring(1);
            }

            // Viết hoa ký tự đầu tiên của chuỗi
            input = textInfo.ToTitleCase(input);

            //Cắt nếu độ dài quá 80 ký tự
            if (input.Length > 80)
            {
                // Cắt tên folder nếu độ dài vượt quá 40 ký tự
                input = input.Substring(0, 80);
                input = input.Trim();
            }

            return input;
        }
        public static string RemoveVietnameseDiacritics(string input)
        {
            string[] baseChars = new string[] { "a", "o", "e", "u", "i", "y", "A", "O", "E", "U", "I", "d", "D" };
            string[] accentChars = new string[] { "à|ả|ã|ạ|á|ắ|ằ|ẵ|ặ|ă|ẳ|ấ|ầ|ẫ|ậ|â|ẩ", "ò|ỏ|õ|ọ|ó|ô|ố|ồ|ỗ|ộ|ổ|ơ|ớ|ờ|ỡ|ợ|ở", "è|ẻ|ẽ|ẹ|é|ê|ế|ề|ễ|ệ|ể", "ù|ủ|ũ|ụ|ú|ư|ứ|ừ|ữ|ự|ử", "ì|ỉ|ĩ|ị|í", "y|ý|ỳ|ỹ|ỷ", "À|Ả|Ã|Ạ|Á|Ắ|Ằ|Ẵ|Ặ|Ẳ|Ă|Ấ|Ầ|Ẫ|Ậ|Â|Ẩ", "Ò|Ỏ|Õ|Ọ|Ó|Ô|Ố|Ồ|Ỗ|Ộ|Ổ|Ơ|Ớ|Ờ|Ỡ|Ợ|Ở", "È|Ẻ|Ẽ|Ẹ|É|Ê|Ế|Ệ|Ể|Ề", "Ù|Ủ|Ũ|Ụ|Ú|Ư|Ứ|Ự|Ừ|Ữ|Ử", "Ì|Ỉ|Ĩ|Ị|Í", "Ỳ|Ý|Ỵ|Ỹ|Ỷ", "đ", "Đ" };

            for (int i = 0; i < baseChars.Length; i++)
            {
                input = Regex.Replace(input, accentChars[i], baseChars[i], RegexOptions.IgnoreCase);
            }

            return input;
        }

        static string CapitalizeFirstLetterAfterSpace(string input)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input);
        }
    }
}
