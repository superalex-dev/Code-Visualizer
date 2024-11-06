using Code_Visualizer_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IGitHubService>(sp =>
{
    var configuration = sp.GetService<IConfiguration>();
    var token = configuration["GitHub:Token"] ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    return new GitHubService(token, sp.GetRequiredService<ILogger<GitHubService>>());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowMyOrigin",
        policy  =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowMyOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();