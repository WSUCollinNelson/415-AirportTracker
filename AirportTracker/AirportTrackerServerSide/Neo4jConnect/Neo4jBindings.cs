using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System.Reflection;

namespace AirportTracker.Neo4jConnect
{
    public class Neo4jBindings : IBindNeo4j
    {
        private readonly IDriver driver;
        private readonly List<Airport> airportCache = new List<Airport>();
        private readonly List<Route> routeCache = new List<Route>();

        public Neo4jBindings() 
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));
        }

        public async Task<List<Airport>> GetAirportsWithFilter(string filter = "", int maxCount = 10) 
        {
            airportCache.Clear();
            using var session = driver.AsyncSession();
            await session.ExecuteWriteAsync(
                async tx => {
                    IResultCursor result = await tx.RunAsync($@"MATCH (n:Airport) 
                        {(filter == "" ? "" : $"WHERE {filter}")}
                        RETURN n LIMIT {maxCount}");
                    var output = result.AsObjectsAsync<Airport>();
                    await foreach (Airport name in output)
                    {
                        airportCache.Add(name);
                    }
                }
            );

            return airportCache;
        }
    }
}
