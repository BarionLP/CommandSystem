using System.Collections.Generic;
using UnityEngine;


namespace Ametrin.Command.Demo{
    // Custom Parser
    public sealed class ItemCommandArgumentParser : IArgumentParser<Item>{
        public Item Parse(ReadOnlySpan<char> raw) => Item.Registry[raw.ToString()];
        public IEnumerable<string> GetSuggestions() => Item.Registry.Keys; //Optional
    }

    [CreateAssetMenu]
    public sealed class Item : ScriptableObject{
        public static readonly Dictionary<string, Item> Registry = new();
    }
    public enum ItemType { Weapon, Food, Armor }
}
