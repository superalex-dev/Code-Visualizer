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
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();