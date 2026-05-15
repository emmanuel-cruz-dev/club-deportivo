using System;

namespace ClubDeportivo.Models
{

    // ══════════════════════════════════════════════════════════════════
    //  ENUMS
    // ══════════════════════════════════════════════════════════════════

    public enum TipoPersona { Socio, NoSocio }
    public enum EstadoSocio { Activo, Suspendido }
    public enum EstadoCuota { Pendiente, Pagada, Vencida }
    public enum EstadoCarnet { Emitido, Entregado, Anulado }
    public enum MedioPago { Efectivo, TarjetaCredito }
    public enum TipoPago { CuotaMensual, Actividad }

    // ══════════════════════════════════════════════════════════════════
    //  BASE: Persona
    // ══════════════════════════════════════════════════════════════════

    public class Persona
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public TipoPersona TipoPersona { get; set; }

        public string NombreCompleto => $"{Apellido}, {Nombre}";
    }

    // ══════════════════════════════════════════════════════════════════
    //  Socio (hereda de Persona)
    // ══════════════════════════════════════════════════════════════════

    public class Socio : Persona
    {
        public int IdSocio { get; set; }
        public string NumeroSocio { get; set; }
        public DateTime FechaAlta { get; set; }
        public EstadoSocio Estado { get; set; }
        public bool AptoFisicoPresentado { get; set; }
        public DateTime? FechaVencimientoCuota { get; set; }

        public Socio() { TipoPersona = TipoPersona.Socio; }

        /// <summary>Verdadero si la cuota está al día.</summary>
        public bool EstaAlDia =>
            Estado == EstadoSocio.Activo &&
            FechaVencimientoCuota.HasValue &&
            FechaVencimientoCuota.Value >= DateTime.Today;
    }

    // ══════════════════════════════════════════════════════════════════
    //  NoSocio (hereda de Persona)
    // ══════════════════════════════════════════════════════════════════

    public class NoSocio : Persona
    {
        public int IdNoSocio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Observaciones { get; set; }

        public NoSocio() { TipoPersona = TipoPersona.NoSocio; }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Carnet
    // ══════════════════════════════════════════════════════════════════

    public class Carnet
    {
        public int IdCarnet { get; set; }
        public int IdSocio { get; set; }
        public string NumeroCarnet { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public EstadoCarnet Estado { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Cuota
    // ══════════════════════════════════════════════════════════════════

    public class Cuota
    {
        public int IdCuota { get; set; }
        public int IdSocio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Importe { get; set; }
        public EstadoCuota Estado { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Actividad
    // ══════════════════════════════════════════════════════════════════

    public class Actividad
    {
        public int IdActividad { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public string Horario { get; set; }
        public int CupoMaximo { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Pago
    // ══════════════════════════════════════════════════════════════════

    public class Pago
    {
        public int IdPago { get; set; }
        public int IdPersona { get; set; }
        public int? IdCuota { get; set; }
        public int? IdActividad { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Importe { get; set; }
        public MedioPago MedioPago { get; set; }
        public int CantidadCuotas { get; set; }
        public TipoPago Tipo { get; set; }
    }
}
