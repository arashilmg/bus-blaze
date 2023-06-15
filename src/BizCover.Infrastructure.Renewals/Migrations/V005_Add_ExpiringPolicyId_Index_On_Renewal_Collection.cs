using BizCover.Entity.Renewals;
using MongoDB.Driver;

namespace BizCover.Infrastructure.Renewals.Migrations;

internal class V005_Add_ExpiringPolicyId_Index_On_Renewal_Collection
{
    public int From => 4;
    public int To => 5;

    public void Execute(IMongoDatabase database)
    {
        const string indexName = "ExpiringPolicyId_Index_On_Renewal";

        var collection = database.GetCollection<Renewal>(nameof(Renewal));

        collection
            .Indexes
            .CreateOne(new CreateIndexModel<Renewal>(
                new IndexKeysDefinitionBuilder<Renewal>().Ascending(r => r.ExpiringPolicyId),
                new CreateIndexOptions { Name = indexName, Unique = true }));
    }
}
