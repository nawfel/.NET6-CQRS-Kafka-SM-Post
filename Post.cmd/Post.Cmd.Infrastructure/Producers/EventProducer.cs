using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;

        public EventProducer(IOptions<ProducerConfig> config)
        {
            _config = config.Value;
            var testConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",

            };
        }
        public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config).SetKeySerializer(Serializers.Utf8).SetValueSerializer(Serializers.Utf8).Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType()),

            };
            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if(deliveryResult.Status==PersistenceStatus.NotPersisted)
            {
                throw new Exception($"could not produce{@event.GetType()} message topic -{topic} due to the following reason {deliveryResult.Message}");
            }
        }
    }
}
