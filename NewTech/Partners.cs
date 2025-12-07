namespace NewTech
{
    using System;
    using System.Collections.Generic;
    
    public partial class Partners
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Partners()
        {
            this.Requests = new HashSet<Requests>();
        }
    
        public int Id { get; set; }
        public int PartnerTypeId { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string PartnerAddress { get; set; }
        public Nullable<long> INN { get; set; }
        public int Rating { get; set; }
    
        public virtual PartnerType PartnerType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requests> Requests { get; set; }
    }
}
