namespace SqlFileReader
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            //MainClass mainClass = new MainClass();
            string filePath = @"..\..\..\example.sql";
            string filePathMod = @"..\..\..\examplemod.sql";
            List<string> sqlTextBefore = SqlFileReader.ReadSqlFileToList(filePath);

            Dictionary<int, string> dictionary = SqlFileReader.GetNoEmptyStrings(sqlTextBefore);

            List<int> modLines = SqlFileReader.DictionaryContents(dictionary);
            List<string> sqlTextAfter = SqlFileReader.UpdateSqlTextWithDictionaryValues(sqlTextBefore, dictionary);
            SqlFileReader.InsertLineToList(sqlTextAfter, sqlTextBefore,modLines);
            string modsql = SqlFileReader.ListToSqlText(sqlTextAfter);
            File.WriteAllText(filePathMod, modsql); // 将内容写入文件

            Console.Write(modsql);
            //Console.ReadLine();
        }
    }
}