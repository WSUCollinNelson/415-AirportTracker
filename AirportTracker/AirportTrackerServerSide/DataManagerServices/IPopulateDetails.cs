namespace AirportTracker.DataManagerServices
{
    public interface IPopulateDetails
    {
        public event Action<Airport> NewAirportSelected;

        void SelectAirport(Airport airport);
    }
}
