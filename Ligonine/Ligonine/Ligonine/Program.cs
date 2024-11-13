using Ligonine.Data;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<HospitalDbContext>();

var app = builder.Build();

app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
