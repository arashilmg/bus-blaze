using BizCover.Entity.Renewals;
using MongoDB.Driver;

namespace BizCover.Infrastructure.Renewals.Migrations
{
    internal class V004_Add_OrderId_Index_On_Renewal_Collection
    {
        public int From => 3;
        public int To => 4;

        public void Execute(IMongoDatabase database)
        {
            const string indexName = "OrderId_Index_On_Renewal";

            var collection = database.GetCollection<Renewal>(nameof(Renewal));

            collection
                .Indexes
                .CreateOne(new CreateIndexModel<Renewal>(
                    new IndexKeysDefinitionBuilder<Renewal>().Ascending(r => r.OrderId),
                    new CreateIndexOptions { Name = indexName, Unique = true }));
        }
    }
}