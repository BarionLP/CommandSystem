# Command System
A system for calling funcions via text input 
(made for Unity, can be used outside)

## Usage (Unity)
- (optional) install UpmGitExtension package (https://github.com/mob-sakai/UpmGitExtension.git)
- install Command Sytem package https://github.com/BarionLP/CommandSystem.git

### Run Commands
```csharp
CommandManager.Execute("command");
```

### Simple Commands
```csharp
CommandManager.Commands.Register(new SimpleCommand("test", ()->{
  //Do something

  //Log
  CommandManager.Log();
  CommandManager.LogWarning();
  CommandManager.LogError();
}));
```

### Set Logger
```csharp
CommandManager.SetLogger(new UnityDebugCommandLogger()); //print logs in the unity console

// Custom Logger
public class CustomLogger : ICommandLogger{
  public void Log(string message) => Debug.Log(message);
  public void LogWarning(string message) => Debug.LogWarning(message);
  public void LogError(string message) => Debug.LogError(message);
}
```

### Create Custom Commands
```csharp

```
