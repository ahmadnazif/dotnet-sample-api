using MySqlConnector;
using SampleApi.Services;
using SampleApi.SignalrHubs;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(x => x.AddDefaultPolicy(y => y.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.WebHost.ConfigureKestrel(x =>
{
    var port = int.Parse(config["Port"]);
    x.ListenAnyIP(port);
});

#region DB registration

var dbServer = config["Db:Server"];
var dbName = config["Db:DbName"];
var dbUserId = config["Db:UserId"];
var dbPassword = config["Db:Password"];
var dbConnTimeout = int.Parse(config["Db:ConnectionTimeoutSec"]);

MySqlConnectionStringBuilder connStr = new()
{
    Server = dbServer,
    Database = dbName,
    UserID = dbUserId,
    Password = dbPassword,
    Pooling = true,
    MinimumPoolSize = 0,
    MaximumPoolSize = 100,
    ConnectionTimeout = (uint)dbConnTimeout
};

builder.Services.AddMySqlDataSource(connStr.ToString());
builder.Services.AddScoped<IDbRepo, DbRepo>();

#endregion

#region SignalR registration

builder.Services.AddSignalR().AddMessagePackProtocol();

#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.MapHub<StatisticHub>("/stat-hub");

await app.RunAsync();
