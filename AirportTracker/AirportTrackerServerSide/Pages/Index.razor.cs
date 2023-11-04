using AirportTracker.DataManagerServices;
using AirportTracker.Neo4jConnect;
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

        protected override async Task OnInitializedAsync() 
        {
            if (Neo4jBindings != null)
            {
                AirportsToDisplay = await Neo4jBindings.GetAirportsWithFilter("", 10);
            }

            PopulateDetails.NewAirportSelected += RecomputeDetails;

            PaulTest paulTest = new PaulTest();
            paulTest.HelloWorld();

            await base.OnInitializedAsync();
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
