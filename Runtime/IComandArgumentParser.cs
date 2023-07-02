using System;

namespace Ametrin.Command{
    public interface ICommandArgumentParser{
        public object Parse(string raw);
        public string[] GetSuggestions() => Array.Empty<string>();
    }
}
