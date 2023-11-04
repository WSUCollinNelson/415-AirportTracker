using AirportTracker.Neo4jConnect;
using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class AirportStats
    {
        [Inject]
        public IBindNeo4j Neo4jBindings { get; set; }

        [Parameter]
        public Airport Source { get; set; }

        protected int Outgoing { get; set; }
        protected int Incoming { get; set; }
        protected int OutgoingRank { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            Outgoing = await Neo4jBindings.CountQuery(@$"(:Airport)-[n:ROUTE]->(:Airport)
                WHERE n.sourceid = {Source.Id}");

            Incoming = await Neo4jBindings.CountQuery(@$"(:Airport)-[n:ROUTE]->(:Airport)
                WHERE n.destid = {Source.Id}");
            await base.OnParametersSetAsync();
        }

        private void RankConnections(int threshold) 
        {
            OutgoingRank = 0;
            Neo4jBindings.ForeachAirport(CompareRouteCount);
        }

        private async void CompareRouteCount(Airport airport) 
        {
            int count = await Neo4jBindings.CountQuery(@$"(:Airport)-[n:ROUTE]->(:Airport)
                WHERE n.sourceid = {airport.Id}");
            if (count > OutgoingRank) 
            {
                OutgoingRank++;
            }
        }
    }
}
