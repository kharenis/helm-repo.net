using HelmRepo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HelmIndexBuilder>();
var app = builder.Build();

const string mime = "text/yaml";

var chartPath = app.Configuration.GetValue<string>("CHART_PATH");
if (String.IsNullOrWhiteSpace(chartPath))
{
    throw new NullReferenceException("CHART_PATH cannot be empty.");
}

app.MapGet("/charts", async () => { return Results.File(Path.Combine(chartPath, "index.yaml"), mime); });

app.MapPost("/charts/{chartName}", async (HelmIndexBuilder indexBuilder, HttpRequest request, string chartName) =>
    {
        if (!chartName.ToLower().EndsWith(".tar.gz"))
        {
            return Results.BadRequest("Chart name must end with .tar.gz");
        }
        var path = app.Environment.WebRootPath;
        using (FileStream fs = new FileStream(Path.Combine(chartPath, chartName.ToLower()), FileMode.Create))
        {
            await request.Body.CopyToAsync(fs);
        }

        indexBuilder.BuildIndex();

        return Results.Ok();
    })
    .Accepts<byte[]>(contentType: "application/octet-stream")
    .Produces(StatusCodes.Status200OK)
    .Produces<string>(StatusCodes.Status400BadRequest);

app.MapGet("/{chart}", async (string chart) => { return Results.File("", mime); });



await app.RunAsync();