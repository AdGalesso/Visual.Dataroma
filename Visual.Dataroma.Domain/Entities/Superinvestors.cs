namespace Visual.Dataroma.Domain
{
    public class Superinvestors
    {
        public Guid Id { get; set; }
        public string PortfolioManager { get; set; }
        public decimal PortfolioValue { get; set; }
        public int NumberOfStocks { get; set; }
        public string ManagerLink { get; set; }
        public string ManagerBase64 { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
    