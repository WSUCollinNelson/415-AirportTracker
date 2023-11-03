namespace AirportTracker
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string IATA { get; set; } = "";
        public string IACO { get; set; } = "";
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public string Timezone { get; set; } = "";
        public string DST { get; set; } = "";
        public string TzTime { get; set; } = "";
        public string Type { get; set; } = "";
        public string Source { get; set; } = "";
    }
}
