using System.Text;

namespace CS._3._019
{
    public interface IBillingRule
    {
        string Name { get; }
        double Compute(int units);
    }


    public class CommercialRule : IBillingRule
    {
        public string Name => "Commercial";

        public double Compute(int units)
        {
            double rate = 8.5;
            double fixedCharge = 150;
            return (units * rate) + fixedCharge;
        }

        public double Rate => 8.5;
        public double FixedCharge => 150;
    }

    public interface IRebate
    {
        string Code { get; }
        double Apply(double currentTotal, int outageDays, int units);
    }

    public class NoOutageRebate : IRebate
    {
        public string Code => "NO_OUTAGE";

        public double Apply(double currentTotal, int outageDays, int units)
        {
            if (outageDays == 0)
                return -0.02 * currentTotal; 
            return 0;
        }
    }

    public class HighUsageRebate : IRebate
    {
        public string Code => "HIGH_USAGE";

        public double Apply(double currentTotal, int outageDays, int units)
        {
            if (units > 500)
                return -0.03 * currentTotal; 
            return 0;
        }
    }

    public class BillingContext
    {
        public IBillingRule Rule { get; }
        public List<IRebate> Rebates { get; } = new();

        public BillingContext(IBillingRule rule) => Rule = rule;

        public double Finalize(int units, int outageDays)
        {
            double total = Rule.Compute(units);
            double rebateSum = 0;

            foreach (var rebate in Rebates)
            {
                rebateSum += rebate.Apply(total, outageDays, units);
            }

            return total + rebateSum;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            int units = 620;
            int outageDays = 0;

            var rule = new CommercialRule();
            var context = new BillingContext(rule);

            context.Rebates.Add(new NoOutageRebate());
            context.Rebates.Add(new HighUsageRebate());

            double energyPart = rule.Rate * units;
            double subtotal = rule.Compute(units);

            Console.WriteLine($"Subtotal: ₹  {rule.Rate}*{units} + {rule.FixedCharge} = ₹  {energyPart,6:0,0} + {rule.FixedCharge} = ₹  {subtotal,6:0,0.00}");
            Console.WriteLine("Rebates: NO_OUTAGE -2% | HIGH_USAGE -3%");

            double finalTotal = context.Finalize(units, outageDays);
            Console.WriteLine($"Final: ₹ {finalTotal:0,0.00}");
        }
    }
}
