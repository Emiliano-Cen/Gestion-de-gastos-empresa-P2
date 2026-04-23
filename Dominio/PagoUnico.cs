using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class PagoUnico : Pago
    {
        public DateTime Fecha { get; set; }
        public string NroRecibo { get; set; }

        public PagoUnico(MetodoPago metodo, TipoGasto tipo, Usuario usuario, string descripcion, double monto, DateTime fecha, string nroRecibo): base(metodo, tipo, usuario, descripcion, monto)
        {
            Fecha = fecha;
            NroRecibo = nroRecibo;
        }

        public override void Validar()
        {
            base.Validar();
            if (string.IsNullOrWhiteSpace(NroRecibo))
            {

                throw new Exception("El número de recibo no puede estar vacío.");
            }
        }

        public override double CalcularMontoFinal()
        {

            double mf = (Metodo == MetodoPago.Efectivo) ? 0.20 : 0.10;
            return Monto * (1 - mf);
        }

        public override bool OcurreEnMes(int anio, int mes)
        {
            return (Fecha.Year == anio && Fecha.Month == mes);
        }

        public override string ResumenListado(DateTime hoy)
        {
            return $"{Id} | Único - {Metodo} - Total: {CalcularMontoFinal()} - Recibo: {NroRecibo}";
        }
    }
}

