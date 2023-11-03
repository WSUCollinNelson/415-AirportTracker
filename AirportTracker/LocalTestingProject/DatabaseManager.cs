using Neo4j.Driver;
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

		public Airport? GetAirport(string city)
		{
			IReadOnlyDictionary<string, object> airportQueryResult;
            try
			{
				airportQueryResult = RunCypher(
					@$"MATCH (n:Airport {{city: '{city}'}})
					RETURN 
						n.name, n.id, n.city, n.country, 
						n.iata, n.iaco, n.latitude, 
						n.longitude, n.altitude, 
						n.timezone, n.dst, n.tztime, 
						n.type, n.source 
					LIMIT 1").Result;
			}
			catch (System.AggregateException)
			{
				return null;
			}

            Airport airport = new Airport();
            airport.Name = airportQueryResult["n.name"].As<string>();
			airport.Id = airportQueryResult["n.id"].As<int>();
			airport.City = airportQueryResult["n.city"].As<string>();
			airport.Country = airportQueryResult["n.country"].As<string>();
			airport.IATA = airportQueryResult["n.iata"].As<string>();
			airport.IACO = airportQueryResult["n.iaco"].As<string>();
			airport.Latitude = airportQueryResult["n.latitude"].As<float>();
			airport.Longitude = airportQueryResult["n.longitude"].As<float>();
			airport.Altitude = airportQueryResult["n.altitude"].As<float>();
			airport.Timezone = airportQueryResult["n.timezone"].As<string>();
			airport.DST = airportQueryResult["n.dst"].As<string>();
			airport.TzTime = airportQueryResult["n.tztime"].As<string>();
			airport.Type = airportQueryResult["n.type"].As<string>();
			airport.Source = airportQueryResult["n.source"].As<string>();

            return airport;
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

		// TODO: note, there are problems if the query does not limit the number of returns to 1
		private async Task<IReadOnlyDictionary<string,object>> RunCypher(string cypher)
		{
			using var session = _driver.AsyncSession();
			var result = await session.ExecuteWriteAsync(
				async tx => {
                    var localResult = await tx.RunAsync(cypher);
					var record = await localResult.SingleAsync();
                    return record.Values;
                }
			);
			return result;
		}
	}
}
