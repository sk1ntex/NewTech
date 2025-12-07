namespace NewTech
{
    using System;
    using System.Collections.Generic;
    
    public partial class RequestItems
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    
        public virtual Products Products { get; set; }
        public virtual Requests Requests { get; set; }
    }
}
