namespace AirportTracker.Neo4jConnect
{
    public interface IBindNeo4j
    {
        Task<List<Airport>> GetAirportsWithFilter(string filter, int maxCount);
    }
}
