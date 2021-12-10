using System.Collections.Generic;
using TRMDataManager.Library.Models;
using TRMDataManger.Library.Models;

namespace TRMDataManger.Library.DataAccess
{
    public interface ISaleData
    {
        List<SaleReportModel> GetSaleReport();
        void SaveSale(SaleModel saleInfo, string cashierId);
    }
}