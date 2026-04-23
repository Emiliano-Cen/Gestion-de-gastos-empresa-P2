using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public abstract class Pago
    {
        private static int _ultId = 0;

        public int Id { get; }
        public MetodoPago Metodo { get; set; }
        public TipoGasto Tipo { get; set; }
        public Usuario Usuario { get; set; }
        public string Descripcion { get; set; }
        public double Monto { get; set; }

        protected Pago(MetodoPago metodo, TipoGasto tipo, Usuario usuario, string descripcion, double monto)
        {

            Id = ++_ultId;
            Metodo = metodo;
            Tipo = tipo;
            Usuario = usuario;
            Descripcion = descripcion;
            Monto = monto;

        }

        public Pago()
        {
            Id = _ultId;

        }
        public virtual void Validar()
        {
            if (Tipo == null)
            { 
                throw new Exception("El tipo de gasto del pago es obligatorio.");
            }
            if (Usuario == null)
            { 

                throw new Exception("El usuario del pago es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(Descripcion))
            {

               throw new Exception("La descripción del pago no puede estar vacía.");
            }
            if (Monto <= 0)
            {

                throw new Exception("El monto del pago debe ser mayor a 0.");
            }
        }

        public abstract double CalcularMontoFinal();
        public abstract bool OcurreEnMes(int anio, int mes);
        public abstract string ResumenListado(DateTime hoy);
        public override string ToString() => $"{Id} | {Tipo} - {Usuario} - {Metodo} - {Descripcion} - Monto: {Monto}";
    }
}


