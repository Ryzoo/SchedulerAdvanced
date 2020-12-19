using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.CSV.Mappers;
using Application.CSV.Models;
using Application.FileRepository;
using Application.Requests;
using Application.Settings;
using Core.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Handlers
{
    public class ReadMailFileHandler : AsyncRequestHandler<ReadMailFileRequest>
    {
        private const int MaxLineToTake = 100;
        private readonly ILogger<ReadMailFileHandler> _logger;
        private readonly ICsvParserService _csvParserService;
        private readonly IOptions<CsvFilePathSettings> _settings;
        private readonly IFileRepository _fileRepository;
        private readonly IMediator _mediator;

        public ReadMailFileHandler(ILogger<ReadMailFileHandler> logger, ICsvParserService csvParserService,
            IOptions<CsvFilePathSettings> settings, IFileRepository fileRepository, IMediator mediator)
        {
            _logger = logger;
            _settings = settings;
            _fileRepository = fileRepository;
            _mediator = mediator;
            _csvParserService = csvParserService;
        }

        protected override async Task Handle(ReadMailFileRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handle ReadMailFileHandler");
            try
            {
                var repositoryContent = await _fileRepository.ReadFromJsonFile();
                var startReadLine = repositoryContent.ReadLineCount;
                var welcomeMailDataList = _csvParserService
                    .ReadCsvFile<WelcomeMailDataCsvModel, WelcomeMailDataCsvMapper>(_settings.Value
                        .WelcomeMailFilePath, startReadLine, MaxLineToTake)
                    .Select(WelcomeMailDataCsvModel.ToDomainModel)
                    .ToList();

                _logger.LogInformation($"Previous read {startReadLine} lines.");
                _logger.LogInformation($"Now we try read {welcomeMailDataList.Count} lines.");

                if (welcomeMailDataList.Count > 0)
                {
                    await _fileRepository.WriteToJsonFile( new FileRepositoryContent()
                    {
                        ReadLineCount = startReadLine + welcomeMailDataList.Count
                    });
                    
                    await _mediator.Send(new QueueScheduledMailRequest()
                    {
                        Mail = welcomeMailDataList
                    }, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            _logger.LogInformation("ReadMailFileHandler handled");
        }
    }
}