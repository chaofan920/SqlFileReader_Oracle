using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlFileReader
{
    internal static class SqlFileReader
    {
        public static string[] ReadSqlFile(string filePath)
        {
            string sql = Regex.Replace(File.ReadAllText(filePath), @"(\r\n|\t|\s+)", @"~$1~", RegexOptions.IgnoreCase);
            string sqlmod = Regex.Replace(sql, @"~~", @"~", RegexOptions.IgnoreCase);
            string[] sqloutput = sqlmod.Split('~');
            for (int i = 0; i < sqloutput.Length; i++)
            {
                sqloutput[i] = sqloutput[i].Replace("\r\n", "\\r\\n").Replace("\t", "\\t");
            }
            return sqloutput;
        }
        public static string ReconstructSqlFile(string[] sql)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string statement in sql)
            {
                sb.Append(statement);
            }
            string sqlmod = sb.ToString().Replace("\\r\\n", "\r\n").Replace("\\t", "\t");
            return sqlmod;
        }
        public static Dictionary<int, string> GetNonEmptyStrings(string[] array)
        {
            var result = new Dictionary<int, string>();
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (!string.IsNullOrWhiteSpace(str) && str != "\\r\\n" && str != "\\t")
                {
                    result[i] = str;
                }
            }
            return result;
        }

        public static void PrintDictionaryContents(Dictionary<int, string> dictionary)
        {
            int i;
            for (i = 0; i < dictionary.Count; i++)
            {
                int key = dictionary.Keys.ElementAt(i);
                if (dictionary[key] == "||")
                {
                    dictionary[dictionary.Keys.ElementAt(i - 1)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i - 1)], @"(\S+)", @"CONCAT($1");
                    dictionary[dictionary.Keys.ElementAt(i)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i)], @"(\|\|)", @",");
                    i += 2;
                    if (i < dictionary.Count)
                    {
                        while (dictionary[dictionary.Keys.ElementAt(i)] == "||")
                        {
                            dictionary[dictionary.Keys.ElementAt(i)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i)], @"\|\|", @",");
                            i += 2;
                            if (i >= dictionary.Count)
                            {
                                break;
                            }
                        }
                        dictionary[dictionary.Keys.ElementAt(i - 1)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i - 1)], @"(\S+)", @"$1)");
                    }
                }
            }
        }
        public static void UpdateArrayWithDictionaryValues(string[] sqlText, Dictionary<int, string> dictionary)
        {
            for (int i = 0; i < sqlText.Length; i++)
            {
                if (dictionary.ContainsKey(i))
                {
                    sqlText[i] = dictionary[i];
                }
            }
        }
    }
}
