using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ConfigMan.Data;

public interface IApplicationService
{
    Task<IEnumerable<Application>> GetApplicationsAsync();
    Task<Application?> GetApplicationByIdAsync(string name);
    Task<Application> CreateApplicationAsync(Application application);
    Task UpdateApplicationAsync(Application application);
    Task DeleteApplicationAsync(string name);
    Task<IEnumerable<Setting>> GetApplicationSettingsAsync(Guid applicationId);
    Task CreateApplicationSettingAsync(int applicationId, Setting setting);
    Task UpdateApplicationSettingAsync(int applicationId, Setting setting);
    Task AddApplicationSetting(Guid applicationId, Setting setting);
    Task AddEnvironmentSetting(Guid applicationId, string environment, Setting setting);
}

public class ApplicationService : IApplicationService
{
    private readonly AppDbContext _dbContext;

    public ApplicationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Application>> GetApplicationsAsync()
    {
        return await _dbContext.Applications.ToListAsync();
    }

    public async Task<Application?> GetApplicationByIdAsync(string name)
    {
        return await _dbContext.Applications.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<Application> CreateApplicationAsync(Application application)
    {
        if (application == null) throw new ArgumentNullException(nameof(application));

        await _dbContext.Applications.AddAsync(application);
        await _dbContext.SaveChangesAsync();

        return application;
    }

    public async Task UpdateApplicationAsync(Application application)
    {
        if (application == null) throw new ArgumentNullException(nameof(application));

        _dbContext.Applications.Update(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteApplicationAsync(string name)
    {
        var application = await _dbContext.Applications.FirstOrDefaultAsync(x=>x.Name == name);
        if (application == null) throw new InvalidOperationException("Application not found.");

        _dbContext.Applications.Remove(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Setting>> GetApplicationSettingsAsync(Guid applicationId)
    {
        var application = await _dbContext.Applications.FindAsync(applicationId);
        if (application == null) throw new InvalidOperationException("Application not found.");

        return application.ApplicationDefaults;

        // return await _dbContext.Settings
        //     .Where(s => s.Id == applicationId)
        //     .ToListAsync();
    }

    public async Task CreateApplicationSettingAsync(int applicationId, Setting setting)
    {
        var application = await _dbContext.Applications.FindAsync(applicationId);
        // if (setting == null) throw new ArgumentNullException(nameof(setting));
        //
        // await _dbContext.Settings.AddAsync(setting);
        // await _dbContext.SaveChangesAsync();
        //
        // return setting;
      //  return Task.CompletedTask;
    }

    public async Task UpdateApplicationSettingAsync(int applicationId, Setting setting)
    {
        var application = await _dbContext.Applications.FindAsync(applicationId);
        // if (setting == null) throw new ArgumentNullException(nameof(setting));
        //
        // _dbContext.Settings.Update(setting);
        // await _dbContext.SaveChangesAsync();
        //return Task.CompletedTask;
    }

    public async Task AddApplicationSetting(Guid applicationId, Setting setting)
    {
        var application = await _dbContext.Applications.FindAsync(applicationId);
        if (application == null) throw new InvalidOperationException("Application not found.");

        application.ApplicationDefaults ??= new List<Setting>();
        application.ApplicationDefaults.Add(setting);

        _dbContext.Entry(application).Property(x => x.ApplicationDefaults).IsModified = true; //EF does not do change detection on jsonb objects
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddEnvironmentSetting(Guid applicationId, string environment, Setting setting)
    {
        var application = await _dbContext.Applications.FindAsync(applicationId);
        if (application == null) throw new InvalidOperationException("Application not found.");

        if (!application.EnvironmentSettings.ContainsKey(environment))
            application.EnvironmentSettings.Add(environment, new List<Setting>());


        var env = application.EnvironmentSettings[environment];
        env.Add(setting);

        _dbContext.Entry(application).Property(x => x.EnvironmentSettings).IsModified = true; //EF does not do change detection on jsonb objects
        await _dbContext.SaveChangesAsync();
    }
}