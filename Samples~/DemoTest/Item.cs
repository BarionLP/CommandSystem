using System.Collections.Generic;
using UnityEngine;


namespace Ametrin.Command.Demo{
    
    [CreateAssetMenu]
    public sealed class Item : ScriptableObject{
        public static readonly Dictionary<string, Item> Registry = new();
    }

    public sealed class ItemCommandArgumentParser : ICommandArgumentParser{
        public object Parse(string raw) => Item.Registry[raw];
    }
}
