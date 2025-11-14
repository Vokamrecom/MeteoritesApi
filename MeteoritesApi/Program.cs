using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using MeteoritesApi.Clients;
using MeteoritesApi.Data;
using MeteoritesApi.Jobs;
using MeteoritesApi.Options;
using MeteoritesApi.Services;
using MeteoritesApi.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicy = "MeteoritesUI";

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.Configure<NasaDataOptions>(builder.Configuration.GetSection(NasaDataOptions.SectionName));
builder.Services.Configure<SyncJobOptions>(builder.Configuration.GetSection(SyncJobOptions.SectionName));
builder.Services.Configure<CachingOptions>(builder.Configuration.GetSection(CachingOptions.SectionName));
builder.Services.Configure<HangfireOptions>(builder.Configuration.GetSection(HangfireOptions.SectionName));

builder.Services.AddDbContext<MeteoritesDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddHangfire((serviceProvider, configuration) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
                           ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(connectionString);
});

builder.Services.AddHangfireServer((serviceProvider, options) =>
{
    var hangfireOptions = serviceProvider.GetRequiredService<IOptions<HangfireOptions>>().Value;
    options.Queues = new[] { hangfireOptions.Queue };
    options.WorkerCount = Math.Max(1, hangfireOptions.WorkerCount);
});

builder.Services.AddHttpClient<INasaDatasetClient, NasaDatasetClient>();
builder.Services.AddScoped<IMeteoriteIngestionService, MeteoriteIngestionService>();
builder.Services.AddScoped<IMeteoriteSummaryService, MeteoriteSummaryService>();
builder.Services.AddScoped<IMeteoriteFilterService, MeteoriteFilterService>();
builder.Services.AddScoped<MeteoriteSyncJob>();
builder.Services.AddMemoryCache();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<MeteoriteSummaryQueryValidator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MeteoritesDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors(CorsPolicy);

app.UseAuthorization();

app.MapControllers();

var syncOptions = app.Services.GetRequiredService<IOptions<SyncJobOptions>>().Value;
var hangfireOptions = app.Services.GetRequiredService<IOptions<HangfireOptions>>().Value;

RecurringJob.AddOrUpdate<MeteoriteSyncJob>(
    "meteorite-sync-job",
    job => job.Execute(CancellationToken.None),
    syncOptions.CronExpression,
    new RecurringJobOptions
    {
        QueueName = hangfireOptions.Queue
    });

app.Run();
