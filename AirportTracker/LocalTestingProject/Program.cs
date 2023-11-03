namespace AirportTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseManager dbManager = new DatabaseManager();
            Airport? airport = dbManager.GetAirport("Seattle");
            if (airport != null)
                Console.WriteLine(airport.Name);
        }
    }
}