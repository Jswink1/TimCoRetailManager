using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Models;
using TRMDataManger.Library.Internal.DataAccess;
using TRMDataManger.Library.Models;

namespace TRMDataManger.Library.DataAccess
{
    public class SaleData
    {
        private readonly IConfiguration _config;

        public SaleData(IConfiguration config)
        {
            _config = config;
        }

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData(_config);
            var taxRate = ConfigHelper.GetTaxRate()/100;

            foreach (var item in saleInfo.SaleDetails)
            {
                // Initialize a SaleDetailModel to insert into the DB
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // Load product information
                var productInfo = products.GetProductById(detail.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");
                }

                // Calculate SaleDetail Total and Tax
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate;
                }

                details.Add(detail);
            }

            // Initialize a SaleModel to insert into the DB
            SaleDBModel sale = new SaleDBModel
            { 
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };
            sale.Total = sale.SubTotal + sale.Tax;

            // Use a SQL Transaction to insert the Sale and SaleDetails into the DB
            using (SqlDataAccess sql = new SqlDataAccess(_config))
            {
                try
                {
                    sql.StartTransaction("TimCoRetailDB");

                    // Insert the Sale
                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    // Retrieve the ID of the inserted Sale
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup",
                                                                      new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                    // Insert each SaleDetail of the Sale into the DB
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;

                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert",
                                                  item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }            
        }

        public List<SaleReportModel> GetSaleReport()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TimCoRetailDB");

            return output;
        }
    }
}
