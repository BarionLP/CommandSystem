using System;

namespace Ametrin.Command{
    public interface ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw);
        public virtual string[] GetSuggestions() => Array.Empty<string>();
    }

    public sealed class StringArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            return raw.ToString();
        }
    }
    
    public sealed class ShortArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(short.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class UShortArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(ushort.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class IntArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(int.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class UIntArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(uint.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class FloatArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(float.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class LongArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(long.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class ULongArgumentParser : ICommandArgumentParser{
        public object Parse(ReadOnlySpan<char> raw){
            if(ulong.TryParse(raw, out var result)) return result;
            return null;
        }
    }
}
