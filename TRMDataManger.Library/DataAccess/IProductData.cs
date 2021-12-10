using System.Collections.Generic;
using TRMDataManger.Library.Models;

namespace TRMDataManger.Library.DataAccess
{
    public interface IProductData
    {
        ProductModel GetProductById(int productId);
        List<ProductModel> GetProducts();
    }
}