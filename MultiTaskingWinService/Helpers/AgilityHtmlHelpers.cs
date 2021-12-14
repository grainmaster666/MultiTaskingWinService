using C9ISM.Scheduler.Entities;
using C9ISM.Scheduler.Logger;
using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;

namespace C9ISM.Scheduler.Helpers
{
    public static class AgilityHtmlHelpers
    {
        static ScrapingBrowser _scrapingbrowser = new ScrapingBrowser();       
        
        /// <summary>
        /// It will retun html from inputed URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static HtmlNode GetHtml(string url)
        {
            WebPage webPage = _scrapingbrowser.NavigateToPage(new Uri(url));
            return webPage.Html;
        }

        /// <summary>
        /// This method is used to return html table data for block and bulk deal
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static List<TableRow> GetTableContent(string url,string xPath, C9BasicLogger _logger)
        {
           List<TableRow> tableContents = new List<TableRow>();
            HtmlNode pageContent = GetHtml(url);
            HtmlNodeCollection rowOfTable = pageContent.SelectNodes(xPath);
            
            foreach (HtmlNode el in rowOfTable)
            {
                if (!string.IsNullOrEmpty( el.ChildNodes[1].InnerText.Trim()) 
                    && el.ChildNodes[1].InnerText.Trim().ToLower()!="&nbsp;")
                {
                    try
                    {
                        TableRow tableRow = new TableRow();
                        tableRow.TransactionDate = DateTime.Parse(el.ChildNodes[1].InnerText);
                        tableRow.CompanyName = el.ChildNodes[3].InnerText;
                        tableRow.Client = el.ChildNodes[5].InnerText;
                        tableRow.TransactionType = el.ChildNodes[7].InnerText;
                        tableRow.Quantity = int.Parse(el.ChildNodes[9].InnerText.Replace(",", ""));
                        tableRow.TradedPrice = decimal.Parse(el.ChildNodes[11].InnerText);
                        tableRow.ClosedPrice = decimal.Parse(el.ChildNodes[13].InnerText);
                        tableContents.Add(tableRow);
                    }
                    catch(Exception ex)
                    {                        
                        _logger.LogWrite(string.Format("Exception:{0}",ex.StackTrace));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            return tableContents;
        }
    }
}
