﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class LoteriaGGEntities : DbContext
    {
        public LoteriaGGEntities()
            : base("name=LoteriaGGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CODIGOS_PAGO_RUT> CODIGOS_PAGO_RUT { get; set; }
        public virtual DbSet<NUB_SORTEO_USUARIO> NUB_SORTEO_USUARIO { get; set; }
        public virtual DbSet<TBL_ADMIN> TBL_ADMIN { get; set; }
        public virtual DbSet<TBL_BITACORA_ERROR> TBL_BITACORA_ERROR { get; set; }
        public virtual DbSet<TBL_CONTACTO> TBL_CONTACTO { get; set; }
        public virtual DbSet<TBL_HOME> TBL_HOME { get; set; }
        public virtual DbSet<TBL_SORTEO> TBL_SORTEO { get; set; }
        public virtual DbSet<TBL_SORTEO_GRATIS> TBL_SORTEO_GRATIS { get; set; }
        public virtual DbSet<TBL_USUARIO> TBL_USUARIO { get; set; }
        public virtual DbSet<TBL_MAILS> TBL_MAILS { get; set; }
    
        public virtual ObjectResult<string> SP_BUSCAR_CLIENTE(Nullable<long> uSU_ID)
        {
            var uSU_IDParameter = uSU_ID.HasValue ?
                new ObjectParameter("USU_ID", uSU_ID) :
                new ObjectParameter("USU_ID", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_BUSCAR_CLIENTE", uSU_IDParameter);
        }
    }
}
