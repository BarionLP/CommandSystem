using System;
using System.Reflection;

namespace Ametrin.Command{
    public record Parameter{
        public readonly string Name;
        public readonly IArgumentParser Parser;
        public readonly object DefaultValue = default;
        public readonly bool HasDefaultValue = false;

        private Parameter(string name, IArgumentParser parser, bool hasDefault, object defaultValue){
            Name = name;
            Parser = parser;
            HasDefaultValue = hasDefault;
            DefaultValue = defaultValue;
        }
        public Parameter(string name, IArgumentParser parser) : this(name, parser, false, null){}
        public Parameter(string name, IArgumentParser parser, object defaultValue) : this(name, parser, true, defaultValue){}


        public static Parameter Of<T>(string name) => new(name, ArgumentParsers.Get(typeof(T)));
        public static Parameter Of<T>(string name, T defaultValue) => new(name, ArgumentParsers.Get(typeof(T)), defaultValue);
        public static Parameter Of(ParameterInfo info){
            var attribute = info.GetCustomAttribute<ParserAttribute>();
            var builder = new Builder(info.Name);
            if(info.HasDefaultValue){
                builder.Default(info.DefaultValue);
            }

            return attribute is null ? builder.Build(info.ParameterType) : builder.Build(attribute.Parser);
        }

        public sealed class Builder{
            private readonly string Name;
            private object DefaultValue = default;
            private bool HasDefaultValue = false;

            public Builder(string name){
                Name = name;
            }

            public Builder Default(object defaultValue){
                HasDefaultValue = true;
                DefaultValue = defaultValue;
                return this;
            }

            public Parameter Build(Type t) => Build(ArgumentParsers.Get(t));
            public Parameter Build<T>() => Build(typeof(T));
            public Parameter Build(IArgumentParser parser){
                return new Parameter(Name, parser, HasDefaultValue, DefaultValue);
            }
        } 
    }
}