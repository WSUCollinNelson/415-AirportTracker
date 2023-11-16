using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseBuilder
{
    internal class CypherBindings : IDisposable
    {

        private readonly IDriver _driver;

        public CypherBindings(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }

        private async void RunCypher(string cypher)
        {
            using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(
                async tx => {
                    await tx.RunAsync(cypher);
                }
            );
        }

        public void FlushDB()
        {
            RunCypher(
                @"MATCH (n)
			DETACH DELETE n"
            );
        }

        public void CreateAirport(List<string> line)
        {
            RunCypher(
                $@"CREATE (a:Airport {{id: {line[0]}, name: ""{line[1]}"", city: ""{line[2]}"", country: ""{line[3]}"", iata: ""{line[4]}"", iaco: ""{line[5]}"", timezone: ""{line[9]}"", tztime: ""{line[11]}""}})"
            );
        }

        public string CreateRoute(List<string> line)
        {
            //NOTE: Mostly fill in the blank here, but airline ID's with the value \N are replaced with -1 so that the field can be stored as an int.
            RunCypher(
                $@"MATCH (source:Airport {{id: {line[3]}}}), (dest:Airport {{id: {line[5]}}})
			CREATE (source)-[:ROUTE {{airline: ""{line[0]}"", airlineid: {(line[1] == "\\N" ? -1 : line[1])}, source: ""{line[2]}"", sourceid: {line[3]}, dest: ""{line[4]}"", destid: {line[5]}}}]->(dest)"
            );
            return "";
        }

        public async void Validation(int expectedNodes, int expectedRelations) 
        {
            using var session = _driver.AsyncSession();
            var nodeCount =  (await session.ExecuteWriteAsync(
                async tx => {
                    var result = await tx.RunAsync($@"MATCH (e:Airport)
                    RETURN count(e)");

                    return (await result.SingleAsync())[0].As<int>();
                }
            )).As<int>();

            var relationsCount = (await session.ExecuteWriteAsync(
                async tx => {
                    var result = await tx.RunAsync($@"MATCH (:Airport)-[r:ROUTE]->(:Airport)
                    RETURN count(r)");

                    return (await result.SingleAsync())[0].As<int>();
                }
            )).As<int>();

            Console.WriteLine($"Node Validation: {nodeCount} of {expectedNodes} nodes loaded.");
            Console.WriteLine($"Relations Validation: {relationsCount} of {expectedRelations} routes loaded.");
        }
    }
}
