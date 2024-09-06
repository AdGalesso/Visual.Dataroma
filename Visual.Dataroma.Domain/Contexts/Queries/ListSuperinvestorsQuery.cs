using Microsoft.EntityFrameworkCore;

namespace Visual.Dataroma.Domain.Contexts.Queries
{
    public interface IListSuperinvestorsQuery : IQueries
    {
        Task<List<Superinvestors>> ListAsync(int skip, int take);
    }

    public class ListSuperinvestorsQuery(VisualDataromaContext context) : IListSuperinvestorsQuery
    {
        public async Task<List<Superinvestors>> ListAsync(int skip, int take)
        {
            return await context.Superinvestors
                .Select(s => new Superinvestors()
                {
                    Id = s.Id,
                    ManagerBase64 = s.ManagerBase64,
                    NumberOfStocks = s.NumberOfStocks,
                    PortfolioManager = s.PortfolioManager,
                    PortfolioValue  = s.PortfolioValue,
                    UpdatedAt = s.UpdatedAt,
                })
                .OrderByDescending(s => s.PortfolioValue)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
