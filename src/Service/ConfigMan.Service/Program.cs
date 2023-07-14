using System.Security.Claims;
using System.Text;
using ConfigMan.Data;
using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using ConfigMan.Service;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserInfo, ClaimsUserInfo>();

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
            .AllowCredentials()
            .WithExposedHeaders("ETag");
    });
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection"));
    opts.Events.MetadataConfig.HeadersEnabled = true;
    opts.Schema.For<EnvironmentSet>().SoftDeleted();
    opts.Projections.Add<ActiveEnvironmentSetProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Add<UsersProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Add<EnvironmentSetHistoryTransformation>(ProjectionLifecycle.Async);
    opts.Projections.Add<ApplicationChangeHistoryTransformation>(ProjectionLifecycle.Async);
    opts.Projections.Add<EnvironmentSetApplicationsProjection>(ProjectionLifecycle.Async);
    opts.Projections.Add<ActiveApplicationProjection>(ProjectionLifecycle.Inline);

    //var agent = await StartDaemon();
    //opts.Projections.Snapshot<EnvironmentSet>(SnapshotLifecycle.Async, asyncConfiguration => {asyncConfiguration.}) //snapshot will create an actual document
    //opts.Projections.LiveStreamAggregation<EnvironmentSet>(); //live can only be used with AggregateStream

    //LoadAsync will actually load the document from the table
}).AddAsyncDaemon(DaemonMode.Solo); //TODO: adjust this for prod?

builder.Services.AddMediatR(x=>
{
    x.RegisterServicesFromAssemblyContaining<CreateEnvironmentSetHandler>();
});

builder.Services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

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

app.Run();