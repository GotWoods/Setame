using System.Text;
using FluentValidation;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Setame.Data;
using Setame.Data.Data;
using Setame.Data.Handlers.Applications;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;
using Setame.Data.Projections;
using Setame.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserInfo, ClaimsUserInfo>();
builder.Services.AddScoped<IEmailService, EMailService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IEnvironmentSetRepository, EnvironmentSetRepository>();
builder.Services.AddScoped<IEnvironmentSetApplicationAssociationRepository, EnvironmentSetApplicationAssociationRepository>();
builder.Services.AddOptions<MailSettings>().BindConfiguration("MailSettings");
builder.Services.AddScoped(typeof(IDocumentSessionHelper<>), typeof(DocumentSessionHelper<>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
builder.Services.AddValidatorsFromAssemblyContaining<CreateApplicationValidator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
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
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
    opts.Events.MetadataConfig.HeadersEnabled = true;
    opts.Schema.For<EnvironmentSet>().SoftDeleted();
    opts.Projections.Add<ActiveEnvironmentSetProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Add<UsersProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Add<EnvironmentSetHistoryTransformation>(ProjectionLifecycle.Async);
    opts.Projections.Add<ApplicationChangeHistoryTransformation>(ProjectionLifecycle.Async);
    opts.Projections.Add<EnvironmentSetApplicationsProjection>(ProjectionLifecycle.Async);
    opts.Projections.Add<ActiveApplicationProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Add<PendingPasswordResetProjection>(ProjectionLifecycle.Async);

    //var agent = await StartDaemon();
    //opts.Projections.Snapshot<EnvironmentSet>(SnapshotLifecycle.Async, asyncConfiguration => {asyncConfiguration.}) //snapshot will create an actual document
    //opts.Projections.LiveStreamAggregation<EnvironmentSet>(); //live can only be used with AggregateStream

    //LoadAsync will actually load the document from the table
}).AddAsyncDaemon(DaemonMode.HotCold).UseLightweightSessions();

builder.Services.AddMediatR(x => { x.RegisterServicesFromAssemblyContaining<CreateEnvironmentSetHandler>(); });

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "wwwroot"; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpMetrics();
app.UseHttpsRedirection();
//app.UseFileServer();
//app.UseDefaultFiles(); //allows serving index.html as a default


app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();
app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("ReactAppPolicy");
app.UseSpaStaticFiles();


app.MapWhen(x => !x.Request.Path.Value!.StartsWith("/api"), config =>
{
    config.UseSpa(spa =>
    {
        if (builder.Environment.IsDevelopment()) spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    });
});

Log.Logger.Information("Application started");

app.Run();

Log.Logger.Information("Application exiting");
Log.CloseAndFlush();

namespace Setame.Web
{
    public partial class Program { }
}