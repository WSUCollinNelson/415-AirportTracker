using AirportTracker.DataManagerServices;
using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class AirportEntry
    {
        [Inject]
        private IPopulateDetails DetailsPopulator { get; set; }

        [Parameter]
        public Airport Source { get; set; }

        protected void OnSelected(EventArgs e) 
        {
            DetailsPopulator?.SelectAirport(Source);
        }
    }
}
