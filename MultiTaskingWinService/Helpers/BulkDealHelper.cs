using C9ISM.Scheduler.Entities;
using DatabaseHandlerLibrary;
using System.Data;
using System.Threading.Tasks;
using System;
using C9ISM.Scheduler.Logger;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace C9ISM.Scheduler.Helpers
{
    public class BulkDealHelper
    {
        public async Task SaveDeals(string url, string xPath , 
            string exchange,string type, C9BasicLogger _logger)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Started processing {0} {1} ...", exchange, type);
            DatabaseHandler<CommonDBResponse> dbHandlerObj = new DatabaseHandler<CommonDBResponse>();
            int successCount=0;
            int failureCount = 0;
            foreach (var tableRow in AgilityHtmlHelpers.GetTableContent(url, xPath, _logger))
            {
                
                try
                {
                    var sqlParameters = new
                    {
                        tableRow.CompanyName,
                        tableRow.Client,
                        tableRow.TransactionType,
                        tableRow.Quantity,                        
                        tableRow.TradedPrice,
                        tableRow.ClosedPrice,
                        tableRow.TransactionDate,
                        Type = type,
                        Exchange = exchange
                    };
                    await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_saveBlockBulkDeals");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Stock added to database : {0}", tableRow.CompanyName);                    
                    successCount++;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not able to add to database : {0}", tableRow.CompanyName);
                    _logger.LogWrite(string.Format("Error : Not able to add to database : {0}", tableRow.CompanyName));
                    failureCount++;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Processed {2} {3} [success : {0} Failure : {1}]", successCount, failureCount,exchange,type);
            _logger.LogWrite(string.Format("Processed {2} {3} [success : {0} Failure : {1}]", 
                successCount, failureCount, exchange, type));
        }

        public async Task UpdatePortFoliOtherColumns()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Update Other PortFolio columns processing ...");
            DatabaseHandler<Company> dbHandlerObj = new DatabaseHandler<Company>();
            int successCount = 0;
            int failureCount = 0;

            List<Company> lstCompany =  await dbHandlerObj.GetData(CommandType.StoredProcedure, null, "sp_GetRegPortFolioCompanydetail");
            foreach(var row in lstCompany)
            {
                try
                {
                    string SMAPILink = CommonConstant.SMAPILink?.Replace("CompanyCode", row.Code);
                    string VolDeliveryAPILink = CommonConstant.VolDeliveryAPILink?.Replace("CompanyCode", row.Code);

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(SMAPILink);
                        string apiJSON = await response.Content.ReadAsStringAsync();
                        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(apiJSON);

                        var volApiResponse = await httpClient.GetAsync(VolDeliveryAPILink);
                        string volDeliveryJSON = await volApiResponse.Content.ReadAsStringAsync();
                        Root2 VolDeserializedClass = JsonConvert.DeserializeObject<Root2>(volDeliveryJSON);

                        decimal sma5 = 0, sma10 = 0, sma20 = 0, sma50 = 0, sma100 = 0, sma200 = 0, ema5 = 0, ema10 = 0, ema50 = 0, ema100 = 0, ema200 = 0,
                            s1 = 0, s2 = 0, s3 = 0, r1 = 0, r2 = 0, r3 = 0;
                        string deliveryAverageMonth = "", deliveryAverageWeek = "", deliveryYesterday = "";

                        if(myDeserializedClass.data.sma != null)
                        {
                            foreach (var item in myDeserializedClass.data.sma)
                            {
                                if (Convert.ToInt32(item.key) == 5)
                                    sma5 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 10)
                                    sma10 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 20)
                                    sma20 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 50)
                                    sma50 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 100)
                                    sma100 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 200)
                                    sma200 = Convert.ToDecimal(item.value);
                            }
                        }

                        if (myDeserializedClass.data.ema != null)
                        {
                            foreach (var item in myDeserializedClass.data.ema)
                            {
                                if (Convert.ToInt32(item.key) == 5)
                                    ema5 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 10)
                                    ema10 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 50)
                                    ema50 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 100)
                                    ema100 = Convert.ToDecimal(item.value);
                                if (Convert.ToInt32(item.key) == 200)
                                    ema200 = Convert.ToDecimal(item.value);
                            }
                        }

                        if (myDeserializedClass.data.pivotLevels != null)
                        {
                            foreach (var item in myDeserializedClass.data.pivotLevels)
                            {
                                if (Convert.ToString(item.key).ToLower() == "classic")
                                {
                                    r1 = Convert.ToDecimal(item.pivotLevel.r1);
                                    r2 = Convert.ToDecimal(item.pivotLevel.r2);
                                    r3 = Convert.ToDecimal(item.pivotLevel.r3);
                                    s1 = Convert.ToDecimal(item.pivotLevel.s1);
                                    s2 = Convert.ToDecimal(item.pivotLevel.s2);
                                    s3 = Convert.ToDecimal(item.pivotLevel.s3);
                                }
                            }
                        }
                        
                        if(VolDeserializedClass != null)
                        {
                            deliveryYesterday = Convert.ToString(VolDeserializedClass.data.stock_price_volume_data.volume.Yesterday.delivery_display_text);
                            deliveryAverageWeek = Convert.ToString(VolDeserializedClass.data.stock_price_volume_data.volume._1WeekAvg.delivery_display_text);
                            deliveryAverageMonth = Convert.ToString(VolDeserializedClass.data.stock_price_volume_data.volume._1MonthAvg.delivery_display_text);

                            deliveryYesterday = deliveryYesterday != "" ? Regex.Replace(deliveryYesterday, @"(.*?\()(.*?)(\))", @"$2", RegexOptions.IgnoreCase) : null;
                            deliveryAverageWeek = deliveryAverageWeek != "" ? Regex.Replace(deliveryAverageWeek, @"(.*?\()(.*?)(\))", @"$2", RegexOptions.IgnoreCase) : null;
                            deliveryAverageMonth = deliveryAverageMonth != "" ? Regex.Replace(deliveryAverageMonth, @"(.*?\()(.*?)(\))", @"$2", RegexOptions.IgnoreCase) : null;
                        }
                        

                        var sqlParameters = new
                        {
                            CompanyId = row.ID,
                            SMA5 = sma5,
                            SMA10 = sma10,
                            SMA20 = sma20,
                            SMA50 = sma50,
                            SMA100 = sma100,
                            SMA200 = sma200,
                            EMA5 = ema5,
                            EMA10 = ema10,
                            EMA50 = ema50,
                            EMA100 = ema100,
                            EMA200 = ema200,
                            R1 = r1,
                            R2 = r2,
                            R3 = r3,
                            S1 = s1,
                            S2 = s2,
                            S3 = s3,
                            DeliveryAverageMonth = deliveryAverageMonth,
                            DeliveryAverageWeek = deliveryAverageWeek,
                            DeliveryYesterday = deliveryYesterday

                        };
                        await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_UpdatePortFolioOtherColums");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Stock Other parameter updated to database : {0}", row.Code);
                        successCount++;
                    }              
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not able to update to database : {0}", row.ID);
                    new C9BasicLogger().LogWrite(string.Format("Error : Not able to update to database : {0}", row.Code));
                    failureCount++;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[success : {0} Failure : {1}]", successCount, failureCount);
            new C9BasicLogger().LogWrite(string.Format("[success : {0} Failure : {1}]",
                successCount, failureCount));
        }

        public async Task UpdateURL()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Update screener url column processing ...");
            DatabaseHandler<Company> dbHandlerObj = new DatabaseHandler<Company>();
            int successCount = 0;
            int failureCount = 0;

            List<Company> lstCompany = await dbHandlerObj.GetData(CommandType.StoredProcedure, null, "sp_GetAllCompany");
            foreach (var row in lstCompany)
            {
                try
                {
                    string ScreenerApi = CommonConstant.ScreenApi?.Replace("cname", row.Name.Replace("&","%26"));

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(ScreenerApi);
                        string apiresponse = await response.Content.ReadAsStringAsync();
                        if (!apiresponse.Contains("<body>"))
                        {
                            var dsrilizeObj = JsonConvert.DeserializeObject<List<ScreenerURL>>(apiresponse);

                            string url = null;

                            int count = dsrilizeObj.Count;
                            if (count > 1)
                            {
                                foreach (var arr in dsrilizeObj)
                                {
                                    if (arr.name.ToLower().Contains(row.Name.ToLower().Replace(".","")))
                                    {
                                        url = arr.url;
                                        break;
                                    }
                                }
                            }
                            else
                                url = dsrilizeObj.Count > 0 ? dsrilizeObj[0].url : "";

                            var sqlParameters = new
                            {
                                row.ID,
                                row.Code,
                                ScreenerUrl = CommonConstant.BaseScreenUrl + url,
                                count
                            };
                            await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_updateScreenerUrl");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Screener URl has been updated in company table : {0}", CommonConstant.BaseScreenUrl + url);

                            successCount++;
                        }                         
                    }  
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not able to update screener url to database : {0}", row.Name);
                    new C9BasicLogger().LogWrite(string.Format("Error : Not able to update screener url to database : {0}{1}", row.Name,ex.StackTrace));
                    failureCount++;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[success : {0} Failure : {1}]", successCount, failureCount);
            new C9BasicLogger().LogWrite(string.Format("[success : {0} Failure : {1}]",
                successCount, failureCount));
        }

    }
}
