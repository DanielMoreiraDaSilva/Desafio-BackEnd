var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureConnections(builder.Configuration);
builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureSwagger();

builder.Services.Configure<AwsConfig>(options => builder.Configuration.GetSection("LocalStackAWS").Bind(options));

builder.WebHost.UseNLog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.Run();

