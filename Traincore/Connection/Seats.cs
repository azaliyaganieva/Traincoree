//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Traincore.Connection
{
    using System;
    using System.Collections.Generic;
    
    public partial class Seats
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Seats()
        {
            this.Tickets = new HashSet<Tickets>();
        }
    
        public int ID_Seat { get; set; }
        public int ID_Wagon { get; set; }
        public string Number_seats { get; set; }
        public string Type_seats { get; set; }
        public Nullable<bool> IsAvailable { get; set; }
        public decimal Price { get; set; }
    
        public virtual Wagons Wagons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}
