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
    
    public partial class CODIGOS_PAGO_RUT
    {
        public int PARU_ID { get; set; }
        public string PARU_CODIGO { get; set; }
        public long USU_ID { get; set; }
    
        public virtual TBL_USUARIO TBL_USUARIO { get; set; }
    }
}
