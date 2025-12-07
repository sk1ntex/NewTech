using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTech
{
    public static class MaterialCalculator
    {
        public static int CalculateRequiredMaterial(
            int productTypeId,
            int materialTypeId,
            int requiredQuantity,
            int stockQuantity,
            double param1,
            double param2)
        {
            using (var db = new Entities())
            {
                try
                {
                    if (requiredQuantity < 0 || stockQuantity < 0 || param1 <= 0 || param2 <= 0)
                        return -1;

                    var productType = db.ProductType.FirstOrDefault(pt => pt.Id == productTypeId);
                    if (productType == null) return -1;

                    var materialType = db.MaterialType.FirstOrDefault(mt => mt.Id == materialTypeId);
                    if (materialType == null) return -1;

                    double productCoefficient = productType.ProductTypeCoefficient;
                    double wastePercent = materialType.DefectivePercent;


                    int productionQuantity = requiredQuantity - stockQuantity;
                    if (productionQuantity <= 0) return 0;

                    double materialPerUnit = param1 * param2 * productCoefficient;
                    double totalMaterial = materialPerUnit * productionQuantity;
                    double totalWithWaste = totalMaterial * (1 + wastePercent / 100);

                    return (int)Math.Ceiling(totalWithWaste);
                }
                catch
                {
                    return -1;
                }
            }
        }
    }

}
