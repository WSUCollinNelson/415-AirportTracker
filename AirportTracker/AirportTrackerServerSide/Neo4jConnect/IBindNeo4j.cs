namespace AirportTracker.Neo4jConnect
{
    public interface IBindNeo4j
    {
        Task<List<Airport>> GetAirportsWithFilter(string filter, int maxCount);
        Task<List<Route>> GetRoutesWithFilter(string filter, int maxCount);
        Task<int> CountQuery(string filter);
        void ForeachAirport(Action<Airport> action);
        Task<List<Route>> GetShortestPathBetweenAirports(Airport source, Airport destination);
        Task<List<Airport>> ShortestPathDjikstras(Airport source, Airport dest);
    }
}
