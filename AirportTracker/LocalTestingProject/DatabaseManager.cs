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

		public Airport GetAirport(string city)
		{
	        var airportQueryResult = RunCypher(
                @$"MATCH (n:Airport {{city: '{city}'}})
				RETURN 
					n.name, n.id, n.city, n.country, 
					n.iata, n.iaco, n.latitude, 
					n.longitude, n.altitude, 
					n.timezone, n.dst, n.tztime, 
					n.type, n.source 
				LIMIT 1").Result;

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
