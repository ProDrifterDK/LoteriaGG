//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datos.SqlData
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_ADMIN
    {
        public long ADM_ID { get; set; }
        public long USU_ID { get; set; }
    
        public virtual TBL_USUARIO TBL_USUARIO { get; set; }
    }
}
