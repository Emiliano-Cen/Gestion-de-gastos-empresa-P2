using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class PagoRecurrente : Pago
    {
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public PagoRecurrente(MetodoPago metodo, TipoGasto tipo, Usuario usuario, string descripcion, double monto, DateTime fechaInicio, DateTime? fechaFin) : base(metodo, tipo, usuario, descripcion, monto)
        {

            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }


        public override void Validar()
        {
            base.Validar();

            if (FechaInicio == DateTime.MinValue)
            {
                throw new Exception("La fecha de inicio es obligatoria.");

            }
            if (FechaFin.HasValue && FechaFin.Value < FechaInicio)
            {

                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio.");
            }
        }

        private double Recargo()
        {
            if (!FechaFin.HasValue)
            {
                return 0.03;

            }
            int n = CantidadCuotas();
            if (n > 10)
            {

                return 0.10;
            }
            if (n >= 6)
            {

                return 0.05;

            }
            return 0.03;
        }

        public int CantidadCuotas()
        {
            if (!FechaFin.HasValue) 
            { 

                throw new Exception("El pago recurrente sin fin no tiene cantidad de cuotas."); 
            }

            DateTime ini = new DateTime(FechaInicio.Year, FechaInicio.Month, 1);
            DateTime fin = new DateTime(FechaFin.Value.Year, FechaFin.Value.Month, 1);
            return (fin.Year - ini.Year) * 12 + (fin.Month - ini.Month) + 1;
        }


        public double CalcularMontoMensualFinal() => Monto * (1 + Recargo());


        public override double CalcularMontoFinal()
        {

            return FechaFin.HasValue ? CantidadCuotas() * CalcularMontoMensualFinal()
                                     : CalcularMontoMensualFinal();

        }

        //pago activo si es igual al mes o año
        public override bool OcurreEnMes(int anio, int mes)
        {

            DateTime inicioMes = new DateTime(anio, mes, 1);
            DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);
            bool empiezaAntesOFIN = FechaInicio <= finMes;
            bool terminaDespuesOIgual = !FechaFin.HasValue || FechaFin.Value >= inicioMes;
            return empiezaAntesOFIN && terminaDespuesOIgual;
        }

        //resumen del pago recurrente
        public override string ResumenListado(DateTime hoy)
        {
            if (!FechaFin.HasValue)
            {

                return $"{Id} | Recurrente sin fin - {Metodo} - Total mensual: {CalcularMontoMensualFinal()} - Estado: recurrente";
            }

            int total = CantidadCuotas();
            // cuotas pagadas hasta hoy redondeo a meses:
            DateTime f = new DateTime(hoy.Year, hoy.Month, 1);
            int pagadas = 0;

            if (f >= new DateTime(FechaInicio.Year, FechaInicio.Month, 1))
            {
                DateTime ultimo = (FechaFin.Value < f) ? FechaFin.Value : f;
                pagadas = (ultimo.Year - FechaInicio.Year) * 12 + (ultimo.Month - FechaInicio.Month) + 1;
                if (pagadas < 0)
                { 

                    pagadas = 0; 
                }
                if (pagadas > total)
                {

                    pagadas = total;

                }
            }
            int pendientes = Math.Max(0, total - pagadas);
            return $"{Id} Recurrente {total} cuotas - {Metodo} - Total: {CantidadCuotas() * CalcularMontoMensualFinal()} - Restan: {pendientes}";
        }
    }
}

