using System.Threading.Tasks;
using UnityEngine;


namespace Ametrin.Command.Demo{
    public sealed class TestCommands{
        static TestCommands{
            ArgumentParsers.Register<ItemType>(new EnumArgumentParser<ItemType>()); //simple enum parser (by name)
            ArgumentParsers.Register(new ItemCommandArgumentParser()); //register custom parser
            CommandManager.Commands.Register<TestCommands>(); //register commands (must be static, can be private)
        }

        [Command("add")] //automatically parses primitives
        public static void Add(int left, int right){
            CommandManager.Log($"{left} + {right} is {left+right}");
        }
        
        [Command("wait")] // supports async
        public static async Task Test(float seconds){
            await Task.Delay((int)(seconds*1000));
            CommandManager.Log($"Waited for {seconds}s");
        }
        
        [Command("name")] //other objects requires custom parsers
        public static void ItemName(Item item){
            CommandManager.Log($"Item is called {item.name}");
        }
    }
}