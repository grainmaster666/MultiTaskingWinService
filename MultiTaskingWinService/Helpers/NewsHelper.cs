using C9ISM.Scheduler.Logger;
using DatabaseHandlerLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace C9ISM.Scheduler.Helpers
{
    public class NewsHelper
    {
        public async Task SaveNews()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ticker Tap News processing ...");
            DatabaseHandler<Company> dbHandlerObj = new DatabaseHandler<Company>();

            News lstNews =  LoadJson();
            if(lstNews != null && lstNews.success && lstNews.data.total > 0)
            {
                foreach (var item in lstNews.data.items)
                {
                    var sqlParameters = new
                    {
                        CID = 3519,
                        CName = "ITC",
                        Title = item.title,
                        Link =  item.link,
                        Newsdate = item.date,
                        Description = item.description,
                        Image = item.image,
                        FeedType = item.feed_type

                    };

                    await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_SaveNews");
                }
            }
            
         
        }

        public News LoadJson()
        {
            string JPath = @"./JSON/response.json";
            using (StreamReader r = new StreamReader(JPath))
            {
                string json = r.ReadToEnd();
                News items = JsonConvert.DeserializeObject<News>(json);
                return items;
            }
        }

        public async Task UpdateTickerTapdetail()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Update screener url column processing ...");
            DatabaseHandler<Company> dbHandlerObj = new DatabaseHandler<Company>();
            int successCount = 0;
            int failureCount = 0;

            List<Company> lstCompany = await dbHandlerObj.GetData(CommandType.StoredProcedure, null, "sp_GetAllCompany");
            foreach (var company in lstCompany)
            {
                try
                {
                    string ScreenerApi = CommonConstant.TickertapeURL?.Replace("cname", company.Name.Replace("&", "%26"));

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(ScreenerApi);
                        string apiresponse = await response.Content.ReadAsStringAsync();

                        Tickertape myDeserializedClass = JsonConvert.DeserializeObject<Tickertape>(apiresponse);
                        if (myDeserializedClass.success && myDeserializedClass.data.total > 0)
                        {
                            bool isMatched = false;
                            foreach (var stock in myDeserializedClass.data.stocks)
                            {
                                if (Convert.ToString(stock.match)?.ToLower() == "exact")
                                {
                                    isMatched = true;
                                    var sqlParameters = new
                                    {
                                        TPSid = stock.sid,
                                        TPurl = stock.slug,
                                        TPName = stock.name,
                                        company.Code,
                                        company.ID
                                    };
                                    await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_updateTickerTapedetail");
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("Stock Tikertape detail has been updated : {0}", company.Name);
                                    successCount++;
                                    break;
                                }
                            }
                            if (!isMatched)
                            {
                                var sqlParameters = new
                                {
                                    companyID = company.ID
                                };
                                await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_UpdateCompanyNotFoundTP");
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine("CompanyStockTP not found for : {0}", company.Name);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not able to update Tickertape detail to database : {0}", company.Name);
                    new C9BasicLogger().LogWrite(string.Format("Error : Not able to update Tickertape detail to database : {0} {1}", company.Name, ex.StackTrace));
                    failureCount++;
                }
            }
        }
    }
}
