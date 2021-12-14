using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DatabaseHandlerLibrary
{
    public class DatabaseHandler<T> where T : class
    {
       readonly string connectionStr = GetConnection().GetSection("ConnectionStrings").GetSection("conStr").Value;
        public static IConfigurationRoot GetConnection()
        {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();

            return builder;

        }

        public async Task<T> SaveData(CommandType commandType,object sqlParameters,string dbObject)
        {
            
            using (var sqlConnection = new SqlConnection(connectionStr))
            {
                try
                {
                    sqlConnection.Open();                    
                    return (await sqlConnection.QueryAsync<T>(dbObject, sqlParameters , commandTimeout : 5000, commandType: commandType)).FirstOrDefault();                    
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exception: {0}", ex.Message);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }
        public async Task<List<T>> GetData(CommandType commandType, object sqlParameters, string dbObject)
        {
            using (var sqlConnection = new SqlConnection(connectionStr))
            {
                try
                {
                    sqlConnection.Open();
                    return (await sqlConnection.QueryAsync<T>(dbObject, sqlParameters, commandType: commandType)).ToList();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exception: {0}", ex.Message);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }
    }    
}