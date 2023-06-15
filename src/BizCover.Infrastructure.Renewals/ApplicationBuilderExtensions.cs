using BizCover.Framework.Persistence.Mongo.Extensions;
using BizCover.Infrastructure.Renewals.Migrations;
using Microsoft.AspNetCore.Builder;

namespace BizCover.Infrastructure.Renewals
{
    public static class ApplicationBuilderExtensions
    {
        public static void AddMongoDbIndexes(this IApplicationBuilder app)
        {
            app.UpgradeMongoSchema(typeof(V001_Add_Initiation_Date_Index_On_Renewal_Collection));
        }
    }
}
