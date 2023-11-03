using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class AirportEntry
    {
        [Parameter]
        public string ID { get; set; } = "";
    }
}
