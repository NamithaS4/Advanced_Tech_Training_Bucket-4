namespace AMIProjectView.Models
{
    public class TariffSlabVm
    {
        public int SlabId { get; set; }
        public int TariffId { get; set; }
        public decimal FromKwh { get; set; }
        public decimal ToKwh { get; set; }
        public decimal RatePerKwh { get; set; }
    }
}
