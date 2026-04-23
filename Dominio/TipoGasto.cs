using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class TipoGasto
    {
        private static int _ultId = 0;

        public int Id { get; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public TipoGasto(string nombre, string descripcion)
        {
            Id = ++_ultId;
            Nombre = nombre;
            Descripcion = descripcion;
        }

        public TipoGasto()
        {
            Id = ++_ultId;
        }
        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new Exception("El nombre del tipo de gasto no puede estar vacío.");
            }

        }

        public override string ToString()
        {
            return $"{Id} | {Nombre} - {Descripcion}";
        }
    }
}

