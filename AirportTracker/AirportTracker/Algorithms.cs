using Neo4j.Driver;
using System;

namespace AirportTracker
{
	public class Algorithms
	{
		private readonly IDriver _driver;

		public Algorithms()
		{
			_driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));
		}

		public string GetAirport(string city)
		{
			RunCypher(@"MATCH (n) RETURN n LIMIT 1");
			return "";
		}

		private async void RunCypher(string cypher)
		{
			using var session = _driver.AsyncSession();
			await session.ExecuteWriteAsync(
				async tx => {
					await tx.RunAsync(cypher);
                    //var test = await tx.RunAsync(cypher);
                    //Console.WriteLine(test.ToString());
                }
			);
		}
	}
}
