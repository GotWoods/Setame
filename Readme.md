## ConfigMan

ConfigMan is a configuration management server designed to have a central location to store configuration. Applications can be configured to load their configuration
on startup and be changed without requiring an application to be redeployed for a config change.

## Features
- Ability to define global environments sets 
- Ability to set global variables per environment that every application can inherit
- Create variables at the application level that are inherited by each environment
- .NET Configuration Provider

- Case Sensitive Config?
- History of changes to variables?
- Per Application Access (read/write/Whatever people want)?
- Secret storage?
- Variable version pinning?
- Live variables changes?
- Permission per application can be granted?
- Tracking of variable usages (may be .NET only)



## TODO
- Setup Screen
- Forgot Password

Environment Sets
- Delete Environment (will cascase to all children apps?)
- Rename Environment (will cascade to all children apps?)
- New Variable -> Ability to add values to it
- Rename Variable on set
- Duplicate Prevention
- History
- Copy Environment Set

Applications
- Detach from Environment Set
- Add custom environment above environment set
- Applied Config
- Globals not working
- Duplicate Prevention
- History
- Copy Application

Users
- All of it

Settings
- All of it