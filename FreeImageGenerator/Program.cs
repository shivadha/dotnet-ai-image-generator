// ✅ MUST be at the very top (before builder)
DateTime lastCall = DateTime.MinValue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/generate-image", (PromptRequest request) =>
{
    if (DateTime.UtcNow - lastCall < TimeSpan.FromSeconds(5))
        return Results.BadRequest("Please wait before next request.");

    lastCall = DateTime.UtcNow;

    var imageUrl =
        $"https://image.pollinations.ai/prompt/{Uri.EscapeDataString(request.Prompt)}";
    var html = $@"
        <html>
            <body style='text-align:center;font-family:sans-serif'>
                <h2>Generated Image</h2>
                <p><b>Prompt:</b> {request.Prompt}</p>
                <img src='{imageUrl}' width='512'/>
            </body>
        </html>";

    return Results.Content(html, "text/html");
});

app.Run();

record PromptRequest(string Prompt);
