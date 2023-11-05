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

            Airport pullmanAirport = dbManager.GetAirports("Pullman")[0];
            Airport dubaiAirport = dbManager.GetAirports("Dubai")[0];

            Console.WriteLine("Path between Pullman and Dubai:");
            Console.WriteLine($"Start: {pullmanAirport.Name}");
            foreach (Route route in dbManager.GetShortestPathBetweenAirports(pullmanAirport, dubaiAirport))
            {
                foreach(Airport airport in dbManager.GetAirportsWithFilter($"n.id = {route.DestID}").Result)
                {
                    Console.WriteLine($"To airport: {airport.Name}");
                }
            }
        }
    }
}