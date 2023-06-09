using System.Collections.Immutable;
using System.Diagnostics;

using Amazon.S3;

using OpenTelemetry.Trace;

using WebSample;
using WebSample.Extensions;
using WebSample.Extensions.ShipmentTracking;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddEventSourceRedisConnection();

IWebHostEnvironment environment = builder.Environment;
string env = environment.EnvironmentName;
string appName = environment.ApplicationName;

builder.Services.AddOpenTelemetry(environment, appName, TestSampler.Create());

string URI = "shipment-tracking";
// make sure to create the bucket on AWS S3 with both prefix 'dev.' and 'prod.' and any other environment you're using (like staging,etc.) 
string s3Bucket = "shipment-tracking-sample";

builder.Services.AddShipmentTrackingProducer(URI, s3Bucket, env);
builder.Services.AddConsumer(URI, s3Bucket, env);

builder.Services.AddHostedService<ConsumerJob>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();


internal class TestSampler : Sampler
{
    private readonly Sampler? _sampler;
    private readonly IImmutableSet<string> _ignore = ImmutableHashSet.Create("XPENDING", "XREADGROUP", "XADD", "XACK");

    public static Sampler Create(LogLevel logLevel = LogLevel.Information, Sampler? chainedSampler = null)
        => new TestSampler(logLevel, chainedSampler);
    private readonly LogLevel _logLevel;

    #region Ctor

    private TestSampler(LogLevel logLevel = LogLevel.Information, Sampler? chainedSampler = null)
    {
        _logLevel = logLevel;
        _sampler = chainedSampler;
    }

    #endregion Ctor

    #region ShouldSample

    /// <summary>
    /// Checks whether span needs to be created and tracked.
    /// </summary>
    /// <param name="samplingParameters">The <see cref="T:OpenTelemetry.Trace.SamplingParameters" /> used by the <see cref="T:OpenTelemetry.Trace.Sampler" />
    /// to decide if the <see cref="T:System.Diagnostics.Activity" /> to be created is going to be sampled or not.</param>
    /// <returns>
    /// Sampling decision on whether Span needs to be sampled or not.
    /// </returns>
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        // Parent span context. Typically taken from the wire.
        ActivityContext parentContext = samplingParameters.ParentContext;
        // Trace ID of a span to be created.
        ActivityTraceId traceId = samplingParameters.TraceId;
        string name = samplingParameters.Name;

        if (_logLevel >= LogLevel.Information && _ignore.Contains(name))
            return new SamplingResult(SamplingDecision.Drop);
        //ActivityKind spanKind = samplingParameters.Kind;
        // Initial set of Attributes for the Span being constructed.
        //var attributes = samplingParameters.Tags;
        // Links associated with the span.
        //IEnumerable<ActivityLink> links = samplingParameters.Links;

        //var path = HttpRequestContext.Value?.Path.Value ?? "";
        //if (path == "/health" ||
        //    path == "/readiness" ||
        //    path == "/version" ||
        //    path == "/settings" ||
        //    path.StartsWith("/v1/kv/") || // configuration 
        //    path == "/api/v2/write" || // influx metrics
        //    path == "/_bulk" ||
        //    path.StartsWith("/swagger") ||
        //    path.IndexOf("health-check") != -1)
        //{
        //    return new SamplingResult(false);
        //}
        return _sampler?.ShouldSample(samplingParameters) ?? new SamplingResult(SamplingDecision.RecordAndSample);
    }

    #endregion ShouldSample
}
