namespace Greco2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
               "dbo.tArchivos",
               c => new
               {
                   Id = c.Guid(nullable: false),
                   fechaCreacion = c.DateTime(nullable: false),
                   Nombre = c.String(nullable: false, maxLength: 100),
                   Extension = c.String(nullable: false, maxLength: 4),
                   Descargas = c.Int(nullable: false),
                   path = c.String(maxLength: 250, unicode: false),
                   DenunciaId = c.Int(nullable: false),
                   usuarioCreador = c.String(nullable: false, maxLength: 30),
               })
               .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tCommonChLogger",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    FechaCambio = c.DateTime(),
                    ObjetoModificado = c.String(maxLength: 20, unicode: false),
                    Descripcion = c.String(maxLength: 512, unicode: false),
                    ValorAnterior = c.String(maxLength: 512, unicode: false),
                    ValorActual = c.String(maxLength: 512, unicode: false),
                    Usuario = c.String(maxLength: 20, unicode: false),
                    ObjetoId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tDatosCoprec",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    honorariosCoprec = c.String(),
                    nroGestion = c.String(),
                    fechaGestionHonorarios = c.DateTime(nullable: false),
                    montoAcordado = c.String(),
                    arancel = c.String(),
                    fechaGestionArancel = c.DateTime(nullable: false),
                    DenunciaId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tDenChLogger",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    FechaCambio = c.DateTime(),
                    ObjetoModificado = c.String(maxLength: 20, unicode: false),
                    Descripcion = c.String(maxLength: 512, unicode: false),
                    ValorAnterior = c.String(maxLength: 512, unicode: false),
                    ValorActual = c.String(maxLength: 512, unicode: false),
                    Usuario = c.String(maxLength: 20, unicode: false),
                    ObjetoId = c.Int(),
                    //Denuncia_DenunciaId = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                //.ForeignKey("dbo.tDenuncias", t => t.Denuncia_DenunciaId)
                //.Index(t => t.Denuncia_DenunciaId)
                ;

            CreateTable(
                "dbo.tDenuncias",
                c => new
                {
                    DenunciaId = c.Int(nullable: false, identity: true),
                    CREATIONDATE = c.DateTime(nullable: false),
                    CREATIONPERSON = c.String(maxLength: 30, unicode: false),
                    DENUNCIANTE_ID = c.Int(nullable: false),
                    CONCILIACION_ID = c.Int(),
                    IMPUTACION_ID = c.Int(),
                    SANCION_ID = c.Int(),
                    ESTUDIO_ID = c.Int(),
                    FSELLOGCIADC = c.DateTime(nullable: false),
                    FSELLOCIA = c.DateTime(nullable: false),
                    INACTIVO = c.Boolean(nullable: false),
                    MODALIDADGESTION = c.Int(),
                    OBSERVACIONES = c.String(maxLength: 250, unicode: false),
                    ORGANISMO_ID = c.Int(),
                    RESP_EXT_ID = c.Int(),
                    RESP_INT_ID = c.Int(),
                    SERV_DEN_ID = c.Int(),
                    TIPO_PRO_ID = c.Int(),
                    SUBTIPO_PRO_ID = c.Int(),
                    RECLAMO_ID = c.Int(),
                    EXPEDIENTE_ID = c.Int(),
                    RESULTADO_ID = c.Int(),
                    DELETED = c.Boolean(),
                    ETAPA_ID = c.Int(),
                    OBJETORECLAMO = c.String(maxLength: 255, unicode: false),
                    FECHARESULTADO = c.DateTime(),
                    PARENTDENUNCIAID = c.Int(),
                    MOTIVOBAJA_ID = c.Int(),
                    TRAMITECRM = c.String(maxLength: 255, unicode: false),
                    ECMID = c.String(maxLength: 50),
                    grupoId = c.Int(),
                    nroClienteContrato = c.String(maxLength: 50, unicode: false),
                    mediadorId = c.Int(),
                    domicilioMediadorId = c.Int(),
                    reclamoRelacionado = c.String(maxLength: 50, unicode: false),
                    fechaHomologacion = c.DateTime(),
                    nroGestionCoprec = c.String(maxLength: 50, unicode: false),
                    honorariosCoprec = c.String(maxLength: 50, unicode: false),
                    fechaGestionHonorarios = c.DateTime(),
                    montoAcordado = c.String(maxLength: 50, unicode: false),
                    arancel = c.String(maxLength: 50, unicode: false),
                    fechaGestionArancel = c.DateTime(),
                    agendaCoprec = c.String(maxLength: 500, unicode: false),
                })
                .PrimaryKey(t => t.DenunciaId);

            CreateTable(
                "dbo.tDenunciantes",
                c => new
                {
                    DenuncianteId = c.Int(nullable: false, identity: true),
                    tipoDenunciante = c.String(maxLength: 2, unicode: false),
                    direccion = c.String(maxLength: 100, unicode: false),
                    email = c.String(maxLength: 100, unicode: false),
                    apellido = c.String(maxLength: 50, unicode: false),
                    linea = c.String(maxLength: 15, unicode: false),
                    nombre = c.String(maxLength: 50, unicode: false),
                    NroCliente = c.String(maxLength: 20, unicode: false),
                    NroDocumento = c.String(maxLength: 15, unicode: false),
                    Observaciones = c.String(maxLength: 500, unicode: false),
                    Telefono = c.String(maxLength: 20, unicode: false),
                    tipoDocumento = c.Int(),
                    IdGrupo = c.Int(),
                    Deleted = c.Boolean(),
                })
                .PrimaryKey(t => t.DenuncianteId);

            CreateTable(
                "dbo.tGrupos",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    IdDenunciantePrincipal = c.Int(),
                    DenunciantePrincipal_DenuncianteId = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tDenunciantes", t => t.DenunciantePrincipal_DenuncianteId)
                .Index(t => t.DenunciantePrincipal_DenuncianteId);

            CreateTable(
                "dbo.tDomicilioMediadores",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Domicilio = c.String(nullable: false, maxLength: 250, unicode: false),
                    MediadorId = c.Int(nullable: false),
                    Activo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tMediadores", t => t.MediadorId)
                .Index(t => t.MediadorId);

            CreateTable(
                "dbo.tMediadores",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 50, unicode: false),
                    Activo = c.Boolean(nullable: false),
                    Matricula = c.String(maxLength: 50, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tEmailAdress",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EmailAdress = c.String(maxLength: 250, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tLogErrors",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Fecha = c.DateTime(nullable: false),
                    Error = c.String(maxLength: 500, unicode: false),
                    UrlRequest = c.String(maxLength: 250, unicode: false),
                    UserId = c.String(maxLength: 30, unicode: false),
                    ErrorDetallado = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tEstados",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TipoEstado = c.String(maxLength: 30, unicode: false),
                    Activo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tSubEstados",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EstadoId = c.Int(nullable: false),
                    Nombre = c.String(maxLength: 100, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                    CierraDenuncia = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tEstados", t => t.EstadoId)
                .Index(t => t.EstadoId);

            CreateTable(
                "dbo.tEstudios",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 50, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tOrganismos",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 50, unicode: false),
                    Localidad_Id = c.Int(),
                    Provincia_Id = c.Int(),
                    Region_Id = c.Int(),
                    Activo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tEventos",
                c => new
                {
                    EventoId = c.Int(nullable: false, identity: true),
                    DenunciaId = c.Int(nullable: false),
                    DenunciaHistId = c.Int(),
                    Fecha = c.DateTime(),
                    Organismo_Id = c.Int(),
                    ResExId = c.Int(),
                    ResIntId = c.Int(),
                    Observacion = c.String(maxLength: 500, unicode: false),
                    Deleted = c.Boolean(),
                    FECHACREACION = c.DateTime(),
                    PRESENCIAL = c.Boolean(),
                    CREATIONPERSON = c.String(maxLength: 30, unicode: false),
                    ASISTENCIA = c.Int(),
                    REQUERIMIENTOINFORME = c.Int(),
                    SOLUCIONADO = c.Int(),
                    CONTESTADO = c.Int(),
                    TipoEventoId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.EventoId);

            CreateTable(
                "dbo.tExpedientes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Numero = c.String(maxLength: 40, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tLocalidades",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 50, unicode: false),
                    ProvinciaId = c.Int(nullable: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tProvincias", t => t.ProvinciaId, cascadeDelete: true)
                .Index(t => t.ProvinciaId);

            CreateTable(
                "dbo.tProvincias",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 50, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tModalidadGestion",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Nombre = c.String(maxLength: 50, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tMotivosDeBaja",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tMotivoDeReclamo",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                    servicioId = c.Int(nullable: false),
                    tipoProcesoId = c.Int(nullable: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tParametros",
                c => new
                {
                    KEY = c.String(nullable: false, maxLength: 255, unicode: false),
                    VALUE = c.String(maxLength: 255, unicode: false),
                })
                .PrimaryKey(t => t.KEY);

            CreateTable(
                "dbo.tReclamos",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Id_Motivo_Reclamo = c.Int(),
                    Id_SubMotivoReclamo = c.Int(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tRegiones",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tReqInforme",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Nombre = c.String(maxLength: 50, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tResponsables",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TipoResponsable = c.String(maxLength: 2, unicode: false),
                    UmeId = c.String(maxLength: 30, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                    Nombre = c.String(maxLength: 50, unicode: false),
                    Apellido = c.String(maxLength: 50, unicode: false),
                    Estudio_Id = c.Int(),
                    FechaBaja = c.DateTime(),
                    Rol = c.String(maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tResultados",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NameRol = c.String(),
                    DNRol = c.String(),
                    Aplication = c.String(),
                    System = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tServicios",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                    Grupo = c.String(maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tSolucionado",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Nombre = c.String(maxLength: 30, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tSubTipoProceso",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                    Tipo_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tTextosDenuncia",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DenunciaId = c.Int(),
                    Fecha = c.DateTime(),
                    Texto = c.String(maxLength: 500, unicode: false),
                    Usuario = c.String(maxLength: 250, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tTipoEvento",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                    Codigo = c.String(maxLength: 8, unicode: false),
                    Agendable = c.Boolean(nullable: false),
                    Instruccion = c.String(maxLength: 30, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.tTipoProceso",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(maxLength: 30, unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.GrupoDenunciantesRel",
                c => new
                {
                    GrupoDto_Id = c.Int(nullable: false),
                    DenuncianteDto_DenuncianteId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.GrupoDto_Id, t.DenuncianteDto_DenuncianteId })
                .ForeignKey("dbo.tGrupos", t => t.GrupoDto_Id, cascadeDelete: true)
                .ForeignKey("dbo.tDenunciantes", t => t.DenuncianteDto_DenuncianteId, cascadeDelete: true)
                .Index(t => t.GrupoDto_Id)
                .Index(t => t.DenuncianteDto_DenuncianteId);

            CreateTable(
                "dbo.tOrganismosEstudiosRel",
                c => new
                {
                    OrganismoDto_Id = c.Int(nullable: false),
                    EstudioDto_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.OrganismoDto_Id, t.EstudioDto_Id })
                .ForeignKey("dbo.tOrganismos", t => t.OrganismoDto_Id, cascadeDelete: true)
                .ForeignKey("dbo.tEstudios", t => t.EstudioDto_Id, cascadeDelete: true)
                .Index(t => t.OrganismoDto_Id)
                .Index(t => t.EstudioDto_Id);


        }

        public override void Down()
        {
            DropForeignKey("dbo.tLocalidades", "ProvinciaId", "dbo.tProvincias");
            DropForeignKey("dbo.tOrganismosEstudiosRel", "EstudioDto_Id", "dbo.tEstudios");
            DropForeignKey("dbo.tOrganismosEstudiosRel", "OrganismoDto_Id", "dbo.tOrganismos");
            DropForeignKey("dbo.tSubEstados", "EstadoId", "dbo.tEstados");
            DropForeignKey("dbo.tDomicilioMediadores", "MediadorId", "dbo.tMediadores");
            DropForeignKey("dbo.GrupoDenunciantesRel", "DenuncianteDto_DenuncianteId", "dbo.tDenunciantes");
            DropForeignKey("dbo.GrupoDenunciantesRel", "GrupoDto_Id", "dbo.tGrupos");
            DropForeignKey("dbo.tGrupos", "DenunciantePrincipal_DenuncianteId", "dbo.tDenunciantes");
            DropIndex("dbo.tOrganismosEstudiosRel", new[] { "EstudioDto_Id" });
            DropIndex("dbo.tOrganismosEstudiosRel", new[] { "OrganismoDto_Id" });
            DropIndex("dbo.GrupoDenunciantesRel", new[] { "DenuncianteDto_DenuncianteId" });
            DropIndex("dbo.GrupoDenunciantesRel", new[] { "GrupoDto_Id" });
            DropIndex("dbo.tLocalidades", new[] { "ProvinciaId" });
            DropIndex("dbo.tSubEstados", new[] { "EstadoId" });
            DropIndex("dbo.tDomicilioMediadores", new[] { "MediadorId" });
            DropIndex("dbo.tGrupos", new[] { "DenunciantePrincipal_DenuncianteId" });
            DropTable("dbo.tOrganismosEstudiosRel");
            DropTable("dbo.GrupoDenunciantesRel");
            DropTable("dbo.tTipoProceso");
            DropTable("dbo.tTipoEvento");
            DropTable("dbo.tTextosDenuncia");
            DropTable("dbo.tSubTipoProceso");
            DropTable("dbo.tSolucionado");
            DropTable("dbo.tServicios");
            DropTable("dbo.Roles");
            DropTable("dbo.tResultados");
            DropTable("dbo.tResponsables");
            DropTable("dbo.tReqInforme");
            DropTable("dbo.tRegiones");
            DropTable("dbo.tReclamos");
            DropTable("dbo.tParametros");
            DropTable("dbo.tMotivoDeReclamo");
            DropTable("dbo.tMotivosDeBaja");
            DropTable("dbo.tModalidadGestion");
            DropTable("dbo.tProvincias");
            DropTable("dbo.tLocalidades");
            DropTable("dbo.tExpedientes");
            DropTable("dbo.tEventos");
            DropTable("dbo.tOrganismos");
            DropTable("dbo.tEstudios");
            DropTable("dbo.tSubEstados");
            DropTable("dbo.tEstados");
            DropTable("dbo.tLogErrors");
            DropTable("dbo.tEmailAdress");
            DropTable("dbo.tMediadores");
            DropTable("dbo.tDomicilioMediadores");
            DropTable("dbo.tDenuncias");
            DropTable("dbo.tGrupos");
            DropTable("dbo.tDenunciantes");
            DropTable("dbo.tDenChLogger");
            DropTable("dbo.tDatosCoprec");
            DropTable("dbo.tCommonChLogger");
            DropTable("dbo.tArchivos");
        }
    }
}
