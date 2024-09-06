using Visual.Dataroma.Domain.Contexts.Queries;

namespace Visual.Dataroma.BFF
{
    public static class SuperinvestorsEndpoint
    {
        public static IEndpointRouteBuilder MapSuperinvestorApi(this IEndpointRouteBuilder app)
        {
            var endpoint = app.MapGroup("v1/superinvestors");

            endpoint.MapGet("/", async (int skip, int take, IListSuperinvestorsQuery query) =>
            {
                var s = await query.ListAsync(skip, take);

                return Results.Ok(s);
            });

            return app;
        }
    }
}
