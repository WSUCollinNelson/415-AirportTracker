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
		
		// TODO can do a shortest path, but for now just take the first one
		public List<Route> GetPathBetweenAirports(Airport source, Airport destination)
		{
			List<Route> path = new List<Route>();

            var adjacentQueryResult = RunCypher(
				@$"MATCH(a:Airport {{id: {source.Id}}})--> (n)
				RETURN n.id").Result;

			Console.WriteLine(adjacentQueryResult["n.id"]);

            // only worry about source id and dest id for the time being

            return path;
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
	}
}
