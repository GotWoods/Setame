using System.Security.Claims;
using System.Text;
using ConfigMan.Data;
using ConfigMan.Data.Models;
using ConfigMan.Service;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("ConfigMan.Data")));


builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IEnvironmentSetService, EnvironmentSetService>();
builder.Services.AddScoped<IEnvironmentGroupService, EnvironmentGroupService>();
builder.Services.AddScoped<IVariableGroupService, VariableGroupService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });



builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection"));
   // opts.Projections.LiveStreamAggregation<EnvironmentSet>();
}).AddAsyncDaemon(DaemonMode.Solo); //TODO: adjust this for prod?


// Add Prometheus
//builder.Services.AddHttpMetrics();

//Configure Serilog
Log.Logger = new LoggerConfiguration()
  //  .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    //.WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpMetrics();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();
app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("ReactAppPolicy");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

app.Run();