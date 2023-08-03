using System.Threading.Tasks;
using UnityEngine;


namespace Ametrin.Command.Demo{
    public sealed class TestCommands{
        static TestCommands{
            CommandManager.RegisterArgumentParser<Item>(new ItemCommandArgumentParser());
            CommandManager.RegisterCommands<TestCommands>();
        }

        [Command("add")]
        public static void Add(int left, int right){
            CommandManager.Log($"{left} + {right} is {left+right}");
        }
        
        [Command("wait")]
        public static async Task Test(float seconds){
            await Task.Delay((int)(seconds*1000));
            CommandManager.Log($"Waited for {seconds}s");
        }
        
        [Command("name")]
        public static void ItemName(Item item){
            CommandManager.Log($"Item is called {item.name}");
        }
    }
}