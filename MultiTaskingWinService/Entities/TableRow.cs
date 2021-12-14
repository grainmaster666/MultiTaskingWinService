using System;

namespace C9ISM.Scheduler.Entities
{
    public class TableRow
    {
        public DateTime? TransactionDate { get; set; }
        public string CompanyName { get; set; }
        public string Client { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal TradedPrice { get; set; }
        public decimal ClosedPrice { get; set; }
    }
}
