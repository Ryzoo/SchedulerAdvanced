using System;
using System.Reflection;
using Application.FileRepository;
using Application.Handlers;
using Application.Requests;
using Application.Services;
using Application.Settings;
using Application.Tasks;
using Core.Interfaces.Mails;
using Core.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SchedulerAdvanced
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/scheduleAppLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    var emailSettings = new EmailSettings()
                    {
                        From = hostContext.Configuration["EmailSettings:From"],
                        Host = hostContext.Configuration["EmailSettings:Host"],
                        Port = int.Parse(hostContext.Configuration["EmailSettings:Port"]),
                        Username = hostContext.Configuration["EmailSettings:Username"],
                        Password = hostContext.Configuration["EmailSettings:Password"]
                    };
                    
                    services
                        .Configure<CsvFilePathSettings>(props => hostContext.Configuration.GetSection("CsvFilePathSettings").Bind(props))
                        .Configure<EmailSettings>(props => hostContext.Configuration.GetSection("DbSettings").Bind(props))
                        .AddMemoryCache()
                        .AddTransient<ICsvParserService, CsvParserService>()
                        .AddTransient<IMailBuilderService, MailBuilderService>()
                        .AddTransient<IFileRepository, FileRepository>()
                        .AddMediatR(typeof(ReadMailFileHandler).GetTypeInfo().Assembly)
                        .AddHostedService<TaskIntervalRunner<ReadMailFileRequest>>()
                        .AddFluentEmail(emailSettings.From)
                        .AddSmtpSender(emailSettings.Host, emailSettings.Port, emailSettings.Username, emailSettings.Password)
                        .AddRazorRenderer();
                });
    }
}