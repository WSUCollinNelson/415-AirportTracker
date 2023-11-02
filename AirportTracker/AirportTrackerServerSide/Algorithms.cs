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
			RunCypher(@"MATCH (n) 
				RETURN n.name LIMIT 1");
			return "";
		}

		private async void RunCypher(string cypher)
		{
			using var session = _driver.AsyncSession();
			await session.ExecuteWriteAsync(
				async tx => {
                    var test = await tx.RunAsync(cypher);
					var testString = (await test.SingleAsync())[0].As<string>();
                    Console.WriteLine(testString);
                }
			);
		}
	}
}
