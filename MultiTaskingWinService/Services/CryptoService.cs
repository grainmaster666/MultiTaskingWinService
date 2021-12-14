using C9ISM.Scheduler.Helpers;
using C9ISM.Scheduler.Logger;
using DatabaseHandlerLibrary;
using Microsoft.Extensions.Hosting;
using MultiTaskingWinService.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTaskingWinService.Services
{
    public class CryptoService : IHostedService, IDisposable
    {
        public static readonly C9BasicLogger _logger = new C9BasicLogger();

        private Timer _timer;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => SaveCrypto(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds((int)ServiceInterval.NinghtySecond));

            return Task.CompletedTask;
        }
        public async Task SaveCrypto()
        {
            await Save();
        }
        
        public async Task Save()
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Crypto data processing ...");
            DatabaseHandler<CryptoDto> dbHandlerObj = new DatabaseHandler<CryptoDto>();
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(CommonConstant.CryptoAPI);
                string apiresponse = await response.Content.ReadAsStringAsync();
                var dsrilizeObj = JsonConvert.DeserializeObject<List<CryptoDto>>(apiresponse);
                
                if (dsrilizeObj != null && dsrilizeObj.Count > 0)
                {
                    DataTable dt = null;
                    try
                    {
                        dt = ToDataTable<CryptoDto>(dsrilizeObj);
                        if(dt.Rows.Count > 0)
                            dt.Columns.Remove("roi");
                        var parameters = new { data = dt };
                        await dbHandlerObj.SaveData(CommandType.StoredProcedure, parameters, "sp_tblCrypto_insert");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Crypto data have been inserted successfully!!");
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                        Console.ForegroundColor = ConsoleColor.Red;
                        new C9BasicLogger().LogWrite(string.Format("Error : Not able to insert crypto to database : {0}", ex.StackTrace));
                    }
                    finally
                    {
                        dt.Clear();
                    }
                    Console.ReadKey();
                }
            } 
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
