## ConfigMan

ConfigMan is a configuration management server designed to have a central location to store configuration. Applications can be configured to load their configuration
on startup and be changed without requiring an application to be redeployed for a config change.

## Features
- Ability to define global environments sets 
- Ability to set global variables per environment that every application can inherit
- Create variables at the application level that are inherited by each environment
- .NET Configuration Provider
- Audit logs of variable changes

- Case Sensitive Config?
- Per Application Access (read/write/Whatever people want)?
- Secret storage?
- Variable version pinning?
- Live variables changes?
- Tracking of variable usages (may be .NET only)
- Host Based or Percentage Based Config. A certain percentage of clients can get a specific setting (this could be used for A/B testing)


## TODO
- Setup Screen
- Forgot Password
- Optomistic Concurrency
- Showing errors to user instead of console
- Internationalization

Environment Sets
- Grid fixes
- Delete Environment (will cascase to all children apps?)
- Rename Environment (will cascade to all children apps?)
- Order Environments (drag/drop would be nice)
- Duplicate Prevention (in progress)
- History (partially completed)
- Versioning (in progress)
- You should not be able to add an Environment Set variable if a child application is already using that name (or it can prompt you if that is the case)
- Duplicate Environment Set?

Applications
- Remove summary screen
- Detach from Environment Set
- Add custom environment above environment set
- Order Environments
- Applied Config 
- Duplicate Prevention (in progress)
- History (partially completed)
- Copy Application
- Host Based or Percentage Based Config. A certain percentage of clients can get a specific setting (this could be used for A/B testing)
- Versioning (in progress)
- Regenerate Token
- Hide EnvironmentSet data when there are no values

Users
- All of it

Settings
- All of it