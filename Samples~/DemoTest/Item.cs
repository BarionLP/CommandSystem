using Ametrin.AutoRegistry;
using UnityEngine;


namespace Ametrin.Command.Demo{
    
    [CreateAssetMenu]
    public sealed class Item : ScriptableObject{
        public static readonly ScriptableObjectRegistry<string, Item> Registry = new(item => item.name);
    }

    public sealed class ItemCommandArgumentParser : ICommandArgumentParser{
        public object Parse(string raw) => Item.Registry.TryGet(raw).Get();
    }
}
