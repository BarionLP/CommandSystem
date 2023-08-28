using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Ametrin.Utils;

namespace Ametrin.Command
{
    public static class CommandManager{
        private static readonly Dictionary<string, ICommand> Commands = new();
        private static ICommandLogger Logger;

        public static void SetLoggger(ICommandLogger logger){
            Logger = logger;
        }

        public static void RegisterCommands<T>(){
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods){
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute is null) continue;
                var command = MethodCommand.Of(method, attribute.Name);
                Commands[command.Key] = command;
            }
        }

        public static void Log(string message) => Logger.Log(message);
        public static void LogWarning(string message) => Logger.LogWarning(message);
        public static void LogError(string message) => Logger.LogError(message);

        public static void Execute(ReadOnlySpan<char> input){
            var inputParts = input.Split(' ');
            if(inputParts.Count == 0) return;

            var commandName = input[inputParts[0]];
            if(GetCommand(commandName).HasFailed(out var command)){
                LogError($"Command not found: {commandName.ToString()}");
                return;
            }

            command.Execute(input, inputParts.Skip(1));
        }

        public static string GetFirstSyntax()=> Commands.Values.First().GetSyntax();
        public static string GetFirstCommand()=> Commands.Keys.First();

        public static string GetSyntax(ReadOnlySpan<char> key){          
            foreach(var command in Commands){
                if(command.Key.StartsWith(key)) return command.Value.GetSyntax();
            }
            
            return null;
        }
        
        public static string GetFirstCommand(ReadOnlySpan<char> filter){          
            if(filter.IsEmpty) return Commands.First().Value.GetSyntax();
            foreach(var command in Commands){
                if(command.Key.StartsWith(filter)) return command.Key;
            }
            
            return null;
        }

        public static Result<ICommand> GetCommand(ReadOnlySpan<char> key){
            if(Commands.TryGetValue(key, out var command)) return Result<ICommand>.Succeeded(command);
            return ResultStatus.Null;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute{
        public readonly string Name;

        public CommandAttribute() : this(null) { }

        public CommandAttribute(string name){
            Name = name;
        }
    }
}