using MassTransit;
using MassTransit.Util;

namespace QuestionService.Tests.FunctionalTests.Configurations.TestServices;

internal class TestTopicProducer<T> : ITopicProducer<T> where T : class
{
    public ConnectHandle ConnectSendObserver(ISendObserver observer) => new EmptyConnectHandle();

    public Task Produce(T message, CancellationToken cancellationToken = new()) => Task.CompletedTask;

    public Task Produce(T message, IPipe<KafkaSendContext<T>> pipe,
        CancellationToken cancellationToken = new()) => Task.CompletedTask;

    public Task Produce(object values, CancellationToken cancellationToken = new()) => Task.CompletedTask;

    public Task Produce(object values, IPipe<KafkaSendContext<T>> pipe,
        CancellationToken cancellationToken = new()) => Task.CompletedTask;
}