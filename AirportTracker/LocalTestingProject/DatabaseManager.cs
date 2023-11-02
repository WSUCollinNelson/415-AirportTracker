using Neo4j.Driver;
using System;

namespace AirportTracker
{
	public class DatabaseManager
	{
		private readonly IDriver _driver;

		public DatabaseManager()
		{
			_driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));
		}

		public string GetAirport(string city)
		{
			return RunCypher(@"MATCH (n) 
				RETURN n.name LIMIT 1").Result;
		}

		private async Task<string> RunCypher(string cypher)
		{
			using var session = _driver.AsyncSession();
			string result = await session.ExecuteWriteAsync(
				async tx => {
                    var localResult = await tx.RunAsync(cypher);
					var record = await localResult.SingleAsync();
                    return record[0].As<string>();
                }
			);
			return result;
		}
	}
}
