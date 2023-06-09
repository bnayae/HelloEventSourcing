﻿using EventSourcing.Backbone;
using EventSourcing.Backbone.Building;
using EventSourcing.Demo;

namespace WebSample;

/// <summary>
/// Consumer job
/// </summary>
/// <seealso cref="Microsoft.Extensions.Hosting.HostedServiceBase" />
/// <seealso cref="System.IDisposable" />
public sealed class ConsumerJob : IHostedService
{
    private readonly IConsumerSubscribeBuilder _builder;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IShipmentTrackingConsumer _subscriber;
    private IConsumerLifetime? _subscription;
    //private const string CONSUMER_GROUP = "CONSUMER";

    #region Ctor

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="consumerBuilder">The builder.</param>
    /// <param name="producer">The producer.</param>
    public ConsumerJob(
        ILogger<ConsumerJob> logger,
        IConsumerReadyBuilder consumerBuilder)
    {
        _builder = consumerBuilder.WithLogger(logger);
        _subscriber = new Subscriber(logger);
    }

    #endregion Ctor

    #region OnStartAsync

    /// <summary>
    /// Start Consumer Job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var canellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
        _subscription = _builder
                                // .Group(CONSUMER_GROUP)
                                .WithCancellation(canellation.Token)
                                // this extension is generate (if you change the interface use the correlated new generated extension method)
                                .SubscribeShipmentTrackingConsumer(_subscriber);

        return Task.CompletedTask;
    }

    #endregion // OnStartAsync

    #region StopAsync

    /// <summary>
    /// Stops the Consumer Job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.CancelSafe();
        await (_subscription?.Completion ?? Task.CompletedTask);
    }

    #endregion // StopAsync

    #region class Subscriber : IShipmentTrackingConsumer

    /// <summary>
    /// The subscriber implementation
    /// </summary>
    /// <seealso cref="EventSourcing.Demo.IShipmentTrackingConsumer" />
    private sealed class Subscriber : IShipmentTrackingConsumer
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriber"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Subscriber(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handle [OrderPlaced] event.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="product">The product.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        ValueTask IShipmentTrackingConsumer.OrderPlacedAsync(
            ConsumerMetadata consumerMetadata,
            User user,
            Product product,
            DateTimeOffset time)
        {
            Metadata meta = consumerMetadata;
            _logger.LogInformation("handling OrderPlaced [{message-id}]: email: {email}, product: {productId}, which produce at {time}", meta.MessageId, user.email, product.id, time);
            return ValueTask.CompletedTask;
        }


        /// <summary>
        /// Handle [Packings] event.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        ValueTask IShipmentTrackingConsumer.PackingAsync(
            ConsumerMetadata consumerMetadata,
            string email,
            int productId,
            DateTimeOffset time)
        {
            Metadata meta = consumerMetadata;
            _logger.LogInformation("handling Packing [{message-id}]: email: {email}, product: {productId}, which produce at {time}", meta.MessageId, email, productId, time);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Handle [on-delivery] event.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        ValueTask IShipmentTrackingConsumer.OnDeliveryAsync(
            ConsumerMetadata consumerMetadata,
            string email,
            int productId,
            DateTimeOffset time)
        {
            Metadata meta = consumerMetadata;
            _logger.LogInformation("handling OnDelivery [{message-id}]: email: {email}, product: {productId}, which produce at {time}", meta.MessageId, email, productId, time);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Handle [on-received] event.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        ValueTask IShipmentTrackingConsumer.OnReceivedAsync(
            ConsumerMetadata consumerMetadata,
            string email,
            int productId,
            DateTimeOffset time)
        {
            Metadata meta = consumerMetadata;
            _logger.LogInformation("handling OnReceived [{message-id}]: email: {email}, product: {productId}, which produce at {time}", meta.MessageId, email, productId, time);
            return ValueTask.CompletedTask;
        }
    }

    #endregion // class Subscriber : IShipmentTrackingConsumer
}
