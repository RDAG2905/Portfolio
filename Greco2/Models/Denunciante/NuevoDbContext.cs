using Greco2.Models;
using Greco2.Models.Denuncia;
using Greco2.Models.Estado;
using Greco2.Models.Estudios;
using Greco2.Models.Evento;
using Greco2.Models.Localidad;
using Greco2.Models.Mediador;
using Greco2.Models.MotivoDeBaja;
using Greco2.Models.MotivoDeReclamo;
using Greco2.Models.Organismo;
using Greco2.Models.Provincia;
using Greco2.Models.Region;
using Greco2.Models.Servicio;
using Greco2.Models.TipoEvento;
using Greco2.Models.Denunciante;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Greco2.Models.TipoProceso;
using Greco2.Models.ModalidadGestión;
using Greco2.Models.ReqInforme;
using Greco2.Models.Grupo;
using Greco2.Models.Solucionado;
using Greco2.Models.DatosCoprec;
using Greco2.Models.Log;
using Greco2.Models.Responsables;
using Greco2.Models.Parametros;
using Greco2.Models.Reclamo;
using Greco2.Models.Expediente;
using Greco2.Models.User;
using Greco2.Models.Mail;
using Greco2.Models.Resultados;
using Greco2.Models.TextoDenuncia;
using Greco2.Models.DenChLogger;

namespace Greco2.Model
{
    public class NuevoDbContext:DbContext
    {
        public NuevoDbContext()
            : base("LocalSqlServer")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DenuncianteModel());
            modelBuilder.Configurations.Add(new DenunciaModel());
            modelBuilder.Configurations.Add(new EventoModel());
            modelBuilder.Configurations.Add(new ArchivoModel());
            modelBuilder.Configurations.Add(new MotivoDeBajaModel());
            modelBuilder.Configurations.Add(new ProvinciaModel());
            modelBuilder.Configurations.Add(new LocalidadModel());
            modelBuilder.Configurations.Add(new EstudioModel());
            modelBuilder.Configurations.Add(new RegionModel());
            modelBuilder.Configurations.Add(new MotivoDeReclamoModel());
            modelBuilder.Configurations.Add(new OrganismoModel());
            modelBuilder.Configurations.Add(new MediadorModel());
            modelBuilder.Configurations.Add(new DomicilioMediadorModel());
            modelBuilder.Configurations.Add(new ServicioModel());
            modelBuilder.Configurations.Add(new TipoEventoModel());
            modelBuilder.Configurations.Add(new EstadoModel());
            modelBuilder.Configurations.Add(new SubEstadoModel());
            modelBuilder.Configurations.Add(new TipoProcesoModel());
            modelBuilder.Configurations.Add(new SubTipoProcesoModel());
            modelBuilder.Configurations.Add(new ModalidadGestionModel());
            modelBuilder.Configurations.Add(new ReqInformeModel());
            modelBuilder.Configurations.Add(new GrupoModel());
            modelBuilder.Configurations.Add(new SolucionadoModel());
            modelBuilder.Configurations.Add(new DatosCoprecModel());
            modelBuilder.Configurations.Add(new CommonChangeLoggerModel());
            modelBuilder.Configurations.Add(new ResponsableModel());
            modelBuilder.Configurations.Add(new LogErrorModel());
            modelBuilder.Configurations.Add(new ResultadoModel());
            modelBuilder.Configurations.Add(new ParametroModel());
            modelBuilder.Configurations.Add(new ReclamoModel());
            modelBuilder.Configurations.Add(new ExpedienteModel());
            modelBuilder.Configurations.Add(new EmailAdressModel());
            modelBuilder.Configurations.Add(new TextoDenunciaModel());
            modelBuilder.Configurations.Add(new DenChLoggerModel());
            
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<DenuncianteDto> Denunciantes { get; set; }
        public virtual DbSet<DenunciaDto> Denuncias { get; set; }
        public virtual DbSet<MotivoDeBajaDto> MotivosDeBaja { get; set; }
        public virtual DbSet<ProvinciaDto> Provincias { get; set; }
        public virtual DbSet<LocalidadDto> Localidades { get; set; }
        public virtual DbSet<EstudioDto> Estudios { get; set; }
        public virtual DbSet<RegionDto> Regiones { get; set; }
        public virtual DbSet<MotivoDeReclamoDto> MotivosDeReclamo { get; set; }
        public virtual DbSet<OrganismoDto> Organismos { get; set; }
        public virtual DbSet<MediadorDto> Mediadores { get; set; }
        public virtual DbSet<DomicilioMediadorDto> DomiciliosMediadores { get; set; }
        public virtual DbSet<ServicioDto> Servicios { get; set; }
        public virtual DbSet<TipoEventoDto> TiposDeEventos { get; set; }
        public virtual DbSet<EstadoDto> Estados { get; set; }
        public virtual DbSet<SubEstadoDto> SubEstados { get; set; }
        public virtual DbSet<TipoProcesoDto> TiposDeProceso  { get; set; }
        public virtual DbSet<SubTipoProcesoDto> SubTiposDeProceso { get; set; }
        public virtual DbSet<ModalidadGestionDto> ModalidadesDeGestion { get; set; }
        public virtual DbSet<ReqInformeDto> ReqsInforme { get; set; }
        public virtual DbSet<GrupoDto> Grupos { get; set; }
        public virtual DbSet<SolucionadoDto> Solucionados { get; set; }
        public virtual DbSet<DatosCoprecDto> DatosCoprec { get; set; }
        public virtual DbSet<CommonChangeLoggerDto> CommonChangeLogger { get; set; }
        public virtual DbSet<ResponsableDto> Responsables { get; set; }
        public virtual DbSet<LogErrorDto> ErrorLogs { get; set; }
        public virtual DbSet<ReclamoDto> Reclamos { get; set; }
        public virtual DbSet<ParametroDto> Parametros { get; set; }
        public virtual DbSet<ExpedienteDto> Expedientes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<EmailAdressDto> Emails { get; set; }
        public virtual DbSet<ResultadoDto> Resultados { get; set; }
        public virtual DbSet<TextoDenunciaDto> TextosDenuncia { get; set; }
        public virtual DbSet<DenChLoggerDto> DenChLoggers { get; set; }

