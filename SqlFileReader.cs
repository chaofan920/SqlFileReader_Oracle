using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace SqlFileReader
{
    internal static class SqlFileReader
    {

        // 将SQL文件的内容（可见字符和不可见字符）存入到一个List中
        public static List<string> ReadSqlFileToList(string filePath)
        {
            string sql = Regex.Replace(File.ReadAllText(filePath), @"(\r\n|\t|\s+)", @"~$1~", RegexOptions.IgnoreCase);
            string sqlmod = Regex.Replace(sql, @"~~", @"~", RegexOptions.IgnoreCase);
            List<string> sqloutput = sqlmod.Split('~').ToList();

            for (int i = 0; i < sqloutput.Count; i++)
            {
                // 将换行符和制表符转义
                sqloutput[i] = sqloutput[i].Replace("\r\n", "\\r\\n").Replace("\t", "\\t");
            }
            return sqloutput;
        }

        // 将换行符和制表符还原
        public static string ListToSqlText(List<string> sql)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string statement in sql)
            {
                sb.Append(statement);
            }
            string sqlmod = sb.ToString().Replace("\\r\\n", "\r\n").Replace("\\t", "\t");
            return sqlmod;
        }

        // 取得可见字符（除换行符、空格、制表符tab等）的内容放入到词典中
        // Key为数组下标、Value为对应的可见字符内容
        public static Dictionary<int, string> GetNoEmptyStrings(List<string> sqlList)
        {
            var result = new Dictionary<int, string>();
            for (int i = 0; i < sqlList.Count; i++)
            {
                string str = sqlList[i];
                // 如果不是不可见字符，则将其存入到词典中
                if (!string.IsNullOrWhiteSpace(str) && str != "\\r\\n" && str != "\\t")
                {
                    result[i] = str;
                }
            }
            return result;
        }


        // 将词典中符合条件的（||、对应的前后行）替换为修改后的内容
        public static List<int> DictionaryContents(Dictionary<int, string> dictionary)
        {
            List<int> modLines = new List<int>();

            int i;
            for (i = 0; i < dictionary.Count; i++)
            {
                int key = dictionary.Keys.ElementAt(i);
                if (dictionary[key] == "||")
                {
                    // 如果当前key的值为“||”，则将前一个key的value值修改
                    dictionary[dictionary.Keys.ElementAt(i - 1)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i - 1)], @"(\S+)", @"CONCAT($1");
                    // 如果当前key的值为“||”，则将当前key的value值修改
                    dictionary[dictionary.Keys.ElementAt(i)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i)], @"(\|\|)", @",");

                    // 将修改过的key值存入到List中
                    modLines.Add(dictionary.Keys.ElementAt(i - 1));
                    modLines.Add(dictionary.Keys.ElementAt(i));

                    i += 2;
                    if (i < dictionary.Count)
                    {
                        while (dictionary[dictionary.Keys.ElementAt(i)] == "||")
                        {
                            dictionary[dictionary.Keys.ElementAt(i)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i)], @"\|\|", @",");
                            modLines.Add(dictionary.Keys.ElementAt(i));
                            i += 2;
                            if (i >= dictionary.Count)
                            {
                                break;
                            }
                        }
                        dictionary[dictionary.Keys.ElementAt(i - 1)] = Regex.Replace(dictionary[dictionary.Keys.ElementAt(i - 1)], @"(\S+)", @"$1)");
                        modLines.Add(dictionary.Keys.ElementAt(i - 1));
                    }
                }
            }
            return modLines;
        }

        // 将修改后的词典中的内容保存到一个新的List中
        public static List<string> UpdateSqlTextWithDictionaryValues(List<string> sqlTextBefore, Dictionary<int, string> dictionary)
        {
            // create a copy of sqlTextAfter
            string[] sqlTextAfterCopy = sqlTextBefore.ToArray();

            for (int i = 0; i < sqlTextAfterCopy.Length; i++)
            {
                if (dictionary.ContainsKey(i))
                {
                    sqlTextAfterCopy[i] = dictionary[i];
                }
            }
            return new List<string>(sqlTextAfterCopy);
        }

        // 将注释添加到修改后的List中
        public static void InsertLineToList(List<string> sqlTextAfter, List<string> sqlTextBefore, List<int> modLines)
        {
            string commLine = "";
            int k = 0;

            for (int i = 0; i < modLines.Count; i++)
            {
                for (int j = modLines[i]; j > 0; j--)
                {
                    if (sqlTextBefore[j - 1] != "\\r\\n")
                    {
                        // 将换行符之后的、j行之前的拼接，作为修改前代码的注释
                        commLine = sqlTextBefore[j - 1] + "-- " + sqlTextBefore[j] + "\\r\\n";
                    }
                    else
                    {
                        j += k;
                        // 如果对修改后的List插入了一行，下一次再插入时基于上一次的插入行应该 +1
                        k++;
                        sqlTextAfter.Insert(j, commLine);
                        break;
                    }
                }
            }
        }

    }
}
