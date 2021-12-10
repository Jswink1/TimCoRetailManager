using System.Collections.Generic;
using TRMDataManger.Library.Models;

namespace TRMDataManger.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryModel> GetInventory();
        void SaveInventoryRecord(InventoryModel item);
    }
}