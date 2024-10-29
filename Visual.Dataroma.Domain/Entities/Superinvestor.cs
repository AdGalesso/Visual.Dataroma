using Visual.Dataroma.Domain.Entities;

namespace Visual.Dataroma.Domain.Entities
{
    public class Superinvestor

    {
        public Guid Id { get; set; }
        public string PortfolioManager { get; set; }
        public decimal PortfolioValue { get; set; }
        public int NumberOfStocks { get; set; }
        public string ManagerLink { get; set; }
        public string ManagerBase64 { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Holding> Holdings { get; set; }
    }
}
    