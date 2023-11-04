namespace AirportTracker.DataManagerServices
{
    public class DetailsPopulator : IPopulateDetails
    {
        public event Action<Airport> NewAirportSelected;
        public void SelectAirport(Airport airport)
        {
            NewAirportSelected?.Invoke(airport);
        }
    }
}
