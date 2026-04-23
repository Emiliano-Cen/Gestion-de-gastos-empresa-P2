using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Equipo
    {
        private static int _ultId = 0;

        public int Id { get; }
        public string Nombre { get; set; }

        public Equipo(string nombre)
        {

            Id = ++_ultId;
            Nombre = nombre;
        }

        public Equipo()
        {
            Id = _ultId;
        }
        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))

                throw new Exception("El nombre del equipo no puede estar vacío.");
        }

        public override string ToString()
        {
            return $"{Id} | {Nombre}";
        }
    }
}
