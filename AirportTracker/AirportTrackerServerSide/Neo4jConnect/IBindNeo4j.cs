namespace AirportTracker.Neo4jConnect
{
    public interface IBindNeo4j
    {
        Task<List<Airport>> GetAirportsWithFilter(string filter, int maxCount);
        Task<int> CountQuery(string filter);
        void ForeachAirport(Action<Airport> action);
    }
}
