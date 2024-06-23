using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Application.DTOs
{
    public class PurchasesDashboardDTO
    {
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public List<PurchasesSupplierDTO> TopSuppliers { get; set; }
    }
}
