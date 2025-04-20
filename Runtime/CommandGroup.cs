using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ametrin.Command
{
    public sealed class CommandGroup : IEnumerable<KeyValuePair<string, ICommand>>
    {
        private readonly Dictionary<string, ICommand> _registry = new();

        public ICommand this[string key]
        {
            get => _registry[key];
            set
            {
                _registry[key] = value;
            }
        }

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace)
        {
            if (slices.Count == 0) return _registry.Keys.First();
            var key = input[slices.First()];
            if (slices.Count == 1 && !endsWithSpace)
            {
                foreach (var item in _registry.Keys)
                {
                    if (item.StartsWith(key)) return item;
                }
                return string.Empty;
            }

            if (!_registry.TryGetValue(key.ToString(), out var command)) return string.Empty;
            slices.RemoveAt(0);
            return command.CompleteNextParameter(input, slices, endsWithSpace);
        }

        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices)
        {
            if (slices.Count == 0) return _registry.First().Value.GetSyntax(input, slices);
            var key = input[slices.First()];
            slices.RemoveAt(0);
            foreach (var item in _registry)
            {
                if (item.Key.StartsWith(key)) return item.Value.GetSyntax(input, slices);
            }
            return string.Empty;
        }

        public bool TryGet(ReadOnlySpan<char> key, out ICommand command) => TryGet(key.ToString(), out command);
        public bool TryGet(string key, out ICommand command) => _registry.TryGetValue(key, out command);

        public bool Register(ICommand command) => _registry.TryAdd(command.Key, command);
        public bool RegisterGroup(string name, IEnumerable<ICommand> commands)
        {
            var group = new GroupCommand(name);
            foreach (var command in commands)
            {
                group.Register(command);
            }
            return Register(group);
        }

        public void Register<T>()
        {
            foreach (var command in EnumerateCommands<T>())
            {
                Register(command);
            }
        }

        public bool RegisterGroup<T>(string name) => RegisterGroup(name, EnumerateCommands<T>());

        private IEnumerable<ICommand> EnumerateCommands<T>()
        {
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute is null) continue;
                yield return MethodCommand.Of(method, attribute.Name);
            }
        }

        public IEnumerator<KeyValuePair<string, ICommand>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, ICommand>>)_registry).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_registry).GetEnumerator();
    }
}
