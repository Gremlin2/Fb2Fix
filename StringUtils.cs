// Copyright (c) 2007-2012 Andrej Repin aka Gremlin2
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FB2Fix
{
    public static class StringUtils
    {
        private static readonly Dictionary<char, string> table;
        private static readonly Regex invalidCharsRegex;

        static StringUtils()
        {
            table = new Dictionary<char, string>(75);

            table.Add('а', "a");
            table.Add('б', "b");
            table.Add('в', "v");
            table.Add('г', "g");
            table.Add('д', "d");
            table.Add('е', "e");
            table.Add('ж', "zh");
            table.Add('з', "z");
            table.Add('и', "i");
            table.Add('й', "y");
            table.Add('к', "k");
            table.Add('л', "l");
            table.Add('м', "m");
            table.Add('н', "n");
            table.Add('о', "o");
            table.Add('п', "p");
            table.Add('р', "r");
            table.Add('с', "s");
            table.Add('т', "t");
            table.Add('у', "u");
            table.Add('ф', "f");
            table.Add('х', "h");
            table.Add('ц', "ts");
            table.Add('ч', "ch");
            table.Add('ш', "sh");
            table.Add('щ', "sch");
            table.Add('ъ', "'");
            table.Add('ы', "yi");
            table.Add('ь', "");
            table.Add('э', "e");
            table.Add('ю', "yu");
            table.Add('я', "ya");
            table.Add('і', "i");
            table.Add('ґ', "g");
            table.Add('ё', "yo");
            table.Add('є', "e");
            table.Add('ї', "yi");
            table.Add('№', "#");
            table.Add('А', "A");
            table.Add('Б', "B");
            table.Add('В', "V");
            table.Add('Г', "G");
            table.Add('Д', "D");
            table.Add('Е', "E");
            table.Add('Ж', "ZH");
            table.Add('З', "Z");
            table.Add('И', "I");
            table.Add('Й', "Y");
            table.Add('К', "K");
            table.Add('Л', "L");
            table.Add('М', "M");
            table.Add('Н', "N");
            table.Add('О', "O");
            table.Add('П', "P");
            table.Add('Р', "R");
            table.Add('С', "S");
            table.Add('Т', "T");
            table.Add('У', "U");
            table.Add('Ф', "F");
            table.Add('Х', "H");
            table.Add('Ц', "TS");
            table.Add('Ч', "CH");
            table.Add('Ш', "SH");
            table.Add('Щ', "SCH");
            table.Add('Ъ', "'");
            table.Add('Ы', "YI");
            table.Add('Ь', "");
            table.Add('Э', "E");
            table.Add('Ю', "YU");
            table.Add('Я', "YA");
            table.Add('І', "I");
            table.Add('Ґ', "G");
            table.Add('Ё', "YO");
            table.Add('Є', "E");
            table.Add('Ї', "YI");

            char[] invalidChars = Path.GetInvalidFileNameChars();
            List<string> pattern = new List<string>(invalidChars.Length);

            foreach (char invalidChar in invalidChars)
            {
                if(invalidChar == Path.DirectorySeparatorChar)
                {
                    continue;
                }

                if (Char.IsControl(invalidChar))
                {
                    pattern.Add(String.Format("\\u{0:x4}", (int) invalidChar));
                }
                else
                {
                    pattern.Add(Regex.Escape(invalidChar.ToString()));
                }
            }

            invalidCharsRegex = new Regex("[" + String.Join("|", pattern.ToArray()) + "]");
        }

        private static char GetChar(string value, int index)
        {
            if(index < value.Length)
            {
                return value[index];
            }

            return Char.MinValue;
        }

        public static string Capitalize(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            return String.Concat(value.Substring(0, 1).ToUpperInvariant(), value.Substring(1));
        }

        public static string Squeeze(string str, char value)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if(str.Length <= 1)
            {
                return str;
            }

            StringBuilder buffer = new StringBuilder(str.Length);

            char prevChar = str[0];
            buffer.Append(prevChar);

            for (int index = 1; index < str.Length; index++)
            {
                char nextChar = str[index];
                
                if(prevChar == nextChar && nextChar == value)
                {
                    continue;
                }

                buffer.Append(nextChar);
                prevChar = nextChar;
            }

            return buffer.ToString();
        }

        public static string Translify(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            StringBuilder buffer = new StringBuilder(value.Length);

            for (int index = 0; index < value.Length; index++)
            {
                if(table.ContainsKey(value[index]))
                {
                    if(Char.IsUpper(value[index]) && Char.IsLower(GetChar(value, index + 1)))
                    {
                        buffer.Append(Capitalize(table[value[index]].ToLowerInvariant()));
                    }
                    else
                    {
                        buffer.Append(table[value[index]]);
                    }
                }
                else
                {
                    buffer.Append(value[index]);
                }
            }

            return buffer.ToString();
        }

        public static string Dirify(string value)
        {
            return Dirify(value, false);
        }

        public static string Dirify(string value, bool strict)
        {
            if(String.IsNullOrEmpty(value))
            {
                return value;
            }

            string filename = value;

            filename = Regex.Replace(filename, @"(\s\&\s)|(\s\&amp\;\s)", " and ");
            
            filename = filename.Replace("–", "-");
            filename = filename.Replace("—", "-");
            filename = filename.Replace("―", "-");
            filename = filename.Replace("‗", "_");
            filename = filename.Replace("«", "'");
            filename = filename.Replace("»", "'");
            filename = filename.Replace("‘", "'");
            filename = filename.Replace("’", "'");
            filename = filename.Replace("‚", "'");
            filename = filename.Replace("‛", "'");
            filename = filename.Replace("“", "'");
            filename = filename.Replace("”", "'");
            filename = filename.Replace("„", "'");
            filename = filename.Replace("′", "'");
            filename = filename.Replace("″", "'");
            filename = filename.Replace("‹", "'");
            filename = filename.Replace("›", "'");
            
            if (strict)
            {
                filename = Regex.Replace(filename, @"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Pc}\p{Lm}\\/\[\]\-_\*,\(\)']", " ");
                filename = Squeeze(filename, ' ');
            }
            
            filename = invalidCharsRegex.Replace(filename, "");

            filename = Regex.Replace(filename, @"(_)$", "");
            filename = Regex.Replace(filename, @"^(_)", "");

            filename = filename.Trim();

            return filename;
        }

        public static string Truncate(string value, int maxLength)
        {
            int length;

            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            length = Math.Min(value.Length, maxLength);

            return value.Substring(0, length);
        }

        public static string CleanupFileName(string filename)
        {
            if(String.IsNullOrEmpty(filename))
            {
                return filename;
            }

            return invalidCharsRegex.Replace(filename, "");
        }
    }
}
