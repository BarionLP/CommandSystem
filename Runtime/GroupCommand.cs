using System;
using System.Collections.Generic;
using System.Linq;

namespace Ametrin.Command{
    public sealed class GroupCommand : ICommand{
        public string Key { get; }
        private readonly CommandGroup _commands = new();

        public GroupCommand(string name){
            Key = name;
        }

        public void Execute(ReadOnlySpan<char> input, IEnumerable<Range> slices){
            if (_commands.TryGet(input[slices.First()], out var command)){
                command.Execute(input, slices.Skip(1));
                return;
            }
            CommandManager.LogError($"Unkown command: {Key} {input[slices.First()].ToString()}");
        }

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace)=> _commands.CompleteNextParameter(input, slices, endsWithSpace);
        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices) => $"{Key} {_commands.GetSyntax(input, slices)}";

        public bool Register(ICommand command) => _commands.Register(command);
        public void Register<T>() => _commands.Register<T>();
    }    
}
