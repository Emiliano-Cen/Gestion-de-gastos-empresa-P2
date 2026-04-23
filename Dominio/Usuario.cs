using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Usuario
    {
        private static int _ultId = 0;
        private DateTime fechaInc;

        public int Id { get; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Password { get; set; }
        public string Email { get; set; } = string.Empty; //Para que siempre sea vacío
        public DateTime FechaIncorporacion { get; set; }
        public Equipo Equipo { get; set; }
        public RolUsuario Rol { get; set; }

        public Usuario(string nombre, string apellido, string password, Equipo equipo, DateTime fechaIncorporacion, RolUsuario rol)
        {

            Id = ++_ultId;
            Nombre = nombre;
            Apellido = apellido;
            Password = password;
            Equipo = equipo;
            FechaIncorporacion = fechaIncorporacion;
            Rol = rol;
        }

        public Usuario()
        {
            Id = ++_ultId;

        }

        public void Validar()
        {

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new Exception("El nombre del usuario no puede estar vacío.");

            }
            if (string.IsNullOrWhiteSpace(Apellido))
            {
                throw new Exception("El apellido del usuario no puede estar vacío.");
            }
            if (Password == null || Password.Length < 8)
            {
                throw new Exception("La contraseña debe tener al menos 8 caracteres.");

            }

            if (Equipo == null)
            { 

            throw new Exception("El usuario debe pertenecer a un equipo."); 
            }

            if (Rol != RolUsuario.Empleado && Rol != RolUsuario.Gerente)
                throw new Exception("El rol del usuario es inválido.");
        }

        public override string ToString()
        {

            return $"{Id} | {Nombre} {Apellido} - {Email} - {Password}";
        }
    }
}

