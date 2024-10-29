using Dapper;
using HtmlAgilityPack;
using Npgsql;
using System.Text.RegularExpressions;
using Visual.Datarama;
using Visual.Dataroma.Domain.Entities;
using Z.Dapper.Plus;

class Program
{
    private readonly static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=quantumPassw0rd;Database=visual.dataroma";
    private readonly static string baseDataroma = "https://www.dataroma.com";

    static async Task Main(string[] args)
    {
        if (args.Contains("superinvestors"))
        {
            var url = $"{baseDataroma}/m/managers.php";

            var superinvestors = await ScrapePortfolioManagersAsync(url);

            await GetAIImageAsync(superinvestors);

            await UpsertSuperinvestorsAsync(superinvestors);
        }

        if (args.Contains("holdings"))
        {
            var superinvestors = await GetExistingSuperinvestorsAsync();

            var data = await ScrapePortfolioHoldingsAsync(superinvestors);

            await UpsertStockHoldingsAsync(data);
        }
    }

    #region "  Superinvestors  "

    private static async Task<List<Superinvestor>> ScrapePortfolioManagersAsync(string url)
    {
        var superinvestors = new List<Superinvestor>();

        // Initialize HttpClient and HtmlAgilityPack
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var document = new HtmlDocument();
        document.LoadHtml(html);

        // Find the table that holds the data
        var tableRows = document.DocumentNode.SelectNodes("//table[@id='grid']/tbody/tr");

        if (tableRows != null)
        {
            foreach (var row in tableRows)
            {
                // Extract individual cells
                var cells = row.SelectNodes("td");
                var a = cells[0].Elements("a").SingleOrDefault();
                var href = a?.Attributes.Where(att => att.Name == "href").FirstOrDefault();

                if (cells != null && cells.Count >= 5)
                {
                    var manager = new Superinvestor
                    {
                        PortfolioManager = cells[0].InnerText.Trim(),
                        PortfolioValue = cells[1].InnerText.MoneyToDecimal(),
                        NumberOfStocks = int.Parse(cells[2].InnerText.Trim()),
                        ManagerLink = href != null ? href.Value : string.Empty,
                        UpdatedAt = DateTime.Now,
                    };

                    superinvestors.Add(manager);
                }
            }
        }

        return superinvestors;
    }

