using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Repository
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private IMongoCollection<EventModel> _eventsStoreCollection;

        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDb = mongoClient.GetDatabase(config.Value.Database);

            _eventsStoreCollection = mongoDb.GetCollection<EventModel>(config.Value.Collection);


        }
        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventsStoreCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel @event)
        {
           await _eventsStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }
    }
}
