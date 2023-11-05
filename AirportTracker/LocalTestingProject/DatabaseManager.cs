using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace AirportTracker
{
	public class DatabaseManager
	{
		private readonly IDriver _driver;

		public DatabaseManager()
		{
			_driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));
		}
		
		public List<Route> GetShortestPathBetweenAirports(Airport source, Airport destination)
        {
            List<List<Route>> paths = new List<List<Route>>();

            List<Route> startingRoutes = GetRoutesWithFilter($"n.sourceid = {source.Id}").Result;
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
                    List<Route> outgoingRoutesFromLastAirport = GetRoutesWithFilter($"n.sourceid = {lastAirportInPathID}").Result;
                    foreach (Route additionalRouteToPath in outgoingRoutesFromLastAirport)
                    {
                        List<Route> pathOfOneStepFurther = new List<Route>(path) { additionalRouteToPath };
                        pathsOfOneStepFurther.Add(pathOfOneStepFurther);
                    }
                }
                paths = pathsOfOneStepFurther;
            }
		}

		public List<Airport> GetAirports(string city)
		{
			return GetAirportsAsync(city).Result;
		}

        private async Task<List<Airport>> GetAirportsAsync(string city)
        {
            using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(
                async tx => {
                    var queryResult = await tx.RunAsync(
						 @$"MATCH (n:Airport {{city: '{city}'}}) RETURN n");
                    var airports = queryResult.AsObjectsAsync<Airport>();
                    List<Airport> resultAirports = new List<Airport>();
                    await foreach (Airport airport in airports)
					{
						resultAirports.Add(airport);
					}
					return resultAirports;
                }
            );
        }

        private async Task<IReadOnlyDictionary<string,object>> RunCypher(string cypher)
		{
			using var session = _driver.AsyncSession();
			var result = await session.ExecuteWriteAsync(
				async tx => {
                    var localResult = await tx.RunAsync(cypher);
                    // note: SingleAsync() in this line means queries must have exactly 1 return value
                    var record = await localResult.SingleAsync();
                    return record.Values;
                }
			);
			return result;
		}

        // NOTE: the following is grabbed from main (so I didn't have to do any funky integration stuff)

        public async Task<List<Airport>> GetAirportsWithFilter(string filter = "", int maxCount = 10000)
        {
            using var session = _driver.AsyncSession();
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
            using var session = _driver.AsyncSession();
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
            using var session = _driver.AsyncSession();
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
            using var session = _driver.AsyncSession();
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
