namespace NewTech
{
    using System;
    using System.Collections.Generic;
    
    public partial class Requests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Requests()
        {
            this.RequestItems = new HashSet<RequestItems>();
        }
    
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public double TotalCost { get; set; }
    
        public virtual Partners Partners { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestItems> RequestItems { get; set; }
    }
}
