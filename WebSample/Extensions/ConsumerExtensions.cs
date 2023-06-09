﻿using EventSourcing.Backbone;

// Configuration: https://medium.com/@gparlakov/the-confusion-of-asp-net-configuration-with-environment-variables-c06c545ef732


namespace WebSample.Extensions
{
    /// <summary>
    ///  DI Extensions for ASP.NET Core
    /// </summary>
    public static class ConsumerExtensions
    {
        /// <summary>
        /// Adds the shipment tracking producer.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="env">The environment.</param>
        /// <returns></returns>
        public static IServiceCollection AddConsumer
            (
            this IServiceCollection services,
            string uri,
            string s3Bucket,
            Env env)
        {
            var s3Options = new S3Options { Bucket = s3Bucket };
            services.AddSingleton(ioc =>
            {
                IConsumerReadyBuilder consumer =
                            ioc.ResolveRedisConsumerChannel()
                                .ResolveS3Storage(s3Options)
                                .WithOptions(o => o with
                                {
                                    TraceAsParent = TimeSpan.FromMinutes(10),
                                    OriginFilter = MessageOrigin.Original,
                                    AckBehavior = AckBehavior.OnSucceed
                                })
                                .Environment(env)
                                .Uri(uri);
                return consumer;
            });

            return services;
        }
    }
}
