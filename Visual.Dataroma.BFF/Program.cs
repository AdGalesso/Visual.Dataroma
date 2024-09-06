using Visual.Dataroma.BFF;
using Visual.Dataroma.Domain.Contexts;
using Visual.Dataroma.Domain.Contexts.Queries;
using static Visual.Dataroma.BFF.SuperinvestorsEndpoint;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy => {
            policy.AllowAnyOrigin().AllowAnyHeader();
        });
    })
    .AddDbContext<VisualDataromaContext>()
    .AddTransient<IListSuperinvestorsQuery, ListSuperinvestorsQuery>();

var app = builder.Build();

app.MapSuperinvestorApi();
app.UseCors();
app.Run();