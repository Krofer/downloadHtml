using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApplication2.Getters
{
    public class FileGetter
    {
        /// <summary>
        /// С помощью данного метода получаем список доменов из файла.
        /// Пока из файла domains.txt, который лежит в bin/Debug
        /// </summary>
        /// <returns>Список доменов из файла</returns>
        public static IEnumerable<string> GetDomainsFromFile()
        {
            var domainsList = new HashSet<string>();

            using (var sr = new StreamReader("domains.txt", Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    domainsList.Add(line);
                }
            }

            return domainsList;
        }
    }
}