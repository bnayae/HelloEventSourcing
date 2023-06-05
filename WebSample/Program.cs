using Amazon.S3;

using StackExchange.Redis;

using WebSample;
using WebSample.Extensions;
using WebSample.Extensions.ShipmentTracking;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddEventSourceRedisConnection();

// Add services to the container.

IWebHostEnvironment environment = builder.Environment;
string env = environment.EnvironmentName;
string appName = environment.ApplicationName;

builder.Services.AddOpenTelemetry(environment, appName);

string URI = "shipment-tracking";
// make sure to create the bucket on AWS S3 with both prefix 'dev.' and 'prod.' and any other environment you're using (like staging,etc.) 
string s3Bucket = "shipment-tracking-sample";

builder.Services.AddShipmentTrackingProducer(URI, s3Bucket, env);
builder.Services.AddShipmentTrackingConsumer(URI, s3Bucket, env);

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
