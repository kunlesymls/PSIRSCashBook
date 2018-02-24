using System.Collections.Generic;

namespace PSIRSCashBook.Models
{
    public class PsirsCode
    {
        public int PsirsCodeId { get; set; }
        public string CodeName { get; set; }
        public virtual ICollection<CashBook> CashBooks { get; set; }
        public virtual ICollection<Item> Items { get; set; }


    }
}