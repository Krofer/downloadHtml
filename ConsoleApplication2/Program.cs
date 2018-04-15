using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ConsoleApplication2.Getters;
using ConsoleApplication2.Helpers;

namespace ConsoleApplication2
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var obGetter = new ContentGetter();
            var domainsList = FileGetter.GetDomainsFromFile();
            var countDomains = domainsList.Count();
            var obTasks = new Task[countDomains];
            char[] arSymbolForReplace = {'/', ':', '?'};

            Console.WriteLine("Введите путь или нажмите \"Enter\"");
            
            var pathForResult = Console.ReadLine();
            
            if (string.IsNullOrEmpty(pathForResult))
            {
                pathForResult = @"D:\HtmlPages";
            }

            if (pathForResult.Substring(pathForResult.Length - 1) == "\\")
            {
                pathForResult += "\\";
            }

            Directory.CreateDirectory(pathForResult);

            for (var i = 0; i < countDomains; i++)
            {
                var domain = domainsList.ElementAt(i);
                var changedDomain = Helper.ReplaceSymbolInString(domain, arSymbolForReplace);
                var linkList = new HashSet<string> {domain};
                var pathForElements = pathForResult + "\\" + changedDomain;
                var linkCrawlDelay = Convert.ToDouble(ContentGetter.GetCrawlDelay(domain + "/" + "robots.txt").Result);
                
                Directory.CreateDirectory(pathForElements);
                
                if (domain.Substring(domain.Length - 1) == "/")
                {
                    linkCrawlDelay = Convert.ToDouble(ContentGetter.GetCrawlDelay(domain + "robots.txt").Result);
                }
                
                obTasks[i] = new Task(() => obGetter.GetContent(linkList, pathForElements, linkCrawlDelay));
                obTasks[i].Start();
            }

            Task.WaitAll(obTasks);

            Console.WriteLine("Страницы с сайтов были загружены");

            Console.ReadLine();
        }
    }
}