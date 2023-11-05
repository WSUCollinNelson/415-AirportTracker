using AirportTracker.Neo4jConnect;
using AirportTracker.RankTree;
using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class AirportStats
    {
        [Inject]
        public IBindNeo4j Neo4jBindings { get; set; }

        [Parameter]
        public Airport Source { get; set; }

        [Parameter]
        public RankingTree OutRankTree { get; set; }

        [Parameter]
        public RankingTree InRankTree { get; set; }

        protected int Outgoing { get; set; }
        protected int Incoming { get; set; }
        protected int OutgoingRank { get; set; }
        protected int IncomingRank { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            Outgoing = await Neo4jBindings.CountQuery(@$"(:Airport)-[n:ROUTE]->(:Airport)
                WHERE n.sourceid = {Source.Id}");

            OutgoingRank = OutRankTree.GetRank(Outgoing);

            Incoming = await Neo4jBindings.CountQuery(@$"(:Airport)-[n:ROUTE]->(:Airport)
                WHERE n.destid = {Source.Id}");

            IncomingRank = InRankTree.GetRank(Incoming);

            await base.OnParametersSetAsync();
        }
    }
}
