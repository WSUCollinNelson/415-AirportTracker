using AirportTracker.DataManagerServices;
using AirportTracker.Neo4jConnect;
using AirportTracker.RankTree;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace AirportTracker.Pages
{
    public partial class Index
    {
        [Inject]
        private IBindNeo4j? Neo4jBindings { get; set; } = default;

        [Inject]
        private IPopulateDetails PopulateDetails { get; set; }

        protected List<Airport> AirportsToDisplay { get; set; } = new List<Airport>();
        protected string City { get { return _city; } set { _city = value; RecomposeFilter(); } }
        private string _city = "";
        protected string Country { get { return _country; } set { _country = value; RecomposeFilter(); } }
        private string _country = "";
        protected string Name { get { return _name; } set { _name = value; RecomposeFilter(); } }
        private string _name = "";
        protected string Code { get { return _code; } set { _code = value; RecomposeFilter(); } }
        private string _code = "";

        protected Airport? DisplayedAirport = null;

        protected RankingTree OutgoingRanks;
        protected Dictionary<int, int> OutgoingCounts;
        protected RankingTree IncomingRanks;
        protected Dictionary<int, int> IncomingCounts;

        protected override async Task OnInitializedAsync()
        {
            if (Neo4jBindings != null)
            {
                AirportsToDisplay = await Neo4jBindings.GetAirportsWithFilter("", 10);
            }

            PopulateDetails.NewAirportSelected += RecomputeDetails;

            /*
            PaulTest paulTest = new PaulTest();
            paulTest.HelloWorld();
            */

            //var testShortPath = await Neo4jBindings.ShortestPathDjikstras((await Neo4jBindings.GetAirportsWithFilter("n.iata = \"PUW\"", 1))[0], (await Neo4jBindings.GetAirportsWithFilter("n.iata = \"ITM\"", 1))[0]);


            await PrecomputeRankTrees();

            await base.OnInitializedAsync();
        }

        private async Task PrecomputeRankTrees()
        {
            if (OutgoingRanks == null)
            {
                OutgoingRanks = new RankingTree();
                OutgoingCounts = new Dictionary<int, int>();
                IncomingRanks = new RankingTree();
                IncomingCounts = new Dictionary<int, int>();

                List<Route> routes = await Neo4jBindings.GetRoutesWithFilter("", 100000);
                foreach (Route route in routes)
                {
                    if (OutgoingCounts.ContainsKey(route.SourceID))
                    {
                        OutgoingCounts[route.SourceID]++;
                    }
                    else
                    {
                        OutgoingCounts.Add(route.SourceID, 1);
                    }

                    if (IncomingCounts.ContainsKey(route.DestID))
                    {
                        IncomingCounts[route.DestID]++;
                    }
                    else
                    {
                        IncomingCounts.Add(route.DestID, 1);
                    }
                }
                foreach (int key in OutgoingCounts.Keys)
                {
                    OutgoingRanks.Insert(key, OutgoingCounts[key]);
                }
                foreach (int key in IncomingCounts.Keys)
                {
                    IncomingRanks.Insert(key, IncomingCounts[key]);
                }
            }
        }

        protected async void RecomposeFilter() 
        {
            if (Neo4jBindings != null)
            {
                string filter = $"n.city CONTAINS \"{City}\" AND n.country CONTAINS \"{Country}\" AND (n.iata CONTAINS \"{Code}\" OR n.iaco CONTAINS \"{Code}\") AND n.name CONTAINS \"{Name}\"";
                AirportsToDisplay = await Neo4jBindings.GetAirportsWithFilter(filter, 10);
                RecomputeDetails(AirportsToDisplay.Count() > 0 ? AirportsToDisplay[0] : null);
            }
        }

        protected void RecomputeDetails(Airport airport) 
        {
            DisplayedAirport = airport;
            InvokeAsync(StateHasChanged);
        }
    }
}
