namespace AMIProjectAPI.Models.Dtos
{
    public class MeterCreateDto
    {
        public string MeterSerialNo { get; set; } = string.Empty;
        public int ConsumerId { get; set; }
        public int OrgUnitId { get; set; }

        public string Ipaddress { get; set; } = string.Empty;
        public string Iccid { get; set; } = string.Empty;
        public string Imsi { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string? Firmware { get; set; }

        // Must match your CHECK constraint values
        public string Category { get; set; } = "Residential Tariff";

        public DateTime InstallDate { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class MeterUpdateDto
    {
        public int ConsumerId { get; set; }
        public int OrgUnitId { get; set; }

        public string? Ipaddress { get; set; }
        public string? Iccid { get; set; }
        public string? Imsi { get; set; }
        public string? Manufacturer { get; set; }
        public string? Firmware { get; set; }
        public string? Category { get; set; }
        public DateTime? InstallDate { get; set; }
        public string? Status { get; set; }
    }
}
