using C9ISM.Scheduler.Helpers;
using Microsoft.Extensions.Hosting;
using MultiTaskingWinService.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTaskingWinService.Services
{
    public class UpdateOtherParametersJob : IHostedService, IDisposable
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => UpdateOtherParameters(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours((int)ServiceInterval.OneHour));

            return Task.CompletedTask;
        }
        public async Task UpdateOtherParameters()
        {
            await UpdatePortFolio(new BulkDealHelper());
        }

        private async Task UpdatePortFolio(BulkDealHelper _bulkDealServices)
        { 
            await _bulkDealServices.UpdatePortFoliOtherColumns();
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