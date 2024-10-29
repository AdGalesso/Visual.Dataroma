using Microsoft.EntityFrameworkCore;
using Visual.Dataroma.Domain.Entities;

namespace Visual.Dataroma.Domain.Contexts.Queries
{
    public interface IListSuperinvestorsQuery : IQueries
    {
        Task<List<Superinvestor>> ListAsync(int skip, int take);
    }

    public class ListSuperinvestorsQuery(VisualDataromaContext context) : IListSuperinvestorsQuery
    {
        public async Task<List<Superinvestor>> ListAsync(int skip, int take)
        {
            return await context.Superinvestors
                .Select(s => new Superinvestor()
                {
                    Id = s.Id,
                    ManagerBase64 = s.ManagerBase64,
                    NumberOfStocks = s.NumberOfStocks,
                    PortfolioManager = s.PortfolioManager,
                    PortfolioValue  = s.PortfolioValue,
                    UpdatedAt = s.UpdatedAt,
                    Holdings = s.Holdings.Select(h => new Holding()
                    {
                        StockCode = h.StockCode,
                        NumberOfStocks = h.NumberOfStocks,
                        ReportedPrice = h.ReportedPrice,
                        LastActivity = h.LastActivity,
                        PortfolioPercentage = h.PortfolioPercentage,
                    })
                    .OrderByDescending(h => h.PortfolioPercentage)
                    .Take(10)
                    .ToList()
                })
                .OrderByDescending(s => s.PortfolioValue)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
