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
        public string Timezone { get; set; } = "";
        public string TzTime { get; set; } = "";
    }
}
