## ConfigMan

ConfigMan is a configuration management server designed to have a central location to store configuration. Applications can be configured to load their configuration
on startup and be changed without requiring an application to be redeployed for a config change.

## Features
- Ability to define global deployment environments as well as global settings that all applications can inherit (e.g. you can set a log server for all applications)
- Create variables at the application level that are inherited by each environment
- .NET Configuration Provider
- Audit logs of changes

## Features I am thinking about
- Case Sensitive Config (possibly as a configuration option)
- Per Application Access (read/write/Whatever people want)
- Secret storage
- Variable version pinning
- Live variables changes (May be .NET provider only)
- Tracking of variable usages (may be .NET only)
- Host Based or Percentage Based Config. A certain percentage of clients can get a specific setting (this could be used for A/B testing)


## TODO
- Forgot Password
- Showing errors to user instead of console
- Internationalization
- Build and deploy 

Environment Sets
- When a new variable is created (it is blank), loose focus, regain focus, and name, the Rename endpoint is being called and not the new endpoint
- Order Environments
- Duplicate Prevention (in progress)

- Low Priority: Rename environment when no apps are associated, should just proceed without the dialog warning
- Low Priority: Duplicate Environment Set?
- Low Priority: History screen to implement paging/searching

Applications
- Ability to rename an application
- Versioning / Optomistic Concurrency (in progress)
- Show Applied Config (and when settings are overridden)
- Duplicate Prevention (in progress)
- Regenerate Token

- Low Priority: Host Based or Percentage Based Config. A certain percentage of clients can get a specific setting (this could be used for A/B testing)
- Low Priority: Detach from Environment Set
- Low Priority: Add custom environment in addition of what comes from environment set (this will then require ordering of environments)
- Low Priority: Copy Application
- Low Priority: History screen to implement paging/searching
- Low Priority: Filter/Search applications
 
Users
- All of it

Settings
- All of it