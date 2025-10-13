namespace CS._3._018
{
    public interface IDataIngestor
    {
        string Name { get; }
        IEnumerable<(DateTime ts, int kwh)> ReadBatch(int count);
    }

    public class DlmsIngestor : IDataIngestor
    {
        private readonly Random _random = new Random();
        private DateTime _currentTime = DateTime.Now.Date.AddHours(10);

        public string Name => "Dlms";

        public IEnumerable<(DateTime ts, int kwh)> ReadBatch(int count)
        {
            var results = new List<(DateTime, int)>();

            for (int i = 0; i < count; i++)
            {
                var kwh = _random.Next(1, 6); 
                results.Add((_currentTime, kwh));
                _currentTime = _currentTime.AddHours(1);
            }

            return results;
        }
    }

    public class RandomOutageDecorator : IDataIngestor
    {
        private readonly IDataIngestor _wrappedIngestor;
        private readonly Random _random;
        private readonly double _outageProbability;

        public string Name => $"{_wrappedIngestor.Name}+Outage";

        public RandomOutageDecorator(IDataIngestor ingestor, double outageProbability = 0.3)
        {
            _wrappedIngestor = ingestor;
            _random = new Random();
            _outageProbability = outageProbability;
        }

        public IEnumerable<(DateTime ts, int kwh)> ReadBatch(int count)
        {
            var results = _wrappedIngestor.ReadBatch(count);

            return results.Select(r =>
            {
                if (_random.NextDouble() < _outageProbability)
                    return (r.ts, 0); 
                return r;
            });
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var dlmsIngestor = new DlmsIngestor();
            var outageIngestor = new RandomOutageDecorator(dlmsIngestor, 0.3);

            Console.WriteLine($" [{outageIngestor.Name}] ReadBatch(10) ");

            var batch = outageIngestor.ReadBatch(10);

            foreach (var (ts, kwh) in batch)
            {
                Console.WriteLine($"{ts:yyyy-MM-dd HH:mm} -> {kwh}");
            }
        }
    }
}