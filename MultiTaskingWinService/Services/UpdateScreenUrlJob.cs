using C9ISM.Scheduler.Helpers;
using Microsoft.Extensions.Hosting;
using MultiTaskingWinService.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTaskingWinService.Services
{
    public class UpdateScreenUrlJob : IHostedService, IDisposable
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => UpdateScreenerURL(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours((int)ServiceInterval.OneHour));

            return Task.CompletedTask;
        }
        public async Task UpdateScreenerURL()
        {
            await UpdateScreenerUrl(new BulkDealHelper());
        }

        private async Task UpdateScreenerUrl(BulkDealHelper _bulkDealServices)
        { 
            await _bulkDealServices.UpdateURL();
            //await _bulkDealServices.UpdateTickerTapdetail();
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