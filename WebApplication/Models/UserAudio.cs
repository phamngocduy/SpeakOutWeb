//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpeakOutWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserAudio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserAudio()
        {
            this.UserAudioGroups = new HashSet<UserAudioGroup>();
        }
    
        public int Id { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UserId { get; set; }
        public byte[] LinkAudio { get; set; }
        public string TextAudio { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAudioGroup> UserAudioGroups { get; set; }
    }
}