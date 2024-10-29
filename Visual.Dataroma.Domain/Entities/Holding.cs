namespace Visual.Dataroma.Domain.Entities
{
    public class Holding
    {
        public Guid Id { get; set; }
        public Guid SuperinvestorId { get; set; }
        public string StockCode { get; set; }
        public decimal PortfolioPercentage { get; set; }
        public string LastActivity { get; set; }
        public int NumberOfStocks { get; set; }
        public decimal ReportedPrice { get; set; }
    }
}
