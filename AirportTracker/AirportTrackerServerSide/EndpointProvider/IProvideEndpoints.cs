namespace AirportTracker.EndpointProvider
{
    public interface IProvideEndpoints
    {
        public event Action<(Airport?, Airport?)> EndpointsUpdated;
        public event Action<List<Airport>> ShortestPathGenerated;
        public void SetSource(Airport source);
        public void SetDestination(Airport destination);
        public (Airport?, Airport?) GetEndpoints();
        public Task GetShortestPath();
    }
}
