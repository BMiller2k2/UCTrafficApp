namespace UCTrafficApp.Models
{
    public class TrafficDataModel
    {
        public int Id { get; set; }
        public string RouteName { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string TrafficStatus { get; set; } // "Clear", "Moderate", "Heavy", "Congested"
        public int EstimatedTravelMinutes { get; set; }
        public string TrafficDescription { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
