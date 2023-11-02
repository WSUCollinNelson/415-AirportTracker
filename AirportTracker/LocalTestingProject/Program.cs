namespace AirportTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseManager dbManager = new DatabaseManager();
            Console.WriteLine(dbManager.GetAirport("test"));
        }
    }
}