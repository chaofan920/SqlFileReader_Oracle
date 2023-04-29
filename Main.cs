namespace SqlFileReader
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            //MainClass mainClass = new MainClass();
            string filePath = @"..\..\..\example.sql";
            string[] sqlText = SqlFileReader.ReadSqlFile(filePath);

            Dictionary<int, string> dictionary = SqlFileReader.GetNonEmptyStrings(sqlText);

            SqlFileReader.PrintDictionaryContents(dictionary);
            SqlFileReader.UpdateArrayWithDictionaryValues(sqlText, dictionary);
            SqlFileReader.ReconstructSqlFile(sqlText);

            Console.ReadLine();
        }
    }
}