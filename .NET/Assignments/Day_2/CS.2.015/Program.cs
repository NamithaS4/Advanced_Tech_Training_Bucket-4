using System.Globalization;
using System.Text;

namespace CS._2._015
{
    public interface IBillingRule
    {
        double Compute(int units);
    }
    public class DomesticRule : IBillingRule
    {
        public double Compute(int units)
        {
            return 6.0 * units + 50.0;
        }
    }
    public class CommercialRule : IBillingRule
    {
        public double Compute(int units)
        {
            return 8.5 * units + 150.0;
        }
    }
    public class AgricultureRule : IBillingRule
    {
        public double Compute(int units)
        {
            return 3.0 * units;
        }
    }
    public class BillingEngine
    {
        public IBillingRule Rule { get; set; }

        public BillingEngine(IBillingRule rule)
        {
            Rule = rule;
        }

        public double GenerateBill(int units)
        {
            return Rule.Compute(units);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            int unitsConsumed = 120;

            var domesticRule = new DomesticRule();
            var commercialRule = new CommercialRule();
            var agricultureRule = new AgricultureRule();

            var domesticEngine = new BillingEngine(domesticRule);
            var commercialEngine = new BillingEngine(commercialRule);
            var agricultureEngine = new BillingEngine(agricultureRule);

            Console.WriteLine($"DOMESTIC -> {domesticEngine.GenerateBill(unitsConsumed).ToString("C", new CultureInfo("en-IN"))}");
            Console.WriteLine($"COMMERCIAL -> {commercialEngine.GenerateBill(unitsConsumed).ToString("C", new CultureInfo("en-IN"))}");
            Console.WriteLine($"AGRICULTURE -> {agricultureEngine.GenerateBill(unitsConsumed).ToString("C", new CultureInfo("en-IN"))}");
        }
    }
}