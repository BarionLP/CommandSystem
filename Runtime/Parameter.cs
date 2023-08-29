using System;
using System.Reflection;

namespace Ametrin.Command{
    public interface IParameter{
        public string Name {get;}
        public IArgumentParser Parser {get;}
        public object DefaultValue {get;}
        public bool HasDefaultValue {get;}
    }
    public interface IParameter<T> : IParameter{
        public new T DefaultValue {get;}
        public new IArgumentParser<T> Parser { get; }
        object IParameter.DefaultValue => DefaultValue;
        IArgumentParser IParameter.Parser => Parser; 
    }
    
    public record Parameter : IParameter{
        public string Name { get; }
        public IArgumentParser Parser { get; }
        public object DefaultValue { get; }
        public bool HasDefaultValue { get; }

        private Parameter(string name, IArgumentParser parser, bool hasDefault, object defaultValue){
            Name = name;
            Parser = parser;
            HasDefaultValue = hasDefault;
            DefaultValue = defaultValue;
        }
        public Parameter(string name, IArgumentParser parser) : this(name, parser, false, null){}
        public Parameter(string name, IArgumentParser parser, object defaultValue) : this(name, parser, true, defaultValue){}

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

    public record Parameter<T> : IParameter<T>{
        public string Name { get; }
        public IArgumentParser<T> Parser { get; }
        public T DefaultValue { get; }
        public bool HasDefaultValue { get; }

        private Parameter(string name, IArgumentParser<T> parser, bool hasDefault, T defaultValue){
            Name = name;
            Parser = parser;
            HasDefaultValue = hasDefault;
            DefaultValue = defaultValue;
        }
        public Parameter(string name, IArgumentParser<T> parser) : this(name, parser, false, default) { }
        public Parameter(string name, IArgumentParser<T> parser, T defaultValue) : this(name, parser, true, defaultValue) { }


        public static IParameter<T> Of(string name) => new Parameter<T>(name, ArgumentParsers.Get<T>());
        public static IParameter<T> Of(string name, T defaultValue) => new Parameter<T>(name, ArgumentParsers.Get<T>(), defaultValue);

        public sealed class Builder{
            private readonly string Name;
            private T DefaultValue = default;
            private bool HasDefaultValue = false;

            public Builder(string name){
                Name = name;
            }

            public Builder Default(T defaultValue){
                HasDefaultValue = true;
                DefaultValue = defaultValue;
                return this;
            }

            public IParameter Build() => Build(ArgumentParsers.Get<T>());
            public IParameter<T> Build(IArgumentParser<T> parser){
                return new Parameter<T>(Name, parser, HasDefaultValue, DefaultValue);
            }
        }
    }
}