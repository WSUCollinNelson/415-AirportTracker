using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class ShortestPathDisplay
    {
        [Parameter]
        public List<Airport> Path { get; set; }


    }
}
