namespace AirportTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseManager dbManager = new DatabaseManager();
            Airport? airport1 = dbManager.GetAirport("Seattle");
            Airport? airport2 = dbManager.GetAirport("Spokane");
            if (airport1 != null && airport2 != null)
            {
                Console.WriteLine(airport1.Name);
                Console.WriteLine(airport2.Name);

                dbManager.GetPathBetweenAirports(airport1, airport2);
            }

        }
    }
}