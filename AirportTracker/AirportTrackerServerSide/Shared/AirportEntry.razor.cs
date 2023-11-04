using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class AirportEntry
    {
        [Parameter]
        public Airport Source { get; set; }
    }
}
