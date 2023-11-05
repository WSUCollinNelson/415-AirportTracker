namespace AirportTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseManager dbManager = new DatabaseManager();

            List<Airport> seattleAirports = dbManager.GetAirports("Seattle");
            foreach (Airport airport in seattleAirports)
            { 
                Console.WriteLine(airport.Name);
            }

            List<Airport> nonexistentAirports = dbManager.GetAirports("not a real city name");
            Console.WriteLine(nonexistentAirports.Count);

            Console.WriteLine("\n\tPath Test\n");

            Airport pullmanAirport = dbManager.GetAirports("Pullman")[0];
            Airport dubaiAirport = dbManager.GetAirports("Dubai")[0];

            Console.WriteLine(dbManager.GetShortestPathBetweenAirports(pullmanAirport, dubaiAirport));
        }
    }
}