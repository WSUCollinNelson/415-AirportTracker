namespace AirportTracker.EndpointProvider
{
    public class EndpointProvider : IProvideEndpoints
    {
        private (Airport?, Airport?) endpoints = (null, null);

        public event Action<(Airport?, Airport?)> EndpointsUpdated;

        public (Airport?, Airport?) GetEndpoints()
        {
            return endpoints;
        }

        public void SetDestination(Airport destination)
        {
            endpoints.Item2 = destination;
            EndpointsUpdated(endpoints);
        }

        public void SetSource(Airport source)
        {
            endpoints.Item1 = source;
            EndpointsUpdated(endpoints);
        }
    }
}
