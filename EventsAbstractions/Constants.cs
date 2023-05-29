using EventSourcing.Backbone;

namespace EventSourcing.Demo;

public class Constants
{
    public const string URI = "hello-event-sourcing";
    public const string S3_BUCKET = "event-sourcing-demo";
    public const string S3_ACCESS_KEY_ENV = "S3_EVENT_DEMO_ACCESS_KEY";
    public const string S3_SECRET_ENV = "S3_EVENT_DEMO_SECRET";
    public const string S3_REGION_ENV = "S3_EVENT_DEMO_REGION";

    public static readonly S3Options S3Options = new S3Options
    {
        Bucket = Constants.S3_BUCKET
    };

}
