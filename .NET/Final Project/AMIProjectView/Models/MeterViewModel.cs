namespace AMIProjectView.Models
{
    // This mirrors the API "Meter" payload so Index, Edit, Create all have the same fields.
    public class MeterViewModel
    {
        public string MeterSerialNo { get; set; } = string.Empty;

        public int ConsumerId { get; set; }          // <- needed by controller
        public int OrgUnitId { get; set; }           // <- missing in your class

        // Keep the property names as your API returns (Db columns mapped by EF):
        public string Ipaddress { get; set; } = string.Empty;   // "IPAddress" in the UI, "Ipaddress" from API
        public string Iccid { get; set; } = string.Empty;
        public string Imsi { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;
        public string? Firmware { get; set; }

        // Must match DB CHECK constraint values
        public string Category { get; set; } = "Residential Tariff";

        public DateTime InstallDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
