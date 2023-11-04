using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System.Reflection;

namespace AirportTracker.Neo4jConnect
{
    public class Neo4jBindings : IBindNeo4j
    {
        private readonly IDriver driver;
        private readonly List<Route> routeCache = new List<Route>();

        public Neo4jBindings()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));
        }

        public async Task<List<Airport>> GetAirportsWithFilter(string filter = "", int maxCount = 10)
        {
            using var session = driver.AsyncSession();
            List<Airport> output = await session.ExecuteWriteAsync(
                async tx => {
                    List<Airport> airportCache = new List<Airport>();
                    IResultCursor result = await tx.RunAsync($@"MATCH (n:Airport) 
                        {(filter == "" ? "" : $"WHERE {filter}")}
                        RETURN n LIMIT {maxCount}");
                    var output = result.AsObjectsAsync<Airport>();
                    await foreach (Airport name in output)
                    {
                        airportCache.Add(name);
                    }
                    return airportCache;
                }
            );

            return output;
        }

        public async Task<int> CountQuery(string filter)
        {
            using var session = driver.AsyncSession();
            int output = await session.ExecuteWriteAsync(
                async tx => {
                    IResultCursor result = await tx.RunAsync($@"MATCH {filter}
                        RETURN count(n)");
                    return (await result.SingleAsync())[0].As<int>();
                }
            );

            return output;
        }

        public async void ForeachAirport(Action<Airport> action) 
        {
            using var session = driver.AsyncSession();
            await session.ExecuteWriteAsync(
                async tx => {
                    List<Airport> airportCache = new List<Airport>();
                    IResultCursor result = await tx.RunAsync($@"MATCH (n:Airport) RETURN n");
                    var output = result.AsObjectsAsync<Airport>();
                    await foreach (Airport airport in output)
                    {
                        action(airport);
                    }
                }
            );
        }
    }
}
