using System;
using System.Collections.Generic;
using System.Linq;
using Ametrin.Registry;

#nullable enable
namespace Ametrin.Command{
    public static class ArgumentParsers{
        private readonly static Dictionary<Type, IArgumentParser> Parsers = new();

        static ArgumentParsers(){
            Register(new StringArgumentParser());
            Register(new ShortArgumentParser());
            Register(new UShortArgumentParser());
            Register(new IntArgumentParser());
            Register(new UIntArgumentParser());
            Register(new FloatArgumentParser());
            Register(new LongArgumentParser());
            Register(new ULongArgumentParser());
        }

        public static bool Register<T, TParser>() where TParser : IArgumentParser, new() => Register<T>(new TParser());
        public static bool Register<T>(IArgumentParser parser) => Parsers.TryAdd(typeof(T), parser);
        
        public static bool Register<T>(IArgumentParser<T> parser){
            var t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;
            return Parsers.TryAdd(t, parser);
        }

        public static IArgumentParser<T> Get<T>(){
            var parser = Get(typeof(T));
            if (parser is IArgumentParser<T> real) return real;
            throw new ArgumentException($"ArgumentParser<{typeof(T).Name}> required");
        }
        
        public static IArgumentParser Get(Type t){
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (Parsers.TryGetValue(t, out var parser)) return parser;

            throw new NullReferenceException($"No parser registered for {t.Name}");
        }
    }

    public interface IArgumentParser{
        public object? Parse(ReadOnlySpan<char> raw);
        public IEnumerable<string> GetSuggestions();
    }
    
    public interface IArgumentParser<T> : IArgumentParser{
        public new T? Parse(ReadOnlySpan<char> raw);
        object? IArgumentParser.Parse(ReadOnlySpan<char> raw) => Parse(raw);
    }

    public sealed class EnumArgumentParser<TEnum> : IArgumentParser where TEnum : Enum{
        private readonly EnumRegistry<TEnum> Registry = new();
        public object? Parse(ReadOnlySpan<char> raw) => Registry.TryGet(raw).TryResolve(out var result) ? result : null;
        public IEnumerable<string> GetSuggestions() => Registry.Keys;
    }

    public interface IPrimitiveArgumentParser<T> : IArgumentParser<T>{
        IEnumerable<string> IArgumentParser.GetSuggestions() => Enumerable.Empty<string>();
    }

    public sealed class StringArgumentParser : IPrimitiveArgumentParser<string>{
        public string? Parse(ReadOnlySpan<char> raw){
            if(raw.IsEmpty) return null;
            return raw.ToString();
        }
    }
    
    public sealed class ShortArgumentParser : IPrimitiveArgumentParser<short?>{
        public short? Parse(ReadOnlySpan<char> raw){
            if(short.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class UShortArgumentParser : IPrimitiveArgumentParser<ushort?>{
        public ushort? Parse(ReadOnlySpan<char> raw){
            if(ushort.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class IntArgumentParser : IPrimitiveArgumentParser<int?>{
        public int? Parse(ReadOnlySpan<char> raw){
            if(int.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class UIntArgumentParser : IPrimitiveArgumentParser<uint?>{
        public uint? Parse(ReadOnlySpan<char> raw){
            if(uint.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class FloatArgumentParser : IPrimitiveArgumentParser<float?>{
        public float? Parse(ReadOnlySpan<char> raw){
            if(float.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    
    public sealed class LongArgumentParser : IPrimitiveArgumentParser<long?>{
        public long? Parse(ReadOnlySpan<char> raw){
            if(long.TryParse(raw, out var result)) return result;
            return null;
        }
    }
    public sealed class ULongArgumentParser : IPrimitiveArgumentParser<ulong?>{
        public ulong? Parse(ReadOnlySpan<char> raw){
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
