﻿using BizCover.Entity.Renewals;
using BizCover.Framework.Persistence.Mongo.Migrations;
using MongoDB.Driver;

namespace BizCover.Infrastructure.Renewals.Migrations
{
    internal class V003_Add_OrderSubmission_Date_Index_On_Renewal_Collection : IMigration
    {
        public int From => 2;
        public int To => 3;

        public void Execute(IMongoDatabase database)
        {
            const string indexName = "OrderSubmission_Date_Index_On_Renewal";

            var collection = database.GetCollection<Renewal>(nameof(Renewal));

            collection
                .Indexes
                .CreateOne(new CreateIndexModel<Renewal>(
                    new IndexKeysDefinitionBuilder<Renewal>().Ascending(r => r.RenewalDates!.OrderSubmission),
                    new CreateIndexOptions {Name = indexName}));
        }
    }
}
