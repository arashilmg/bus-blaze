using BizCover.Entity.Renewals;
using BizCover.Framework.Persistence.Mongo.Migrations;
using MongoDB.Driver;

namespace BizCover.Infrastructure.Renewals.Migrations
{
    internal class V001_Add_Initiation_Date_Index_On_Renewal_Collection : IMigration
    {
        public int From => 0;
        public int To => 1;

        public void Execute(IMongoDatabase database)
        {
            const string indexName = "Initiation_Date_Index_On_Renewal";

            var collection = database.GetCollection<Renewal>(nameof(Renewal));

            collection
                .Indexes
                .CreateOne(new CreateIndexModel<Renewal>(
                    new IndexKeysDefinitionBuilder<Renewal>().Ascending(r => r.RenewalDates!.Initiation),
                    new CreateIndexOptions {Name = indexName}));
        }
    }
}
