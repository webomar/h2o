using WorkerService.Conversion;

namespace WorkerService.Worker
{
    public class ImportWorker : BackgroundService
    {
        private readonly IConversionService _conversionService;
        private readonly ILogger<ImportWorker> _logger;

        public ImportWorker(
            IConversionService conversionService,
            ILogger<ImportWorker> logger)
        {
            _conversionService = conversionService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Start import & conversie");

                var employees = _conversionService.Convert();

                foreach (var employee in employees)
                {              
                    _logger.LogInformation(
                        "Employee verwerkt: {Id} ({Source})",
                        employee.ExternalId,
                        employee.Source);
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(5),
                    stoppingToken);
            }
        }
    }

}
