namespace ConsoleApplication2.Helpers
{
    public class Helper
    {
        /// <summary>
        /// Метод заменяет в строке переданные символы, на символ -
        /// </summary>
        /// <param name="stringForReplace">Строка, в которой нужно заменить символы</param>
        /// <param name="arSymbolForReplace">Символы для замены</param>
        /// <returns></returns>
        public static string ReplaceSymbolInString(string stringForReplace, char[] arSymbolForReplace)
        {
            foreach (var symbol in arSymbolForReplace)
            {
                stringForReplace = stringForReplace.Replace(symbol, '-');
            }

            return stringForReplace;
        }
    }
}