using System;
using System.Collections.Generic;
using System.Linq;

namespace Ametrin.Command{
    public static class ArgumentParsers{
        private readonly static Dictionary<Type, IArgumentParser> Parsers = new();

        static ArgumentParsers(){
            Register<string>(new StringArgumentParser());
            Register<short>(new ShortArgumentParser());
            Register<ushort>(new UShortArgumentParser());
            Register<int>(new IntArgumentParser());
            Register<uint>(new UIntArgumentParser());
            Register<float>(new FloatArgumentParser());
            Register<long>(new LongArgumentParser());
            Register<ulong>(new ULongArgumentParser());
        }

        public static bool Register<T>(IArgumentParser parser){
            return Parsers.TryAdd(typeof(T), parser);
        }

        public static IArgumentParser Get<T>() => Get(typeof(T));
        public static IArgumentParser Get(Type t){
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (Parsers.TryGetValue(t, out var parser)) return parser;

            throw new NullReferenceException($"No parser registered for {t.Name}");
        }

    }

    public interface IArgumentParser{
        public object Parse(ReadOnlySpan<char> raw);
        public IEnumerable<string> GetSuggestions();
    }

    public abstract class PrimitiveArgumentParser : IArgumentParser{
        public abstract object Parse(ReadOnlySpan<char> raw);
        public IEnumerable<string> GetSuggestions() => Enumerable.Empty<string>();
    }

    public sealed class StringArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            return raw.ToString();
        }
    }
    
    public sealed class ShortArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(short.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class UShortArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(ushort.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class IntArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(int.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class UIntArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(uint.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class FloatArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(float.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class LongArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(long.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class ULongArgumentParser : PrimitiveArgumentParser{
        public override object Parse(ReadOnlySpan<char> raw){
            if(ulong.TryParse(raw, out var result)) return result;
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ParserAttribute : Attribute{
        public readonly IArgumentParser Parser;
        public ParserAttribute(IArgumentParser parser){
            Parser = parser;
        }
    }
}
