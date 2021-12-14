using System.Threading.Tasks;
using System;
using C9ISM.Scheduler.Logger;
using System.Net;
using System.IO.Compression;
using System.Threading;
using System.IO;
using System.Data;
using DatabaseHandlerLibrary;
using C9ISM.Scheduler.Entities;

namespace C9ISM.Scheduler.Helpers
{
    public class StockHistoryHelper
    {
        public async Task SaveData(string url,string exchange, C9BasicLogger _logger)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Processing {0} ...", exchange);

            DateTime date = DateTime.Now;
            string dateToday = date.ToString("d");
            DayOfWeek day = DateTime.Now.DayOfWeek;

            // compare enums
            if ((day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday))
            {
                Console.WriteLine(dateToday + " is a weekend");
            }
            else
            {
                string fname;
                if (exchange.ToLower() == "nse")
                {
                    int year = date.Year;
                    string month = date.ToString("MMM").ToUpper();
                    fname = "cm" + date.AddDays(-1).ToString("ddMMMyyyy").ToUpper() + "bhav";
                    url = url.Replace("cmstrdatebhav", fname).Replace("month", month).Replace("year", year.ToString());
                }
                else
                {
                    fname = "EQ" + date.AddDays(-1).ToString("ddMMyy");
                    url = url.Replace("EQstrdate", fname);
                }
                //Check file exist on server
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 30000;

                int flag;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    flag = 1;
                }
                catch
                {
                    flag = -1;
                }

                if (flag == 1)
                {
                    Console.WriteLine("File downloading.... {0}", fname);

                    DownLoadAndExtractFile( url, fname);

                    string csvFilePath = CommonConstant.ZipPath+ fname + "\\" + fname + ".csv";

                    if (File.Exists(csvFilePath))
                    {
                        string ReadCSV = File.ReadAllText(csvFilePath);
                        //spliting row after new line  
                        int count = 0;
                        foreach (string csvRow in ReadCSV.Split('\n'))
                        {
                            count++;
                            if (!string.IsNullOrEmpty(csvRow) && count > 1)
                            {
                                var arr = csvRow.Replace(",\r", "").Split(",");
                                if(arr.Length >= 13)
                                {
                                    if(exchange.ToLower() == "nse")
                                        await SaveNSE(exchange, arr, fname, _logger);
                                    else
                                        await Save( exchange, arr, fname, _logger);
                                }
                            }
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Processed {0}[success : {1}]", exchange, count - 1);
                        _logger.LogWrite(string.Format("Processed {0}[success : {1}]", exchange, count - 1));
                    }
                }
                else
                {
                    Console.WriteLine("File Not Found!!!");
                }
            }  
        }

        public async Task Save(string exchange,string[] arr,string FName, C9BasicLogger _logger)
        {
            if (arr.Length >= 13)
            {
                try
                {
                    var sqlParameters = new
                    {
                        Code = arr[0],
                        Name = arr[1].Trim().ToString(),
                        Group = arr[2],
                        Type = arr[3],
                        Open = Convert.ToDecimal(arr[4]),
                        High = Convert.ToDecimal(arr[5]),
                        Low = Convert.ToDecimal(arr[6]),
                        Close = Convert.ToDecimal(arr[7]),
                        Last = Convert.ToDecimal(arr[8]),
                        PrevClose = Convert.ToDecimal(arr[9]),
                        NoOfTrades = Convert.ToDecimal(arr[10]),
                        NoOFShares = Convert.ToDecimal(arr[11]),
                        NetTurnOver = Convert.ToDecimal(arr[12]),
                        Exchange = exchange,
                        StockFileName = FName
                    };
                    DatabaseHandler<CommonDBResponse> dbHandlerObj = new DatabaseHandler<CommonDBResponse>();
                    await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_saveStockHistoricalData");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Stock added to database for : {0}", arr[1]);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not able to add to database : {0}", arr[1]);
                    _logger.LogWrite(string.Format("Error : Not able to add to database : {0}", arr[1]));
                }
            }
        }

        public async Task SaveNSE(string exchange, string[] arr, string FName, C9BasicLogger _logger)
        {
            if (arr.Length >= 13)
            {
                try
                {
                    var sqlParameters = new
                    {
                        Name = arr[0],
                        Series = Convert.ToString(arr[1].Trim()),
                        Open = Convert.ToDecimal(arr[2]),
                        High = Convert.ToDecimal(arr[3]),
                        Low = Convert.ToDecimal(arr[4]),
                        Close = Convert.ToDecimal(arr[5]),
                        Last = Convert.ToDecimal(arr[6]),
                        PrevClose = Convert.ToDecimal(arr[7]),
                        TotalTradQuantity = Convert.ToDecimal(arr[8]),
                        TotalTradVal = Convert.ToDecimal(arr[9]),
                        TimeStamp = arr[10],
                        TotalTrades = Convert.ToInt32(arr[11]),
                        ISIN = Convert.ToString(arr[12]),
                        Exchange = exchange,
                        StockFileName = FName
                    };
                    DatabaseHandler<CommonDBResponse> dbHandlerObj = new DatabaseHandler<CommonDBResponse>();
                    await dbHandlerObj.SaveData(CommandType.StoredProcedure, sqlParameters, "sp_saveNSEStockHistoricalData");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Stock added to database for : {0}", arr[1]);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not able to add to database : {0}", arr[1]);
                    _logger.LogWrite(string.Format("Error : Not able to add to database : {0}", arr[1]));
                }
            }
        }

        public void DownLoadAndExtractFile(string url ,string fname)
        {
            string BaseZipDirPath = CommonConstant.ZipPath + fname;
            string zipFilePath = BaseZipDirPath + ".zip";

            //Download
            using WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatiable; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadFileAsync(new Uri(url), zipFilePath);
            Thread.Sleep(2000);
            Console.WriteLine("File downloaded.... {0}", fname);

            //Unzip
            Console.WriteLine("File unziping.... {0}", fname);
            if (Directory.Exists(BaseZipDirPath))
                Directory.Delete(BaseZipDirPath, true);

            ZipFile.ExtractToDirectory(zipFilePath, BaseZipDirPath);
            Console.WriteLine("File unziped.... {0}", fname);
        }

    }
}
