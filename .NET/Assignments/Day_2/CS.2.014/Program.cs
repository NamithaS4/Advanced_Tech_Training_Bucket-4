namespace CS._2._014
{
    public interface IReadable
    {
        int ReadKwh();             
        string SourceId { get; }
    }

    public class DlmsMeter : IReadable
    {
        private readonly Random _random;
        public string SourceId { get; }

        public DlmsMeter(string sourceId, Random random)
        {
            SourceId = sourceId;
            _random = random;
        }

        public int ReadKwh()
        {
            return _random.Next(1, 11);
        }
    }

    public class ModemGateway : IReadable
    {
        private readonly Random _random;
        public string SourceId { get; }

        public ModemGateway(string sourceId, Random random)
        {
            SourceId = sourceId;
            _random = random;
        }

        public int ReadKwh()
        {
            return _random.Next(0, 3);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var seededRandom = new Random(12345);

            var meter = new DlmsMeter("AP-0001", seededRandom);
            var gateway = new ModemGateway("GW-21", seededRandom);

            var readers = new List<IReadable> { meter, gateway };

            for (int i = 0; i < 5; i++)
            {
                foreach (var reader in readers)
                {
                    int deltaKwh = reader.ReadKwh();
                    Console.WriteLine($"{reader.SourceId,-10} -> {deltaKwh}");
                }
            }
        }
    }
}