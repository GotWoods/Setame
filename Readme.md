## ConfigMan
ConfigMan is a configuration management server designed to have a central location to store configuration. Applications can be configured to load their configuration
on startup and be changed without requiring an application to be redeployed for a config change.

## Features
- Ability to define global deployment environments as well as global settings that all applications can inherit (e.g. you can set a log server for all applications)
- Create variables at the application level that are inherited by each environment
- .NET Configuration Provider
- Audit logs of changes

## Getting Started
1. Open the configuration file and set the ConnecitonStrings.DefaultConnection to be to a postgress database. 
1. Setup the MailSettings (this is used when a user forgets their password)
1. Open the application and login with "admin@admin.com" and a password of "admin". This will open the setup page
1. On the setup page enter a valid email address and your password
1. Now login and you are ready to start configurating!
 

## Upcoming Feature Possibilities
This is a rough roadmap of features we want to have but not a guarantee

- Case Sensitive/Insensitive Configuration flag (or just detection of case mismatches between sets/groups/applications)
- Per Application Access (read/write/Whatever people want)
- Secret storage
- Variable version pinning so that you always get the version of config that you want (may help with environment renames as well)
- Live variables changes (May be .NET provider only)
- Tracking of variable usages (may be .NET only)
- Host Based or Percentage Based Config. A certain percentage of clients can get a specific setting (this could be used for A/B testing)
- Environment Groups. A set of environment variables tied to an environment set that can be applied to multiple applications. This allows the user to attach commonly re-used variables to an application that may not be used for ALL applications (That is where you would use the environment set method)
- Host Based or Percentage Based Config. A certain percentage of clients can get a specific setting (this could be used for A/B testing)
- Ability to Duplicate Environment Set
- Ability to Duplicate Application
- History screens to have searching/paging
- Attach/Detach application from Environment Set
- Add custom environment to an application in addition of what comes from environment set (this will then require ordering of environments)
 
## TODO
Items that are still pending before this is considered a Beta:

- Forgot Password
- Showing errors to user instead of console
- Internationalization
- Build and deploy 
- Logging

### Environment Sets
- When a new variable is created (it is blank), loose focus, regain focus, and name, the Rename endpoint is being called and not the new endpoint
- Order Environments
- Duplicate Prevention (in progress)
- Rename environment when no apps are associated, should just proceed without the dialog warning

### Applications
- Ability to rename an application
- Versioning / Optomistic Concurrency (in progress)
- Show Applied Config (and when settings are overridden)
- Duplicate Prevention (in progress)
- Regenerate Token
- Filter/Search applications
 
### Users
- All of it

### Settings
- All of it