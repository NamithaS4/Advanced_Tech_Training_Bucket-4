namespace AMIProjectAPI.Dtos
{
    public class TariffSlabDto
    {
        public int SlabId { get; set; }            // optional for create, used in update
        public int TariffId { get; set; }
        public decimal FromKwh { get; set; }
        public decimal ToKwh { get; set; }
        public decimal RatePerKwh { get; set; }
    }
}