    private static async Task UpsertSuperinvestorsAsync(List<Superinvestor> superinvestors)
    {
        using var connection = new NpgsqlConnection(connectionString);

        connection.Open();

        foreach (var s in superinvestors)
        {
            var checkQuery = "SELECT COUNT(1) FROM Superinvestor WHERE PortfolioManager = @PortfolioManager";
            bool exists = connection.ExecuteScalar<int>(checkQuery, new { s.PortfolioManager }) > 0;

            using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@PortfolioManager", s.PortfolioManager);

                var execute = await checkCommand.ExecuteScalarAsync();

                exists = execute != null && (Int64)execute > 0;
            }

            if (exists)
            {
                // Update the record if it exists
                var updateQuery = @"
                UPDATE Superinvestor
                SET PortfolioValue = @PortfolioValue,
                    NumberOfStocks = @NumberOfStocks,
                    ManagerLink = @ManagerLink,
                    ManagerBase64 = @ManagerBase64,
                    UpdatedAt = @UpdatedAt
                WHERE PortfolioManager = @PortfolioManager";

                await connection.ExecuteAsync(updateQuery, new
                {
                    s.PortfolioManager,
                    s.PortfolioValue,
                    s.NumberOfStocks,
                    s.ManagerLink,
                    s.ManagerBase64,
                    s.UpdatedAt
                });
            }
            else
            {
                var insertQuery = @"
                INSERT INTO Superinvestor (PortfolioManager, PortfolioValue, NumberOfStocks, ManagerLink, ManagerBase64, UpdatedAt)
                VALUES (@PortfolioManager, @PortfolioValue, @NumberOfStocks, @ManagerLink, @ManagerBase64, @UpdatedAt)";

                await connection.ExecuteAsync(insertQuery, new
                {
                    s.PortfolioManager,
                    s.PortfolioValue,
                    s.NumberOfStocks,
                    s.ManagerLink,
                    s.ManagerBase64,
                    s.UpdatedAt
                });
            }
        }
    }

    private static async Task GetAIImageAsync(List<Superinvestor> superinvestors)
    {
        foreach (var s in superinvestors)
        {
            using HttpClient client = new();

            try
            {
                var prompt = Regex.Replace(s.PortfolioManager, "[^a-zA-Z0-9 ]", string.Empty);

                var model = "flux-realism";
                var width = 256;
                var height = 512;
                var seed = new Random(1).Next(230);

                var url = $"https://image.pollinations.ai/prompt/{prompt}?model={model}&width={width}&height={height}&seed={seed}&nologo=true&enhance=true";


                HttpResponseMessage response = await client.GetAsync(url);
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                s.ManagerBase64 = Convert.ToBase64String(imageBytes);
            }
            catch (Exception)
            {
                s.ManagerBase64 = string.Empty;
            }
        }
    }

    #endregion

    #region "  Holdings  "

    private static async Task<List<Superinvestor>> GetExistingSuperinvestorsAsync()
    {
        using var connection = new NpgsqlConnection(connectionString);

        connection.Open();

        var superinvestorsQuery = "SELECT id, manager_link as ManagerLink FROM public.superinvestors;";
        var superinvestors = await connection.QueryAsync<Superinvestor>(superinvestorsQuery);

        return superinvestors.ToList();
    }

    private static async Task<(List<Stock>, List<Holding>)> ScrapePortfolioHoldingsAsync(List<Superinvestor> superinvestors)
    {
        var stocks = new List<Stock>();
        var holdings = new List<Holding>();

        foreach (var s in superinvestors)
        {
            // Initialize HttpClient and HtmlAgilityPack
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync($"{baseDataroma}{s.ManagerLink}");
            var document = new HtmlDocument();
            document.LoadHtml(html);

            // Find the table that holds the data
            var tableRows = document.DocumentNode.SelectNodes("//table[@id='grid']/tbody/tr");

            if (tableRows != null)
            {
                foreach (var row in tableRows)
                {
                    var cells = row.SelectNodes("td");

                    if (cells != null && cells.Count >= 10)
                    {
                        var stockCode = cells[1].InnerText.Split("-");

                        if (!stocks.Any(s => s.Code == stockCode[0].Trim()))
                        {
                            stocks.Add(new Stock()
                            {
                                Code = stockCode[0].Trim(),
                                Name = stockCode[1].Trim(),
                            });
                        }

                        var holding = new Holding()
                        {
                            SuperinvestorId = s.Id,
                            StockCode = stockCode[0].Trim(),
                            PortfolioPercentage = Convert.ToDecimal(cells[2].InnerText),
                            LastActivity = cells[3].InnerText,
                            NumberOfStocks = int.Parse(cells[4].InnerText.Replace(",", string.Empty)),
                            ReportedPrice = cells[5].InnerText.MoneyToDecimal(),
                        };

                        holdings.Add(holding);
                    }
                }
            }
        }

        return (stocks, holdings);
    }

    private static async Task UpsertStockHoldingsAsync((List<Stock> stocks, List<Holding> holdings) data)
    {
        using var connection = new NpgsqlConnection(connectionString);

        DapperPlusManager.Entity<Stock>()
            .Table("stock")
            .Key(s => s.Code);

        await connection.BulkMergeAsync(data.stocks);

        DapperPlusManager.Entity<Holding>()
            .Table("holding")
            .Key(h => new { superinvestor_id = h.SuperinvestorId, stock_code = h.StockCode })
            .Map(h => new
            {
                portfolio_percentage = h.PortfolioPercentage,
                last_activity = h.LastActivity,
                number_of_stocks = h.NumberOfStocks,
                reported_price = h.ReportedPrice,
            });

        await connection.BulkMergeAsync(data.holdings);
    }

    #endregion
}