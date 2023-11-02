namespace AirportTracker
{
    public class Route
    {
        //RAW DATA
        public string Airline { get; set; } = "";
        public int AirlineID { get; set; }
        public string Source { get; set; } = "";
        public int SourceID { get; set; }
        public string Dest { get; set; } = "";
        public int DestID { get; set; }
        public string Codeshare { get; set; } = "";
        public int Stops { get; set; }
        public string Equipment { get; set; } = "";

        //DERIVED LINKS
        public Airport? SourceAirport { get; private set; }
        public Airport? DestAirport { get; private set; }

    }
}
