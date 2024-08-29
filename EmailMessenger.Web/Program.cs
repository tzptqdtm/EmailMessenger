using Microsoft.OpenApi.Models;
using System.Reflection;
using EmailMessenger.Core.Services.Email;
using EmailMessenger.Core.Services.Job;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using EmailMessenger.Models.Data.Settings;
using EmailMessenger.Data.DataContexts;
using EmailMessenger.Data.DataServices;
using EmailMessenger.Models.Core;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;

namespace EmailMessenger.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("init main");

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mailing API", Version = "v1" });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.Configure<DatabaseSettings>(
                builder.Configuration.GetSection("DatabaseSettings"));

            builder.Services.Configure<MailSettings>(
                builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetSection("DatabaseSettings:ConnectionString").Value,
                    x => x.MigrationsHistoryTable("__EFMigrationHistory",
                        builder.Configuration.GetSection("DatabaseSettings:DatabaseSchema").Value));
            });

            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => 
                    options.UseNpgsqlConnection(builder.Configuration.GetSection("DatabaseSettings:ConnectionString").Value)));

            builder.Services.AddHangfireServer();

            builder.Services.AddScoped<IDataSource, AppDbContext>();

            builder.Services.AddScoped<IDataService, DataService>();

            builder.Services.AddTransient<IMailingService, MailingService>();

            builder.Services.AddTransient<IJobRunnerService, JobRunnerService>();

            builder.Services.AddTransient<IMessageEventHandler, MessageEventHandler>();

            builder.Services.AddTransient<IJobScheduler, JobScheduler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard("/dashboard");
            
            app.Run();
        }
        catch (Exception exception)
        {
            // NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}
