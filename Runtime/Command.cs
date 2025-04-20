using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ametrin.Command
{

    public interface ICommand
    {
        public string Key { get; }
        public void Execute(ReadOnlySpan<char> input, IEnumerable<Range> slices);
        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace);
        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices);
    }

    public sealed class MethodCommand : ICommand
    {
        public string Key { get; }
        private MethodInfo MethodInfo;
        private Parameter[] Parameters;
        private string _SyntaxCache = null;

        private MethodCommand(string key)
        {
            Key = key;
        }

        public void Execute(ReadOnlySpan<char> input, IEnumerable<Range> slices)
        {
            var args = new object[Parameters.Length];

            var idx = 0;
            foreach (var slice in slices)
            {
                var arg = Parameters[idx].Parser.Parse(input[slice]);
                if (arg is null)
                {
                    CommandManager.LogError($"Failed parsing {input[slice].ToString()} as {Parameters[idx].Name}");
                    return;
                }

                args[idx] = arg;
                idx++;
            }

            while (idx < args.Length)
            {
                if (!Parameters[idx].HasDefaultValue)
                {
                    CommandManager.LogError($"Missing parameter {Parameters[idx].Name}");
                    return;
                }
                args[idx] = Parameters[idx].DefaultValue;
                idx++;
            }

            MethodInfo.Invoke(null, args);
        }

        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices)
        {
            _SyntaxCache ??= GenerateSytanx();
            return _SyntaxCache;
        }

        public static ICommand Of(MethodInfo method, string keyOverride = null)
        {
            var parameters = method.GetParameters();
            var key = keyOverride ?? method.Name.ToLower();

            if (parameters.Length == 0) return new SimpleCommand(key, () => method.Invoke(null, Array.Empty<object>()));

            var command = new MethodCommand(key)
            {
                Parameters = new Parameter[parameters.Length],
                MethodInfo = method,
            };

            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters[i] = Parameter.Of(parameters[i]);
            }

            return command;
        }

        private string GenerateSytanx()
        {
            var builder = new StringBuilder(Key);

            foreach (var parameter in Parameters)
            {
                if (parameter.HasDefaultValue)
                {
                    builder.AppendFormat(" [<{0}>({1})]", parameter.Name, parameter.DefaultValue ?? "null");
                }
                else
                {
                    builder.AppendFormat(" <{0}>", parameter.Name);
                }
            }

            return builder.ToString();
        }

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace)
        {
            var paramIdx = endsWithSpace ? slices.Count : slices.Count - 1;
            if (paramIdx < 0 || paramIdx >= Parameters.Length) return string.Empty;
            var suggestions = Parameters[paramIdx].Parser.GetSuggestions();
            if (!suggestions.Any()) return string.Empty;
            if (endsWithSpace)
            {
                return suggestions.First();
            }
            else
            {
                var filter = input[slices[paramIdx]]; // needs to iterate -> BAD
                foreach (var suggestion in suggestions)
                {
                    if (suggestion.StartsWith(filter)) return suggestion;
                }
            }

            return string.Empty;
        }
    }

    public sealed class SimpleCommand : ICommand
    {
        public string Key { get; }
        private readonly Action Action;

        public SimpleCommand(string key, Action action)
        {
            Key = key;
            Action = action;
        }

        public void Execute(ReadOnlySpan<char> args, IEnumerable<Range> slices) => Action();

        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices) => Key;

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace) => string.Empty;
    }

    public sealed class SimpleCommand<T> : ICommand
    {
        public string Key { get; }
        private readonly Action<T> Action;
        private readonly IParameter<T> Parameter;
        private readonly string Syntax;

        public SimpleCommand(string key, Action<T> action, IParameter<T> parameter)
        {
            Key = key;
            Action = action;
            Parameter = parameter;
            Syntax = Parameter.HasDefaultValue ? $"{Key} [<{Parameter.Name}>({Parameter.DefaultValue})]" : $"{Key} <{Parameter.Name}>";
        }


        public void Execute(ReadOnlySpan<char> args, IEnumerable<Range> slices)
        {
            if (!slices.Any())
            {
                CommandManager.LogError($"Missing argument");
                return;
            }

            Action(Parameter.Parser.Parse(args[slices.First()]));
        }

        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices) => Syntax;

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace)
        {
            if (slices.Count > 1 || (endsWithSpace && slices.Count == 1)) return string.Empty;

            var suggestions = Parameter.Parser.GetSuggestions();
            if (endsWithSpace && slices.Count == 0) return suggestions.First();
            foreach (var suggestion in suggestions)
            {
                if (suggestion.StartsWith(input[slices[0]])) return suggestion;
            }

            return string.Empty;
        }
    }
}
