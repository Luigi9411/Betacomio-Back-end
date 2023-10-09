using System;
using System.Collections.Generic;

namespace Betacomio.Models;

public partial class VOrderHistory
{
    public int SalesOrderId { get; set; }

    public short OrderQty { get; set; }

    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public decimal SubTotal { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal UnitPrice { get; set; }
}
