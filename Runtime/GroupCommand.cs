using System;
using System.Collections.Generic;
using System.Linq;
using Ametrin.Utils.Optional;

namespace Ametrin.Command{
    public sealed class GroupCommand : ICommand{
        public string Key { get; }
        private readonly CommandGroup Group = new();

        public GroupCommand(string name){
            Key = name;
        }

        public void Execute(ReadOnlySpan<char> input, IEnumerable<Range> slices){
            if (Group.TryGet(input[slices.First()]).TryResolve(out var command)){
                command.Execute(input, slices.Skip(1));
                return;
            }
            CommandManager.LogError($"Unkown command {Key} {input[slices.First()].ToString()}");
        }

        public string CompleteNextParameter(ReadOnlySpan<char> input, IList<Range> slices, bool endsWithSpace)=> Group.CompleteNextParameter(input, slices, endsWithSpace);
        public string GetSyntax(ReadOnlySpan<char> input, IList<Range> slices) => $"{Key} {Group.GetSyntax(input, slices)}";

        public ResultFlag Register(ICommand command) => Group.Register(command);
        public void Register<T>() => Group.Register<T>();
    }    
}
