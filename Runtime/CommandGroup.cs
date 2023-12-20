using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ametrin.Utils;
using Ametrin.Utils.Optional;
using Ametrin.Utils.Registry;

namespace Ametrin.Command{
    public sealed class CommandGroup : IEnumerable<KeyValuePair<string, ICommand>>{
        private readonly MutableRegistry<string, ICommand> Registry = new();

        public ICommand this[string key]{
            get => Registry[key];
            set{
                Registry[key] = value;
            }
        }

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace){
            if (slices.Count == 0) return Registry.Keys.First();
            var key = input[slices.First()];
            if (slices.Count == 1 && !endsWithSpace){
                foreach (var item in Registry.Keys){
                    if (item.StartsWith(key)) return item;
                }
                return string.Empty;
            }

            if (Registry.TryGet(key).ReduceOrNull() is not ICommand command) return string.Empty;
            slices.RemoveAt(0);
            return command.CompleteNextParameter(input, slices, endsWithSpace);
        }

        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices){
            if(slices.Count == 0) return Registry.First().GetSyntax(input, slices);
            var key = input[slices.First()];
            slices.RemoveAt(0);
            foreach (var item in Registry){
                if(item.Key.StartsWith(key)) return item.GetSyntax(input, slices);
            }
            return string.Empty;
        }

        public Option<ICommand> TryGet(ReadOnlySpan<char> key) => Registry.TryGet(key);
        public Option<ICommand> TryGet(string key) => Registry.TryGet(key);

        public ResultFlag Register(ICommand command) => Registry.TryRegister(command.Key, command);
        public ResultFlag RegisterGroup(string name, IEnumerable<ICommand> commands){
            var group = new GroupCommand(name);
            foreach(var command in commands){
                group.Register(command);
            }
            return Register(group);
        }

        public void Register<T>(){
            foreach (var command in EnumerateCommands<T>()){
                Register(command);
            }
        }
        
        public ResultFlag RegisterGroup<T>(string name) => RegisterGroup(name, EnumerateCommands<T>());
        
        private IEnumerable<ICommand> EnumerateCommands<T>(){
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods){
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute is null) continue;
                yield return MethodCommand.Of(method, attribute.Name);
            }
        }

        public IEnumerator<KeyValuePair<string, ICommand>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, ICommand>>)Registry).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Registry).GetEnumerator();
    }
}
