using BizCover.Entity.Renewals;
using BizCover.Framework.Persistence.Mongo.Migrations;
using MongoDB.Driver;

namespace BizCover.Infrastructure.Renewals.Migrations
{
    internal class V002_Add_OrderGeneration_Date_Index_On_Renewal_Collection : IMigration
    {
        public int From => 1;
        public int To => 2;

        public void Execute(IMongoDatabase database)
        {
            const string indexName = "OrderGeneration_Date_Index_On_Renewal";

            var collection = database.GetCollection<Renewal>(nameof(Renewal));

            collection
                .Indexes
                .CreateOne(new CreateIndexModel<Renewal>(
                    new IndexKeysDefinitionBuilder<Renewal>().Ascending(r => r.RenewalDates!.OrderGeneration),
                    new CreateIndexOptions {Name = indexName}));
        }
    }
}
