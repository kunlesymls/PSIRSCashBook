using System;
using System.ComponentModel.DataAnnotations;

namespace PSIRSCashBook.Models
{
    public class CashBook
    {
        public int CashBookId { get; set; }
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }
        public string Payee { get; set; }
        public string Purpose { get; set; }
        public string PvCode { get; set; }
        public double Amount { get; set; }
        public int ItemId { get; set; }
        public int PsirsCodeId { get; set; }
        public string StaffId { get; set; }
        public virtual PsirsCode PsirsCode { get; set; }
        public virtual Item Item { get; set; }
        public virtual Staff Staff { get; set; }
    }
    public class DisplayCashBookVm
    {

        public string TransactionDate { get; set; }
        public string Payee { get; set; }
        public string Purpose { get; set; }
        public string PvCode { get; set; }
        public double Amount { get; set; }
        public string ItemName { get; set; }
        public string CodeName { get; set; }

    }
}