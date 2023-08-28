using System;
using System.Reflection;

namespace Ametrin.Command{
    public record Parameter{
        public readonly string Name;
        public readonly object DefaultValue = default;
        public readonly bool HasDefaultValue = false;
        public readonly IArgumentParser Parser;

        public Parameter(string name, IArgumentParser parser){
            Name = name;
            Parser = parser;
        }

        public Parameter(string name, IArgumentParser parser, object defaultValue) : this(name, parser){
            DefaultValue = defaultValue;
            HasDefaultValue = true;
        }
        
        public Parameter(string name, Type type) : this(name, ArgumentParsers.Get(type)) {}
        public Parameter(string name, Type type, object defaultValue) : this(name, ArgumentParsers.Get(type), defaultValue){}

        public static Parameter Of<T>(string name) => new(name, typeof(T));
        public static Parameter Of<T>(string name, T defaultValue) => new(name, typeof(T), defaultValue);
        public static Parameter Of(ParameterInfo info){
            var attribute = info.GetCustomAttribute<ParserAttribute>();
            return attribute is null
                ? info.HasDefaultValue
                    ? new Parameter(info.Name, info.ParameterType, info.DefaultValue)
                    : new Parameter(info.Name, info.ParameterType)
                : info.HasDefaultValue
                    ? new Parameter(info.Name, attribute.Parser, info.DefaultValue)
                    : new Parameter(info.Name, attribute.Parser);
        }
    }
}