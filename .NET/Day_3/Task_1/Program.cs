namespace Task_1
{
    // Smart Home Device System

    
    public interface IDevice
    {
        string DeviceName { get; }
        bool IsOn { get; }
        void TurnOn();
        void TurnOff();
    }

    
    public interface ISmartDevice : IDevice
    {
        void ConnectToWiFi(string networkName);
        void ShowStatus();
    }

    
    public class Light : ISmartDevice
    {
        public string DeviceName { get; }
        public bool IsOn { get; private set; }
        public int Brightness { get; private set; }
        private string _connectedNetwork;

        public Light(string name)
        {
            DeviceName = name;
            IsOn = false;
            Brightness = 0;
        }

        public void TurnOn()
        {
            IsOn = true;
            Brightness = 100;
            Console.WriteLine($"{DeviceName} is now ON with brightness at 100%.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Brightness = 0;
            Console.WriteLine($"{DeviceName} is now OFF.");
        }

        public void ConnectToWiFi(string networkName)
        {
            _connectedNetwork = networkName;
            Console.WriteLine($"{DeviceName} connected to WiFi network '{networkName}'.");
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Status for {DeviceName}");
            Console.WriteLine($"Is On: {IsOn}");
            Console.WriteLine($"Brightness: {Brightness}%");
            Console.WriteLine($"Connected to WiFi: {(_connectedNetwork ?? "Not connected")}");
            Console.WriteLine();
        }
    }

    
    public class Fan : ISmartDevice
    {
        public string DeviceName { get; }
        public bool IsOn { get; private set; }
        public int Speed { get; private set; }
        private string _connectedNetwork;

        public Fan(string name)
        {
            DeviceName = name;
            IsOn = false;
            Speed = 0;
        }

        public void TurnOn()
        {
            IsOn = true;
            Speed = 3;
            Console.WriteLine($"{DeviceName} is now ON at speed level 3.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Speed = 0;
            Console.WriteLine($"{DeviceName} is now OFF.");
        }

        public void ConnectToWiFi(string networkName)
        {
            _connectedNetwork = networkName;
            Console.WriteLine($"{DeviceName} connected to WiFi network '{networkName}'.");
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Status for {DeviceName}");
            Console.WriteLine($"Is On: {IsOn}");
            Console.WriteLine($"Speed: {Speed}");
            Console.WriteLine($"Connected to WiFi: {(_connectedNetwork ?? "Not connected")}");
            Console.WriteLine();
        }
    }

   
    public class Thermostat : ISmartDevice
    {
        public string DeviceName { get; }
        public bool IsOn { get; private set; }
        public double Temperature { get; private set; }
        private string _connectedNetwork;

        public Thermostat(string name)
        {
            DeviceName = name;
            IsOn = false;
            Temperature = 20.0;
        }

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"{DeviceName} is now ON.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{DeviceName} is now OFF.");
        }

        public void ConnectToWiFi(string networkName)
        {
            _connectedNetwork = networkName;
            Console.WriteLine($"{DeviceName} connected to WiFi network '{networkName}'.");
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Status for {DeviceName}");
            Console.WriteLine($"Is On: {IsOn}");
            Console.WriteLine($"Temperature: {Temperature}°C");
            Console.WriteLine($"Connected to WiFi: {(_connectedNetwork ?? "Not connected")}");
            Console.WriteLine();
        }
    }

    
    internal class Program
    {
        static void Main(string[] args)
        {
            List<ISmartDevice> smartDevices = new List<ISmartDevice>
            {
                new Light("Living Room Light"),
                new Fan("Bedroom Fan"),
                new Thermostat("Main Thermostat")
            };

            Console.WriteLine("Initial Status");
            foreach (var device in smartDevices)
            {
                device.ShowStatus();
            }

            Console.WriteLine("Turning on all devices");
            foreach (var device in smartDevices)
            {
                device.TurnOn();
            }
            Console.WriteLine();

            Console.WriteLine("Connecting all devices to WiFi");
            foreach (var device in smartDevices)
            {
                device.ConnectToWiFi("MyHomeNetwork");
            }
            Console.WriteLine();

            Console.WriteLine("Final Status");
            foreach (var device in smartDevices)
            {
                device.ShowStatus();
            }
        }
    }
}