        public void Add<T>(T theElement) where T : class
        {
            this.Set<T>().Add(theElement);
        }

        public void Remove<T>(T theElement) where T : class
        {
            this.Set<T>().Remove(theElement);
        }



        public IQueryable<DenuncianteDto>getDenunciantes(bool trackChanges = false)
        {
            IQueryable<DenuncianteDto> query = this.Set<DenuncianteDto>();

            if (!trackChanges)
                query = query.AsNoTracking();

            return query;
        }

        public IQueryable<DenunciaDto> getDenuncias(bool trackChanges = false)
        {
            IQueryable<DenunciaDto> query = this.Set<DenunciaDto>();

            if (!trackChanges)
                query = query.AsNoTracking();

            return query;
        }

        public IQueryable<EventoDto> getEventos(bool trackChanges = false)
        {
            IQueryable<EventoDto> query = this.Set<EventoDto>();

            if (!trackChanges)
                query = query.AsNoTracking();

            return query;
        }

        public IQueryable<MotivoDeBajaDto> getMotivos(bool trackChanges = false)
        {
            IQueryable<MotivoDeBajaDto> query = this.Set<MotivoDeBajaDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ProvinciaDto> getProvincias(bool trackChanges = false)
        {
            IQueryable<ProvinciaDto> query = this.Set<ProvinciaDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<LocalidadDto> getLocalidades(bool trackChanges = false)
        {
            IQueryable<LocalidadDto> query = this.Set<LocalidadDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<EstudioDto> getEstudios(bool trackChanges = false)
        {
            IQueryable<EstudioDto> query = this.Set<EstudioDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<RegionDto> getRegiones(bool trackChanges = false)
        {
            IQueryable<RegionDto> query = this.Set<RegionDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<MotivoDeReclamoDto> getMotivosDeReclamo(bool trackChanges = false)
        {
            IQueryable<MotivoDeReclamoDto> query = this.Set<MotivoDeReclamoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<OrganismoDto> getOrganismos(bool trackChanges = false)
        {
            IQueryable<OrganismoDto> query = this.Set<OrganismoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<MediadorDto> getMediadores(bool trackChanges = false)
        {
            IQueryable<MediadorDto> query = this.Set<MediadorDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<DomicilioMediadorDto> getDomiciliosMediadores(bool trackChanges = false)
        {
            IQueryable<DomicilioMediadorDto> query = this.Set<DomicilioMediadorDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ServicioDto> getServicios(bool trackChanges = false)
        {
            IQueryable<ServicioDto> query = this.Set<ServicioDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<TipoEventoDto> getTiposDeEventos(bool trackChanges = false)
        {
            IQueryable<TipoEventoDto> query = this.Set<TipoEventoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<EstadoDto> getEstados(bool trackChanges = false)
        {
            IQueryable<EstadoDto> query = this.Set<EstadoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<SubEstadoDto> GetSubEstados(bool trackChanges = false)
        {
            IQueryable<SubEstadoDto> query = this.Set<SubEstadoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<TipoProcesoDto> GetTiposDeProceso(bool trackChanges = false)
        {
            IQueryable<TipoProcesoDto> query = this.Set<TipoProcesoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<SubTipoProcesoDto> GetSubTiposDeProceso(bool trackChanges = false)
        {
            IQueryable<SubTipoProcesoDto> query = this.Set<SubTipoProcesoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ReqInformeDto> GetReqsInforme(bool trackChanges = false)
        {
            IQueryable<ReqInformeDto> query = this.Set<ReqInformeDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<GrupoDto> getGrupos(bool trackChanges = false)
        {
            IQueryable<GrupoDto> query = this.Set<GrupoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }
        public IQueryable<SolucionadoDto> getSolucionados(bool trackChanges = false)
        {
            IQueryable<SolucionadoDto> query = this.Set<SolucionadoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<DatosCoprecDto> getDatosCoprec(bool trackChanges = false)
        {
            IQueryable<DatosCoprecDto> query = this.Set<DatosCoprecDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<CommonChangeLoggerDto> getCommonChangeLoggers(bool trackChanges = false)
        {
            IQueryable<CommonChangeLoggerDto> query = this.Set<CommonChangeLoggerDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }


        public IQueryable<ResponsableDto> getResponsables(bool trackChanges = false)
        {
            IQueryable<ResponsableDto> query = this.Set<ResponsableDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<LogErrorDto> getErrorLogs(bool trackChanges = false)
        {
            IQueryable<LogErrorDto> query = this.Set<LogErrorDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ReclamoDto> getReclamos(bool trackChanges = false)
        {
            IQueryable<ReclamoDto> query = this.Set<ReclamoDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ParametroDto> getParametros(bool trackChanges = false)
        {
            IQueryable<ParametroDto> query = this.Set<ParametroDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ExpedienteDto> getExpedientes(bool trackChanges = false)
        {
            IQueryable<ExpedienteDto> query = this.Set<ExpedienteDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }
        public IQueryable<DenChLoggerDto> getDenChLoggers(bool trackChanges = false)
        {
            IQueryable<DenChLoggerDto> query = this.Set<DenChLoggerDto>();
            if (!trackChanges)
                query = query.AsNoTracking();
            return query;
        }
        //public IQueryable<EstadoDto> getEstados(bool trackChanges = false)
        //{
        //    IQueryable<EstadoDto> query = this.Set<EstadoDto>();
        //    if (!trackChanges)
        //        query = query.AsNoTracking();
        //    return query;
        //}

        public System.Data.Entity.DbSet<Greco2.Models.Evento.EventoDto> EventoDtoes { get; set; }
        public IDbSet<ArchivoDto> Archivos { get; set; }
    }

    


}