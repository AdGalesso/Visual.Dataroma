using Dapper;
using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Visual.Dataroma.Domain;
using Visual.Datarama;

class Program
{
    static async Task Main(string[] args)
    {
        var url = "https://www.dataroma.com/m/managers.php";

        var superinvestors = await ScrapePortfolioManagersAsync(url);

        await GetAIImageAsync(superinvestors);

        UpsertSuperinvestors(superinvestors);
    }

    // Function to scrape the portfolio superinvestors' data
    private static async Task<List<Superinvestors>> ScrapePortfolioManagersAsync(string url)
    {
        var superinvestors = new List<Superinvestors>();

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
                    var manager = new Superinvestors
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

    private static void UpsertSuperinvestors(List<Superinvestors> superinvestors)
    {
        string connectionString = "Server=localhost;Database=visual.dataroma;User Id=sa;Password=quantumPassw0rd;";

        using var connection = new SqlConnection(connectionString);

        connection.Open();

        foreach (var s in superinvestors)
        {
            var checkQuery = "SELECT COUNT(1) FROM Superinvestors WHERE PortfolioManager = @PortfolioManager";
            bool exists = connection.ExecuteScalar<int>(checkQuery, new { s.PortfolioManager }) > 0;

            using (var checkCommand = new SqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@PortfolioManager", s.PortfolioManager);

                exists = (int)checkCommand.ExecuteScalar() > 0;
            }

            if (exists)
            {
                // Update the record if it exists
                var updateQuery = @"
                UPDATE Superinvestors
                SET PortfolioValue = @PortfolioValue,
                    NumberOfStocks = @NumberOfStocks,
                    ManagerLink = @ManagerLink,
                    ManagerBase64 = @ManagerBase64,
                    UpdatedAt = @UpdatedAt
                WHERE PortfolioManager = @PortfolioManager";

                connection.Execute(updateQuery, new
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
                INSERT INTO Superinvestors (PortfolioManager, PortfolioValue, NumberOfStocks, ManagerLink, ManagerBase64, UpdatedAt)
                VALUES (@PortfolioManager, @PortfolioValue, @NumberOfStocks, @ManagerLink, @ManagerBase64, @UpdatedAt)";

                connection.Execute(insertQuery, new
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

    private static async Task GetAIImageAsync(List<Superinvestors> superinvestors)
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
}