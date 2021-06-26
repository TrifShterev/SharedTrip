using System.Collections.Generic;

namespace SharedTrip.ViewModels.TripsModels
{
    public class AllTripsViewModel
    {
        public ICollection<TripViewModel> Trips { get; set; }
    }
}