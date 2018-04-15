using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication2.Helpers;

namespace ConsoleApplication2.Getters
{
    internal class ContentGetter
    {
        /// <summary>
        /// С помощью данного метода получаем директиву crawl-delay
        /// </summary>
        /// <param name="link">Ссылка на robots.txt</param>
        /// <returns></returns>
        public static async Task<string> GetCrawlDelay(string link)
        {
            var html = "";
            var pattern = "crawl-delay:(.*)";
            var obRgx = new Regex(pattern, RegexOptions.IgnoreCase);
            
            try
            {
                var crawlDelay = "";
                
                html = await new WebClient().DownloadStringTaskAsync(link);
                html = html.Trim();
                
                foreach (Match match in obRgx.Matches(html))
                {
                    crawlDelay = match.Groups[1].ToString().Trim().Replace(".", ",");
                }

                if (crawlDelay.Length > 0)
                {
                    return crawlDelay;
                }
                
            }
            catch (WebException err)
            {
                Console.Write("Error - " + err.Message);
            }

            return "0";
        }
        
        /// <summary>
        /// Метод получает HTML страницы
        /// </summary>
        /// <param name="link">URL куда посылать запрос</param>
        /// <returns>HTML код страницы</returns>
        private async Task<string> GetPageHtml(string link)
        {
            var html = "";
            try
            {
                html = await new WebClient().DownloadStringTaskAsync(link);
                html = html.Trim();
                
                return html;
            }
            catch (WebException err)
            {
                Console.Write("Error - " + err.Message);
            }

            return html;
        }

        /// <summary>
        /// С помощью данного метода получаем контент с сайта и сохраняем в папку
        /// </summary>
        /// <param name="linkList">Список с ссылками</param>
        /// <param name="pathName">Путь, где сохранять файлы</param>
        /// <returns>Полный список ссылок сайта</returns>
        public IEnumerable<string> GetContent(IEnumerable<string> linkList, string pathName, double linkForFindCrawlDelay)
        {
            var domainsList = linkList.ToList();
            char[] arSymbolForReplace = {'/', ':', '?'};

            foreach (var link in domainsList)
            {
                var bGoNext = false;
                var fileName = Helper.ReplaceSymbolInString(link, arSymbolForReplace);
                var domain = domainsList.First();
                var lastDomainSymbol = domain.Substring(domain.Length - 1);

                if (File.Exists(pathName + "\\" + fileName + ".html"))
                {
                    continue;
                }

                Console.WriteLine(link);

                if (linkForFindCrawlDelay > 0)
                    Thread.Sleep(TimeSpan.FromSeconds(linkForFindCrawlDelay));
              
                var html = GetPageHtml(link).Result;
                
                File.WriteAllText(pathName + "\\" + fileName + ".html", html);

                if (lastDomainSymbol == "/")
                    domain = domain.Remove(domain.Length - 1);

                // ищем ссылки в полученном html
                var pattern = "<a href=\"(" + domain + ")?(?!//)/(.*?)\"";
                var obRgx = new Regex(pattern);

                foreach (Match match in obRgx.Matches(html))
                {
                    var findedLink = domain + match.Groups[1];

                    if (match.Groups[2] != null)
                        findedLink = domainsList.First() + match.Groups[2];

                    if (domainsList.Contains(findedLink)) continue;

                    // ограничение по кол-ву ссылок. Не больше 500
                    if (domainsList.Count <= 500)
                    {
                        bGoNext = true;
                        domainsList.Add(findedLink);
                    }
                }

                if (bGoNext)
                    return GetContent(domainsList, pathName, linkForFindCrawlDelay);
            }

            return domainsList;
        }
    }
}
