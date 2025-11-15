using System.Text.Json.Serialization;

namespace AMIProjectView.Models
{
    public class MeterEditViewModel
    {
        public string MeterSerialNo { get; set; } = "";
        public int ConsumerId { get; set; }
        public int OrgUnitId { get; set; }

        [JsonPropertyName("Ipaddress")]
        public string IpAddress { get; set; } = "";

        public string ICCID { get; set; } = "";
        public string IMSI { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Firmware { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime InstallDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
