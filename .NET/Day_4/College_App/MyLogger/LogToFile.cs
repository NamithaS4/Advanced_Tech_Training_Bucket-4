namespace College_App.MyLogger
{
    public class LogToFile: IMyLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("LogtoFile");
        }
    }
}
