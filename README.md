# Command System
A system for calling funcions via text input 
(made for Unity, can be used outside)

## Usage (Unity)
- (optional) install UpmGitExtension package (https://github.com/mob-sakai/UpmGitExtension.git)
- install Unity Utils package 
- install Command Sytem package https://github.com/BarionLP/CommandSystem.git

### Run Commands
```csharp
CommandManager.Execute("command");
```

### Set Logger
```csharp
CommandManager.SetLogger(new UnityDebugCommandLogger()); //print logs in the unity console

// Custom Logger
public class CustomLogger : ICommandLogger
{
  public void Log(string message) => Debug.Log(message);
  public void LogWarning(string message) => Debug.LogWarning(message);
  public void LogError(string message) => Debug.LogError(message);
}
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

### Advanced Commands
```csharp
//register static methods with the Command attribute
// atomatically generates syntax hints
CommandManager.Commands.Register<TestCommands>();

//register command "cmd" with the methods as subcommands (e.g. "cmd add 4 2")
CommandManager.Commands.RegisterGroup<TestCommands>("cmd");

public class TestCommands
{
  [Command("add")] //automatically parses primitive arguments
  public static void Add(int left, int right) => CommandManager.Log($"{left} + {right} is {left+right}");
        
  [Command("wait")] // supports async
  public static async Task Test(float seconds) => await Task.Delay((int)(seconds*1000));
        
  [Command("give")] //other objects require custom parsers (see below)
  public static void GiveItem(Item item) => Player.Give(item);
}
```

### Parsers
```csharp
//has built in parsers for most primitives (float, int, uint, short, ...) see in `ArgumentParser.cs`

ArgumentParsers.Register<CustomType>(new CustomParser()); // register a custom parser
ArgumentParsers.Register<CustomEnum>(new EnumArgumentParser<CustomEnum>()); //generates an enum parser (parses by name)

public class CustomParser : IArgumentParser<CustomType>
{
  public CustomType Parse(ReadOnlySpan<char> raw) => //however you want, e.g. from a registry (`null` if fails)
  public IEnumerable<string> GetSuggestions() => Enumerable.Empty<string>(); //or all possible values
}

```
