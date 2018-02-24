using System.Collections.Generic;
using System.Diagnostics;

namespace PSIRSCashBook.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int? PsirsCodeId { get; set; }
        public virtual PsirsCode PsirsCode { get; set; }
        public virtual ICollection<CashBook> CashBooks { get; set; }

    }
}