namespace AMIProjectView.Models
{
    // This mirrors the API "Meter" payload so Index, Edit, Create all have the same fields.
    public class MeterViewModel
    {
        public string MeterSerialNo { get; set; } = string.Empty;

        public int ConsumerId { get; set; }
        public int OrgUnitId { get; set; }
        public string Ipaddress { get; set; } = string.Empty;
        public string Iccid { get; set; } = string.Empty;
        public string Imsi { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;
        public string? Firmware { get; set; }

        public string Category { get; set; } = "Residential Tariff";

        public DateTime InstallDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
