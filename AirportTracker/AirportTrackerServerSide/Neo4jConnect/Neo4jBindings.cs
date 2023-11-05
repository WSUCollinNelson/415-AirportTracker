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

        public async Task<List<Airport>> GetAirportsWithFilter(string filter = "", int maxCount = 10000)
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

        public async Task<List<Route>> GetRoutesWithFilter(string filter = "", int maxCount = 100000)
        {
            using var session = driver.AsyncSession();
            List<Route> output = await session.ExecuteWriteAsync(
                async tx => {
                    List<Route> routeCache = new List<Route>();
                    IResultCursor result = await tx.RunAsync($@"MATCH (s:Airport)-[n:ROUTE]->(d:Airport) 
                        {(filter == "" ? "" : $"WHERE {filter}")}
                        RETURN n LIMIT {maxCount}");
                    var output = result.AsObjectsAsync<Route>();
                    await foreach (Route name in output)
                    {
                        routeCache.Add(name);
                    }
                    return routeCache;
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

        public async Task<List<Route>> GetShortestPathBetweenAirports(Airport source, Airport destination)
        {
            List<List<Route>> paths = new List<List<Route>>();

            List<Route> startingRoutes = await GetRoutesWithFilter($"n.sourceid = {source.Id}");
            foreach (Route startingRoute in startingRoutes)
            {
                paths.Add(new List<Route> { startingRoute });
            }

            while (true)
            {
                List<List<Route>> pathsOfOneStepFurther = new List<List<Route>>();
                foreach (List<Route> path in paths)
                {
                    int lastAirportInPathID = path.Last<Route>().DestID;
                    if (lastAirportInPathID == destination.Id)
                    {
                        return path;
                    }
                    List<Route> outgoingRoutesFromLastAirport = await GetRoutesWithFilter($"n.sourceid = {lastAirportInPathID}");
                    foreach (Route additionalRouteToPath in outgoingRoutesFromLastAirport)
                    {
                        List<Route> pathOfOneStepFurther = new List<Route>(path) { additionalRouteToPath };
                        pathsOfOneStepFurther.Add(pathOfOneStepFurther);
                    }
                }
                paths = pathsOfOneStepFurther;
            }
        }

        public async Task<List<Airport>> ShortestPathDjikstras(Airport source, Airport dest) 
        {
            Dictionary<int, (int, int)> distances = new Dictionary<int, (int, int)>(); //Dictionary nodeid -> (dist, prev)
            distances.Add(source.Id, (0, -1));
            HashSet<int> visited = new HashSet<int>();
            Queue<int> nextNodes = new Queue<int>();

            int current = source.Id;

            while (true) 
            {
                visited.Add(current);
                if (current == dest.Id)
                {
                    break;
                }
                
                List<Route> connections = await GetRoutesWithFilter($"n.sourceid = {current}");
                foreach (Route connection in connections) 
                {
                    if (distances.ContainsKey(connection.DestID))
                    {
                        if (distances[current].Item1 + 1 < distances[connection.DestID].Item1)
                        {
                            distances[connection.DestID] = (distances[current].Item1 + 1, current);
                        }
                    }
                    else 
                    {
                        distances.Add(connection.DestID, (distances[current].Item1 + 1, current));
                    }

                    if (!visited.Contains(connection.DestID) && !nextNodes.Contains(connection.DestID)) 
                    {
                        nextNodes.Enqueue(connection.DestID);
                    }
                }

                if (nextNodes.Count() == 0)
                {
                    break;
                }
                else 
                {
                    current = nextNodes.Dequeue();
                }
            }

            List<Airport> output = new List<Airport>();
            if (visited.Contains(dest.Id))
            {
                current = dest.Id;
                while (current != -1) 
                {
                    output.Add((await GetAirportsWithFilter($"n.id = {current}", 1))[0]);
                    current = distances[current].Item2;
                }
                output.Reverse();
            }
            return output;
        }
    }
}
