using C9ISM.Scheduler.Helpers;
using C9ISM.Scheduler.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MultiTaskingWinService.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTaskingWinService.Services
{

  /// <summary>
  /// 
  /// </summary>
   public class DealJobService : IHostedService, IDisposable
    {

        public static readonly C9BasicLogger _logger = new C9BasicLogger();
        /// <summary>
        /// Enum for Deal type 
        /// </summary>
        public enum DealType
        {
            BulkDeal,
            BlockDeal
        }

        /// <summary>
        /// Enum for exchange name
        /// </summary>
        public enum ExchangeName
        {
            NSE,
            BSE
        }

        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => DealJob(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours((int)ServiceInterval.TwentyFourHours));

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task DealJob()
        {
            await SaveDeals(new BulkDealHelper());
        }

        private async Task SaveDeals(BulkDealHelper _bulkDealServices)
        {
            //NSE bulk deals 
            await _bulkDealServices.SaveDeals(CommonConstant.BulkDealNse,
                CommonConstant.BulkBlockDealXpath, ExchangeName.NSE.ToString(),
                DealType.BulkDeal.ToString(), _logger);

            //NSE block deals
            await _bulkDealServices.SaveDeals(CommonConstant.BlockDealNse,
               CommonConstant.BulkBlockDealXpath, ExchangeName.NSE.ToString(),
               DealType.BlockDeal.ToString(), _logger);


            //BSE bulk deals
            await _bulkDealServices.SaveDeals(CommonConstant.BulkDealBse,
                CommonConstant.BulkBlockDealXpath, ExchangeName.BSE.ToString(), DealType.BulkDeal.ToString(),
                _logger);

            //BSE  block deals 
            await _bulkDealServices.SaveDeals(CommonConstant.BlockDealBse,
               CommonConstant.BulkBlockDealXpath, ExchangeName.BSE.ToString(), DealType.BlockDeal.ToString(),
               _logger);
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