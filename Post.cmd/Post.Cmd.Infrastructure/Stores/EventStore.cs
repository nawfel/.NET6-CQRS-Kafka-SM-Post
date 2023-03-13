using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepo;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepo,IEventProducer eventProducer)
        {
            _eventStoreRepo = eventStoreRepo;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepo.FindByAggregateId(aggregateId);
            if(eventStream == null || !eventStream.Any()) {
                throw new AggregateNotFoundException("Incorrect post ID provided");
            }
            return eventStream.OrderBy(x => x.Version)
                .Select(x => x.EventData)
                .ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepo.FindByAggregateId(aggregateId);
            if(expectedVersion !=-1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrecnyException();
            var version = expectedVersion;
            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,                    
                    EventData = @event
                };
                await _eventStoreRepo.SaveAsync(eventModel);

                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
