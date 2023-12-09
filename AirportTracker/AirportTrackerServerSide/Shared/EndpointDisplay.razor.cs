﻿using AirportTracker.EndpointProvider;
using Microsoft.AspNetCore.Components;

namespace AirportTracker.Shared
{
    public partial class EndpointDisplay
    {
        [Inject]
        public IProvideEndpoints endpointProvider { get; set; }

        protected string Source { get; set; } = "";
        protected string Destination { get; set; } = "";

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender) 
            {
                endpointProvider.EndpointsUpdated += Update;
            }
            base.OnAfterRender(firstRender);
        }

        protected void Update((Airport?, Airport?) airports) 
        {
            this.Source = airports.Item1?.Name ?? "";
            this.Destination = airports.Item2?.Name ?? "";
            StateHasChanged();
        }
    }
}
