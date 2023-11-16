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
    }
}
