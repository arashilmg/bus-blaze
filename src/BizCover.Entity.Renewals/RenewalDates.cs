using MongoDB.Bson.Serialization.Attributes;

namespace BizCover.Entity.Renewals
{
    [BsonIgnoreExtraElements]
    public class RenewalDates
    {
        public DateTime Initiation { get; set; }
        public DateTime? Initiated { get; set; }
        public DateTime OrderGeneration { get; set; }
        public DateTime? OrderGenerated { get; set; }
        public DateTime OrderSubmission { get; set; }
        public DateTime? OrderSubmitted { get; set; }
    }
}
