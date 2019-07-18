using System.ComponentModel.DataAnnotations.Schema;

namespace API.Trade.Common
{
    public class Security
    {
        [Column("INSTRUMENT_ID")]
        public long InstrumentId { get; set; }
        [Column("SECURITY_IDENTIFIER")]
        public string SecurityIdentifier { get; set; }
        [Column("ASSET_TYPE")]
        public string AssetType { get; set; }
    }
}
