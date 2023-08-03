using System;

namespace Ametrin.Command{
    public interface ICommandArgumentParser{
        public object Parse(string raw);
        public virtual string[] GetSuggestions() => Array.Empty<string>();
    }
}
