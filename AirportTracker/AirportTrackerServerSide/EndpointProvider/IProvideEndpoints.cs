namespace AirportTracker.EndpointProvider
{
    public interface IProvideEndpoints
    {
        public event Action<(Airport?, Airport?)> EndpointsUpdated;
        public void SetSource(Airport source);
        public void SetDestination(Airport destination);
        public (Airport?, Airport?) GetEndpoints();
    }
}
