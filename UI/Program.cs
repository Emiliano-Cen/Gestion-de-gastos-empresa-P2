using System;
using System.Collections.Generic;
using Dominio;

namespace UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sistema sistema = Sistema.GetInstancia();

            int opcion = -1;

            while (opcion != 0)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("   Sistema de Pagos - Menú Principal  ");
                Console.WriteLine("======================================");
                Console.WriteLine("1) Listado de todos los usuarios");
                Console.WriteLine("2) Listar pagos de un usuario por email");
                Console.WriteLine("3) Alta de usuario");
                Console.WriteLine("4) Listar usuarios por equipo");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("0) Salir");
                Console.WriteLine("======================================");
                Console.Write("Opción: "); //marca lugar donde escribir pura estetica

                //Verificacion de que la opción existe
                opcion = numeroValido();

                Console.Clear();

                if (opcion == 1)
                {
                    OpcionListarUsuarios(sistema);
                }
                else if (opcion == 2)
                {
                    OpcionListarPagosDeUsuario(sistema);
                }
                else if (opcion == 3)
                {
                    OpcionAltaUsuario(sistema);
                }
                else if (opcion == 4)
                {
                    OpcionUsuariosPorEquipo(sistema);
                }
                else if (opcion == 0)
                {
                    Console.WriteLine("Saliendo del sistema...");
                }
                else
                {
                    Console.WriteLine("Opción no válida.");
                    Pausa();//me parecía muy agresivo que se corte el menu después de salir de un opción o tener un error
                }
            }
        }

        // Opción 1: Listar usuarios

        static void OpcionListarUsuarios(Sistema sistema)
        {
            Console.WriteLine("Listado de usuarios");
            Console.WriteLine("-------------------");

            List<Usuario> usuarios = sistema.GetUsuarios();
            if (usuarios.Count == 0)
            {
                Console.WriteLine("No hay usuarios cargados.");
            }
            else
            {
                foreach (Usuario u in usuarios)
                {
                    string equipo = (u.Equipo == null) ? "(Sin equipo)" : u.Equipo.Nombre;
                    Console.WriteLine($"{u.Nombre} {u.Apellido} | {u.Email} | Equipo: {equipo}");
                }
            }
            Pausa();
        }

        // Opción 2: Listar pagos de un usuario

        static void OpcionListarPagosDeUsuario(Sistema sistema)
        {
            Console.WriteLine("Listar pagos de un usuario por email");
            Console.WriteLine("------------------------------------");

            Console.Write("Email del usuario: ");
            string email = Console.ReadLine();
            if (email == null) email = "";
            email = email.Trim().ToLower();

            Usuario usuario = sistema.BuscarUsuarioPorEmail(email);
            if (usuario == null)
            {
                Console.WriteLine("No se encontró un usuario con ese email.");
                Pausa();
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Pagos del usuario: {usuario.Nombre} {usuario.Apellido} ({usuario.Email})");
            Console.WriteLine("-----------------------------------------------------------------------");

            List<Pago> pagos = sistema.GetPagos();
            int cont = 0;
            foreach (Pago p in pagos)
            {
                if (p.Usuario != null && (p.Usuario.Email ?? "").ToLower() == email)
                {
                    Console.WriteLine(p.ResumenListado(DateTime.Today));
                    cont++;
                }
            }

            if (cont == 0)
                Console.WriteLine("No hay pagos registrados para este usuario.");

            Pausa();
        }


        // Opción 3: Alta de usuario

        static void OpcionAltaUsuario(Sistema sistema)
        {
            Console.WriteLine("Alta de usuario");
            Console.WriteLine("---------------");

            Console.Write("Nombre: ");
            string nombre = (Console.ReadLine() ?? "").Trim();

            Console.Write("Apellido: ");
            string apellido = (Console.ReadLine() ?? "").Trim();

            Console.Write("Contraseña (mínimo 8): ");
            string password = (Console.ReadLine() ?? "").Trim();

            Equipo equipo = ElegirEquipoPorIndice(sistema);
            if (equipo == null)
            {

                Console.WriteLine("No se seleccionó un equipo válido.");
                Pausa();
                return;
            }

            // Elegir rol para evitar errores
            Console.WriteLine("Rol del usuario:");
            Console.WriteLine("1) Empleado");
            Console.WriteLine("2) Gerente");
            Console.Write("Opción: ");
            int opcionRol = numeroValido();

            RolUsuario rol;
            if (opcionRol == 2)
            {
                rol = RolUsuario.Gerente;
            }
            else
            {
                rol = RolUsuario.Empleado;
            }

            DateTime fechaInc = LeerFecha("Fecha de incorporación (yyyy-mm-dd): ");

            Usuario u = new Usuario(nombre, apellido, password, equipo, fechaInc, rol);
            u.Email = ""; // vacío para que el sistema lo genere automáticamente

            try
            {

                sistema.AltaUsuario(u);
                Console.WriteLine();
                Console.WriteLine("Usuario dado de alta correctamente.");
                Console.WriteLine($"Email generado: {u.Email}");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error al crear el usuario: " + ex.Message);
            }

            Pausa();
        }


        // Opción 4: Listar usuarios por nombre de equipo
        static void OpcionUsuariosPorEquipo(Sistema sistema)
        {
            Console.WriteLine("Usuarios por equipo");
            Console.WriteLine("-------------------");

            Console.Write("Nombre del equipo: ");
            string nombreEquipo = (Console.ReadLine() ?? "").Trim();

            List<Usuario> res = sistema.UsuariosPorEquipo(nombreEquipo);
            if (res.Count == 0)
            {

                Console.WriteLine("No hay usuarios para ese equipo o el equipo no existe.");
            }
            else
            {
                foreach (Usuario u in res)
                {

                    Console.WriteLine($"{u.Nombre} {u.Apellido} | {u.Email}");
                }

            }
            Pausa();
        }


        // Error de numero no valido
        static int numeroValido()
        {
            while (true)
            {
                string input = Console.ReadLine();
                int valor;
                if (int.TryParse(input, out valor))
                {
                    return valor;

                }

                Console.Write("Ingrese un número válido: ");

            }
        }

        static DateTime LeerFecha(string mensaje)
        {
            while (true)
            {

                Console.Write(mensaje);
                string input = Console.ReadLine();
                DateTime fecha;
                if (DateTime.TryParse(input, out fecha))
                {

                    return fecha;
                }

                Console.WriteLine("Fecha inválida. Formato sugerido: 2025-10-10");
            }
        }

        static Equipo ElegirEquipoPorIndice(Sistema sistema)
        {

            List<Equipo> equipos = sistema.GetEquipos();

            Console.WriteLine();
            Console.WriteLine("Equipos disponibles:");
            for (int i = 0; i < equipos.Count; i++)
            {

                Console.WriteLine($"{i + 1}) {equipos[i].Nombre}");
            }

            Console.Write("Seleccione un equipo (número): ");
            int idx = numeroValido();

            if (idx < 1 || idx > equipos.Count) 
            { 

                return null; 
            }

            return equipos[idx - 1];
        }

        static void Pausa() //post error o salir de opción amablemente
        {

            Console.WriteLine();
            Console.Write("Presione una tecla para continuar");
            Console.ReadKey();
        }
    }
}
