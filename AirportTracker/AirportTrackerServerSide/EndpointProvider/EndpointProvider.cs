using AirportTracker.Neo4jConnect;

namespace AirportTracker.EndpointProvider
{
    public class EndpointProvider : IProvideEndpoints
    {
        private IBindNeo4j neo4jBindings;

        private (Airport?, Airport?) endpoints = (null, null);

        public event Action<(Airport?, Airport?)> EndpointsUpdated;
        public event Action<List<Airport>> ShortestPathGenerated;

        public EndpointProvider(IBindNeo4j neo4jBindings) 
        {
            this.neo4jBindings = neo4jBindings;
        }

        public (Airport?, Airport?) GetEndpoints()
        {
            return endpoints;
        }

        public async Task GetShortestPath()
        {
            if (endpoints.Item1 == null || endpoints.Item2 == null) return;
            List<Airport> airports = await neo4jBindings.ShortestPathDjikstras(endpoints.Item1, endpoints.Item2);
            ShortestPathGenerated(airports);
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
