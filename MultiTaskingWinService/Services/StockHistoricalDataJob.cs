using C9ISM.Scheduler.Helpers;
using C9ISM.Scheduler.Logger;
using Microsoft.Extensions.Hosting;
using MultiTaskingWinService.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTaskingWinService.Services
{
    public class StockHistoricalDataJob : IHostedService, IDisposable
    {
        private Timer _timer;

        public static readonly C9BasicLogger _logger = new C9BasicLogger();
        /// <summary>
        /// Enum for Exchange Name 
        /// </summary>
        public enum ExchangeName
        {
            NSE,
            BSE
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) =>  GetStockHistoryData(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromDays((int)ServiceInterval.OneHour));

            return Task.CompletedTask;
        }
        public async Task GetStockHistoryData()
        {
            await StockHistoryData(new StockHistoryHelper());
        }

        private async Task StockHistoryData(StockHistoryHelper _historyHelper)
        {
            //BSE
            await _historyHelper.SaveData(CommonConstant.BlockHistoryBse, ExchangeName.BSE.ToString(), _logger);
            //NSE
            await _historyHelper.SaveData(CommonConstant.BlockHistoryNse, ExchangeName.NSE.ToString(), _logger);
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