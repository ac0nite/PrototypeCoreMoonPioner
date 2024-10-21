using System.Linq;
using Cysharp.Threading.Tasks;

namespace Gameplay.Warehouses
{
    public class TransferResources
    {
        public static async UniTask TransferTo(IWarehouse source, ResourceType type, int quantity, IWarehouse target)
        {
            var sourceStorage = source.GetStorage(type);
            var targetStorage = target.GetStorage(type);
            
            for (int i = 0; i < quantity; i++)
            {
                var item = sourceStorage.RemoveResource(1);
                targetStorage.AddResource(item);
                await item.Collections.First().Animation.PlayJumpTask(targetStorage.GeеFreePointPlacement());
            }
        }
    }
}