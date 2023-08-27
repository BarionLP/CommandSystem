using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ametrin.Utils;

namespace Ametrin.Command{
    public static class CommandManager{
        private static readonly Dictionary<string, (MethodInfo info, string syntax)> Commands = new();
        private static readonly Dictionary<Type, ICommandArgumentParser> ArgumentParsers = new();
        private static ICommandLogger Logger;
        
        static CommandManager(){
            RegisterArgumentParser<string>(new StringArgumentParser());
            RegisterArgumentParser<short>(new ShortArgumentParser());
            RegisterArgumentParser<ushort>(new UShortArgumentParser());
            RegisterArgumentParser<int>(new IntArgumentParser());
            RegisterArgumentParser<uint>(new UIntArgumentParser());
            RegisterArgumentParser<float>(new FloatArgumentParser());
            RegisterArgumentParser<long>(new LongArgumentParser());
            RegisterArgumentParser<ulong>(new ULongArgumentParser());
        }

        public static void OverrideLogger(ICommandLogger logger){
            Logger = logger;
        }
        public static void RegisterArgumentParser<T>(ICommandArgumentParser argumentParser){
            ArgumentParsers[typeof(T)] = argumentParser;
        }

        public static void RegisterCommands<T>(){
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var method in methods){
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute is null) continue;

                var commandName = attribute.Name ?? method.Name.ToLower();
                Commands[commandName] = (method, GenerateCommandSytanx(commandName, method));
            }
        }

        public static void Log(string message) => Logger.Log(message);
        public static void LogWarning(string message) => Logger.LogWarning(message);
        public static void LogError(string message) => Logger.LogError(message);

        public static void Execute(ReadOnlySpan<char> input){
            var inputParts = input.Split(' ');
            if(inputParts.Count == 0) return;

            var commandName = input[inputParts[0]];
            if (!Commands.TryGetValue(commandName, out var command)){
                LogError($"Command not found: {commandName.ToString()}");
                return;
            }

            var parameters = command.info.GetParameters();
            var args = new object[parameters.Length];

            if(parameters.Length < inputParts.Count - 1) LogError($"Too many arguments: expected {parameters.Length} got {inputParts.Count - 1}");

            for(var i = 0; i < parameters.Length; i++){
                var parameter = parameters[i];

                object arg = null;

                if(i + 1 < inputParts.Count){
                    arg = ConvertArgument(input[inputParts[i + 1]], parameter.ParameterType);
                }

                if(arg is null){
                    if (!parameter.HasDefaultValue){
                        LogError($"Missing or invalid argument: {parameter.Name}");
                        return;
                    }
                    arg = parameter.DefaultValue;
                }

                args[i] = arg;
            }

            command.info.Invoke(null, args);
        }

        private static object ConvertArgument(ReadOnlySpan<char> argValue, Type targetType){
            if(ArgumentParsers.TryGetValue(targetType, out var argumentParser)){
                return argumentParser.Parse(argValue);
            }

            return null;
        }

        public static string GetFirstSyntax()=> Commands.Values.First().syntax;

        public static string GetSyntax(ReadOnlySpan<char> commandKey){          
            foreach(var command in Commands){
                if(command.Key.StartsWith(commandKey)) return command.Value.syntax;
            }
            
            return null;
        }
        
        public static string GetFirstCommand(ReadOnlySpan<char> start){          
            foreach(var command in Commands){
                if(command.Key.StartsWith(start)) return command.Key;
            }
            
            return null;
        }

        private static string GenerateCommandSytanx(string key, MethodInfo method){   
            var builder = new StringBuilder(key);
            
            foreach(var parameter in method.GetParameters()){
                if(parameter.HasDefaultValue){
                    builder.AppendFormat(" [<{0}>]", parameter.Name);
                }else{
                    builder.AppendFormat(" <{0}>", parameter.Name);
                }
            }

            return builder.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute{
        public string Name { get; set; }

        public CommandAttribute() { }

        public CommandAttribute(string name){
            Name = name;
        }
    }
}