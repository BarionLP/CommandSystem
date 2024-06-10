using System;
using System.Linq;

namespace Ametrin.Command{
    public static class CommandManager{
        public static readonly CommandGroup Commands = new();
        private static ICommandLogger Logger;

        public static void SetLogger(ICommandLogger logger){
            Logger = logger;
        }

        public static void Log(string message) => Logger.Log(message);
        public static void LogWarning(string message) => Logger.LogWarning(message);
        public static void LogError(string message) => Logger.LogError(message);

        public static void Execute(ReadOnlySpan<char> input){
            var inputParts = input.Split(' ');
            if(inputParts.Count == 0) return;

            var key = input[inputParts[0]];
            if(!Commands.TryGet(key, out var command)){
                LogError($"Command not found {key.ToString()}");
                return;
            }

            command.Execute(input, inputParts.Skip(1));
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