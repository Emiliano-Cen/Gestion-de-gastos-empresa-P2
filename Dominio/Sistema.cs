using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Sistema
    {
        private static Sistema instance = null;
        private Sistema()
        {
            PrecargarDatos();
        }
        public static Sistema GetInstancia() {
            if (instance == null)
            {

                instance = new Sistema();
            }

            return instance;
        }
        

        private List<Equipo> _equipos = new List<Equipo>();
        private List<Usuario> _usuarios = new List<Usuario>();
        private List<TipoGasto> _tipos = new List<TipoGasto>();
        private List<Pago> _pagos = new List<Pago>();


        public List<Equipo> GetEquipos() { return _equipos; }
        public List<Usuario> GetUsuarios() { return _usuarios; }
        public List<TipoGasto> GetTiposGasto() { return _tipos; }
        public List<Pago> GetPagos() { return _pagos; }


        public void AltaEquipo(Equipo e)
        {
            if (e == null)
            {

                throw new Exception("El equipo no puede ser nulo.");
            }

            // evito que sean null
            if (e.Nombre == null) e.Nombre = "";
            e.Nombre = e.Nombre.Trim();

            e.Validar();

            string nombreNuevo = e.Nombre.ToLower();
            foreach (Equipo ex in _equipos)
            {

                string nombreExistente = (ex.Nombre == null) ? "" : ex.Nombre;
                if (nombreExistente.ToLower() == nombreNuevo)
                {

                    throw new Exception("Ya existe un equipo con ese nombre.");
                }
            }

            _equipos.Add(e);
        }

        public void AltaTipoGasto(TipoGasto t)
        {
            if (t == null)
            {

                throw new Exception("El tipo de gasto no puede ser nulo.");
            }

            if (t.Nombre == null) t.Nombre = "";
            if (t.Descripcion == null) t.Descripcion = "";
            t.Nombre = t.Nombre.Trim();
            t.Descripcion = t.Descripcion.Trim();

            t.Validar();

            _tipos.Add(t);
        }


        // Si Email llega vacío o nulo, se genera automáticamente
        ///Si llega con valor, se valida que no exista otro igual
        public void AltaUsuario(Usuario u)
        {

            if (u == null)
            {

                throw new Exception("El usuario no puede ser nulo.");
            }

            if (u.Nombre == null) u.Nombre = "";
            if (u.Apellido == null) u.Apellido = "";
            if (u.Password == null) u.Password = "";
            if (u.Email == null) u.Email = "";

            u.Nombre = u.Nombre.Trim();
            u.Apellido = u.Apellido.Trim();
            u.Password = u.Password.Trim();
            u.Email = u.Email.Trim();

            u.Validar();

            if (u.Email.Length == 0)
            {
                u.Email = GenerarEmailUnico(u.Nombre, u.Apellido);

            }
            else
            {

                if (EmailExiste(u.Email))
                    throw new Exception("Ya existe un usuario con ese email.");

            }

            _usuarios.Add(u);
        }

        public void AltaPago(Pago p)
        {
            if (p == null)
            {

                throw new Exception("El pago no puede ser nulo.");
            }

            //si la descripcion es nula se reemplaza por espacio vacio
            if (p.Descripcion == null) p.Descripcion = "";
            p.Descripcion = p.Descripcion.Trim();

            p.Validar();

            //convierte p en un pago recurrente si es que lo es sino lo deja null
            PagoRecurrente? pr = p as PagoRecurrente;
            if (pr != null)
            {
                //redondeo la fecha dada al primer día del mes
                pr.FechaInicio = new DateTime(pr.FechaInicio.Year, pr.FechaInicio.Month, 1);
                //evalúo que tenga fecha fin o sin limite
                if (pr.FechaFin.HasValue)
                {

                    //redondeo la fecha fin
                    DateTime finRedonda = new DateTime(pr.FechaFin.Value.Year, pr.FechaFin.Value.Month, 1);
                    if (finRedonda < pr.FechaInicio)
                    {

                        throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio en un pago recurrente.");
                    }
                    pr.FechaFin = finRedonda;
                }
            }

            _pagos.Add(p);
        }

        public string GenerarEmailUnico(string nombre, string apellido)
        {
            string baseUser = Prefijo3(nombre) + Prefijo3(apellido);
            string dominio = "@laEmpresa.com";
            string correo = baseUser + dominio;

            for (int numero = 1; EmailExiste(correo); numero++)
            {

                correo = baseUser + numero.ToString() + dominio;
            }

            return correo;
        }

        private bool EmailExiste(string emailCandidato)
        {
            string buscado = (emailCandidato == null) ? "" : emailCandidato;
            buscado = buscado.ToLower();

            foreach (Usuario u in _usuarios)
            {

                string actual = (u.Email == null) ? "" : u.Email;
                if (actual.ToLower() == buscado)
                    return true;
            }
            return false;
        }

        //genera las 3 letras a usar
        private string Prefijo3(string s)
        {

            if (s == null) s = "";
            s = s.Trim().ToLower();
            if (s.Length <= 3) return s;
            return s.Substring(0, 3);
        }


        public Usuario? BuscarUsuarioPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            string buscado = email.Trim().ToLower();
            foreach (Usuario u in _usuarios)
            {

                string actual = (u.Email == null) ? "" : u.Email;

                if (actual.ToLower() == buscado)
                {
                    return u;
                }
            }
            return null;
        }

        public List<Usuario> UsuariosPorEquipo(string nombreEquipo)
        {
            List<Usuario> res = new List<Usuario>();
            if (string.IsNullOrWhiteSpace(nombreEquipo))
            {
                return res;
            }

            string equipoBuscado = nombreEquipo.Trim().ToLower();
            foreach (Usuario u in _usuarios)
            {

                string nombreEquipoActual = "";
                if (u.Equipo != null)
                {
                    nombreEquipoActual = (u.Equipo.Nombre == null) ? "" : u.Equipo.Nombre;
                }

                if (nombreEquipoActual.ToLower() == equipoBuscado)
                {
                    res.Add(u);
                }
            }
            return res;
        }

        // Devuelve los pagos que ocurren en fecha
        // Usa OcurreEnMes
        public List<Pago> PagosDelMes(DateTime fecha)
        {
            int anio = fecha.Year;
            int mes = fecha.Month;

            List<Pago> res = new List<Pago>();
            foreach (Pago p in _pagos)
            {

                if (p.OcurreEnMes(anio, mes))
                {

                    res.Add(p);
                }
            }
            return res;
        }

        //Obl 2

        //Login
        public Usuario? ValidarLogin(string email, string password)
        {

            if (email == null) email = "";
            if (password == null) password = "";

            email = email.Trim().ToLower();

            password = password.Trim();

            foreach (Usuario u in _usuarios)
            {
                string mailU = (u.Email == null) ? "" : u.Email;
                string passU = (u.Password == null) ? "" : u.Password;

                if (mailU.ToLower() == email && passU == password)
                {
                    return u; 
                }
            }

            return null; 
        }

        //lo uso luego en OrdenarPagosPorMontoDesc
        private double MontoParaOrdenYPagos(Pago p)
        {
            PagoRecurrente pr = p as PagoRecurrente;
            if (pr != null)
            {
                // para recurrentes se usa la cuota mensual
                return pr.CalcularMontoMensualFinal();

            }

            // para pagos unicos se usa el monto final con descuentos
            return p.CalcularMontoFinal();
        }

        //orden de monto descendente
        private void OrdenarPagosPorMontoDesc(List<Pago> lista)
        {
            for (int i = 0; i < lista.Count - 1; i++)
            {

                for (int j = i + 1; j < lista.Count; j++)
                {

                    double montoI = MontoParaOrdenYPagos(lista[i]);
                    double montoJ = MontoParaOrdenYPagos(lista[j]);

                    if (montoJ > montoI)
                    {

                        Pago aux = lista[i];
                        lista[i] = lista[j];
                        lista[j] = aux;
                    }

                }
            }
        }

        //pagos de un us en un mes
        public List<Pago> PagosUsuarioEnMes(Usuario usuario, int anio, int mes)
        {
            List<Pago> lista = new List<Pago>();

            foreach (Pago p in _pagos)
            {

                if (p.Usuario == usuario && p.OcurreEnMes(anio, mes))
                {

                    lista.Add(p);

                }
            }

            OrdenarPagosPorMontoDesc(lista);

            return lista;
        }


        //total gastado por usuario en mes
        public double TotalGastadoUsuarioEnMes(Usuario usuario, int anio, int mes)
        {

            double total = 0;
            List<Pago> lista = PagosUsuarioEnMes(usuario, anio, mes);

            foreach (Pago p in lista)
            {

                total += MontoParaOrdenYPagos(p);
            }


            return total;
        }

        //pagos del equipo en mes gerent
        public List<Pago> PagosEquipoEnMes(Equipo equipo, int anio, int mes)
        {

            List<Pago> lista = new List<Pago>();

            foreach (Pago p in _pagos)
            {

                if (p.Usuario != null && p.Usuario.Equipo == equipo && p.OcurreEnMes(anio, mes))
                {

                    lista.Add(p);

                }
            }

            OrdenarPagosPorMontoDesc(lista);
            return lista;
        }


        //miembros del equipo por mail
        public List<Usuario> MiembrosDeEquipoOrdenadosPorEmail(Equipo equipo)
        {
            List<Usuario> lista = new List<Usuario>();

            foreach (Usuario u in _usuarios)
            {
                if (u.Equipo == equipo)
                {

                    lista.Add(u);
                }

            }

            for (int i = 0; i < lista.Count - 1; i++)
            {

                for (int j = i + 1; j < lista.Count; j++)
                {

                    string emailI = (lista[i].Email == null) ? "" : lista[i].Email;
                    string emailJ = (lista[j].Email == null) ? "" : lista[j].Email;

                    if (string.Compare(emailJ, emailI, StringComparison.OrdinalIgnoreCase) < 0)
                    {

                        Usuario aux = lista[i];
                        lista[i] = lista[j];
                        lista[j] = aux;

                    }
                }

            }

            return lista;
        }

        //elminar tipo gasto
        public void EliminarTipoGasto(TipoGasto tipo)
        {
            if (tipo == null)
                throw new Exception("Indique un tipo de gasto");

            // verifico que no este en uso
            foreach (Pago p in _pagos)
            {

                if (p.Tipo == tipo)
                {

                    throw new Exception("No se puede eliminar el tipo de gasto porque está siendo utilizado");

                }

            }


            _tipos.Remove(tipo);
        }

        // Precarga generada por ChatGPT luego modificada manualmente

        public void PrecargarDatos()
        {
            _equipos.Clear();
            _usuarios.Clear();
            _tipos.Clear();
            _pagos.Clear();

            // Equipos (4)
            Equipo desarrollo = new Equipo("Desarrollo");
            AltaEquipo(desarrollo);

            Equipo qa = new Equipo("QA");
            AltaEquipo(qa);

            Equipo soporte = new Equipo("Soporte");
            AltaEquipo(soporte);

            Equipo administracion = new Equipo("Administración");
            AltaEquipo(administracion);

            // Usuarios (22) (ChatGPT generó emails así que los reemplacé por empty) 
            //ajustes para OBL2 separé por equipos y el primer usuario de cada equipo es gerente, el resto empleados
            //ajustes hechos a mano directamente no era necesaria una nueva precarga a chatgpt

            //Desarrollo
            // 1
            Usuario u1 = new Usuario("Gaston", "Perez", "PassGaston1", desarrollo, new DateTime(2019, 3, 15), RolUsuario.Gerente);
            u1.Email = string.Empty;
            AltaUsuario(u1);

            // 2
            Usuario u2 = new Usuario("Lucia", "Rodriguez", "LuciaRodr1", desarrollo, new DateTime(2020, 6, 10), RolUsuario.Empleado);
            u2.Email = string.Empty;
            AltaUsuario(u2);

            // 3
            Usuario u3 = new Usuario("Martin", "Sosa", "MartinSosa2", desarrollo, new DateTime(2021, 1, 25), RolUsuario.Empleado);
            u3.Email = string.Empty;
            AltaUsuario(u3);

            // 4
            Usuario u4 = new Usuario("Sofia", "Lopez", "SofiaLopez3", desarrollo, new DateTime(2018, 11, 5), RolUsuario.Empleado);
            u4.Email = string.Empty;
            AltaUsuario(u4);

            // 5
            Usuario u5 = new Usuario("Diego", "Fernandez", "DiegoFern4", desarrollo, new DateTime(2022, 2, 14), RolUsuario.Empleado);
            u5.Email = string.Empty;
            AltaUsuario(u5);

            // 6
            Usuario u6 = new Usuario("Valentina", "Silva", "ValenSilv5", desarrollo, new DateTime(2023, 9, 1), RolUsuario.Empleado);
            u6.Email = string.Empty;
            AltaUsuario(u6);

            //QA
            // 7
            Usuario u7 = new Usuario("Camila", "Ramos", "CamilaRam6", qa, new DateTime(2020, 8, 20), RolUsuario.Gerente);
            u7.Email = string.Empty;
            AltaUsuario(u7);

            // 8
            Usuario u8 = new Usuario("Andres", "Gomez", "AndresGom7", qa, new DateTime(2019, 4, 2), RolUsuario.Empleado);
            u8.Email = string.Empty;
            AltaUsuario(u8);

            // 9
            Usuario u9 = new Usuario("Fernanda", "Mora", "Fernanda8", qa, new DateTime(2021, 7, 12), RolUsuario.Empleado);
            u9.Email = string.Empty;
            AltaUsuario(u9);

            // 10
            Usuario u10 = new Usuario("Rodrigo", "Suarez", "RodrigoS9", qa, new DateTime(2024, 1, 9), RolUsuario.Empleado);
            u10.Email = string.Empty;
            AltaUsuario(u10);

            // 11
            Usuario u11 = new Usuario("Bruno", "Castro", "BrunoCast10", qa, new DateTime(2018, 5, 30), RolUsuario.Empleado);
            u11.Email = string.Empty;
            AltaUsuario(u11);

            //Soporte
            // 12
            Usuario u12 = new Usuario("Paula", "Diaz", "PaulaDiaz11", soporte, new DateTime(2019, 10, 18), RolUsuario.Gerente);
            u12.Email = string.Empty;
            AltaUsuario(u12);

            // 13
            Usuario u13 = new Usuario("Nicolas", "Mendez", "NicoMend12", soporte, new DateTime(2020, 12, 3), RolUsuario.Empleado);
            u13.Email = string.Empty;
            AltaUsuario(u13);

            // 14
            Usuario u14 = new Usuario("Agustina", "Ruiz", "AgusRuiz13", soporte, new DateTime(2022, 4, 27), RolUsuario.Empleado);
            u14.Email = string.Empty;
            AltaUsuario(u14);

            // 15
            Usuario u15 = new Usuario("Santiago", "Alonso", "SantiAlon14", soporte, new DateTime(2023, 3, 22), RolUsuario.Empleado);
            u15.Email = string.Empty;
            AltaUsuario(u15);

            // 16
            Usuario u16 = new Usuario("Julieta", "Vega", "JulietaV15", soporte, new DateTime(2021, 9, 6), RolUsuario.Empleado);
            u16.Email = string.Empty;
            AltaUsuario(u16);

            //Administración
            // 17
            Usuario u17 = new Usuario("Marcos", "Pereyra", "MarcosPer16", administracion, new DateTime(2018, 2, 8), RolUsuario.Gerente);
            u17.Email = string.Empty;
            AltaUsuario(u17);

            // 18
            Usuario u18 = new Usuario("Carolina", "Acosta", "CarolinaA17", administracion, new DateTime(2019, 6, 19), RolUsuario.Empleado);
            u18.Email = string.Empty;
            AltaUsuario(u18);

            // 19
            Usuario u19 = new Usuario("Federico", "Brito", "FedeBrito18", administracion, new DateTime(2020, 1, 28), RolUsuario.Empleado);
            u19.Email = string.Empty;
            AltaUsuario(u19);

            // 20
            Usuario u20 = new Usuario("Rocio", "Pena", "RocioPena19", administracion, new DateTime(2022, 8, 11), RolUsuario.Empleado);
            u20.Email = string.Empty;
            AltaUsuario(u20);

            // 21
            Usuario u21 = new Usuario("Matias", "Ibarra", "MatiasIba20", administracion, new DateTime(2023, 5, 5), RolUsuario.Empleado);
            u21.Email = string.Empty;
            AltaUsuario(u21);

            // 22
            Usuario u22 = new Usuario("Eliana", "Cardozo", "ElianaCar21", administracion, new DateTime(2024, 7, 17), RolUsuario.Empleado);
            u22.Email = string.Empty;
            AltaUsuario(u22);



            // ---- Tipos de Gasto (10) ----
            TipoGasto luz = new TipoGasto("Luz", "Gasto mensual de energía eléctrica");
            AltaTipoGasto(luz);

            TipoGasto agua = new TipoGasto("Agua", "Servicio de agua potable");
            AltaTipoGasto(agua);

            TipoGasto internet = new TipoGasto("Internet", "Conexión de internet empresarial");
            AltaTipoGasto(internet);

            TipoGasto telefono = new TipoGasto("Telefonía", "Servicio de telefonía fija y móvil");
            AltaTipoGasto(telefono);

            TipoGasto alquiler = new TipoGasto("Alquiler", "Renta mensual de la oficina central");
            AltaTipoGasto(alquiler);

            TipoGasto limpieza = new TipoGasto("Limpieza", "Servicio de limpieza y mantenimiento del edificio");
            AltaTipoGasto(limpieza);

            TipoGasto seguros = new TipoGasto("Seguros", "Cobertura de seguros empresariales y de equipo");
            AltaTipoGasto(seguros);

            TipoGasto transporte = new TipoGasto("Transporte", "Gastos de movilidad y logística");
            AltaTipoGasto(transporte);

            TipoGasto software = new TipoGasto("Software", "Licencias de programas y herramientas de desarrollo");
            AltaTipoGasto(software);

            TipoGasto cafeteria = new TipoGasto("Cafetería", "Gastos de insumos para cocina y café");
            AltaTipoGasto(cafeteria);


            // =======================
            // Pagos Recurrentes (25)
            //  - 5 finalizados totalmente (FechaFin en el pasado)
            // =======================

            // 1) Alquiler oficina central (ongoing)
            PagoRecurrente pr1 = new PagoRecurrente(MetodoPago.Debito, alquiler, u17, "Alquiler oficina central", 55000.0, new DateTime(2023, 1, 1), null);
            AltaPago(pr1);

            // 2) Internet desarrollo (ongoing)
            PagoRecurrente pr2 = new PagoRecurrente(MetodoPago.Credito, internet, u1, "Internet Fibra Desarrollo", 1800.0, new DateTime(2024, 5, 1), null);
            AltaPago(pr2);

            // 3) Luz sede principal (ongoing)
            PagoRecurrente pr3 = new PagoRecurrente(MetodoPago.Debito, luz, u2, "Luz sede principal", 12000.0, new DateTime(2023, 3, 1), null);
            AltaPago(pr3);

            // 4) Agua sede principal (ongoing)
            PagoRecurrente pr4 = new PagoRecurrente(MetodoPago.Debito, agua, u3, "Agua sede principal", 4000.0, new DateTime(2022, 11, 1), null);
            AltaPago(pr4);

            // 5) Telefonía corporativa (ongoing)
            PagoRecurrente pr5 = new PagoRecurrente(MetodoPago.Credito, telefono, u7, "Telefonía corporativa", 2500.0, new DateTime(2024, 1, 1), null);
            AltaPago(pr5);

            // 6) Limpieza edificio (ongoing)
            PagoRecurrente pr6 = new PagoRecurrente(MetodoPago.Debito, limpieza, u12, "Limpieza mensual edificio", 9000.0, new DateTime(2023, 6, 1), null);
            AltaPago(pr6);

            // 7) Seguros póliza integral (ongoing)
            PagoRecurrente pr7 = new PagoRecurrente(MetodoPago.Debito, seguros, u18, "Seguros póliza integral", 7000.0, new DateTime(2024, 4, 1), null);
            AltaPago(pr7);

            // 8) Software suite dev (ongoing)
            PagoRecurrente pr8 = new PagoRecurrente(MetodoPago.Credito, software, u6, "Licencias IDE/DevTools", 15000.0, new DateTime(2023, 9, 1), null);
            AltaPago(pr8);

            // 9) Cafetería insumos mensuales (ongoing)
            PagoRecurrente pr9 = new PagoRecurrente(MetodoPago.Debito, cafeteria, u20, "Cafetería - insumos", 6000.0, new DateTime(2025, 2, 1), null);
            AltaPago(pr9);

            // 10) Transporte de paquetería (ongoing)
            PagoRecurrente pr10 = new PagoRecurrente(MetodoPago.Debito, transporte, u8, "Transporte paquetería", 8000.0, new DateTime(2025, 1, 1), null);
            AltaPago(pr10);

            // --- Finalizados totalmente (5) ---

            // 11) Software Proyecto X (finalizado: 2024-06)
            PagoRecurrente pr11 = new PagoRecurrente(MetodoPago.Credito, software, u3, "Software Proyecto X (licencias temporales)", 20000.0, new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
            AltaPago(pr11);

            // 12) Alquiler anexo depósito (finalizado: 2023-01)
            PagoRecurrente pr12 = new PagoRecurrente(MetodoPago.Debito, alquiler, u17, "Alquiler anexo depósito", 30000.0, new DateTime(2022, 2, 1), new DateTime(2023, 1, 1));
            AltaPago(pr12);

            // 13) Seguros equipo legado (finalizado: 2022-04)
            PagoRecurrente pr13 = new PagoRecurrente(MetodoPago.Debito, seguros, u18, "Seguros equipo legado", 5000.0, new DateTime(2021, 5, 1), new DateTime(2022, 4, 1));
            AltaPago(pr13);

            // 14) Telefonía Proveedor B (finalizado: 2024-03)
            PagoRecurrente pr14 = new PagoRecurrente(MetodoPago.Credito, telefono, u7, "Telefonía Proveedor B", 2200.0, new DateTime(2023, 7, 1), new DateTime(2024, 3, 1));
            AltaPago(pr14);

            // 15) Limpieza contrato anterior (finalizado: 2023-02)
            PagoRecurrente pr15 = new PagoRecurrente(MetodoPago.Debito, limpieza, u12, "Limpieza contrato anterior", 8000.0, new DateTime(2022, 9, 1), new DateTime(2023, 2, 1));
            AltaPago(pr15);

            // --- Recurrentes adicionales (ongoing o con fin futuro) ---

            // 16) Internet QA (ongoing)
            PagoRecurrente pr16 = new PagoRecurrente(MetodoPago.Credito, internet, u10, "Internet fibra QA", 1900.0, new DateTime(2024, 10, 1), null);
            AltaPago(pr16);

            // 17) Luz depósito (ongoing)
            PagoRecurrente pr17 = new PagoRecurrente(MetodoPago.Debito, luz, u17, "Luz depósito", 9000.0, new DateTime(2023, 5, 1), null);
            AltaPago(pr17);

            // 18) Agua depósito (ongoing)
            PagoRecurrente pr18 = new PagoRecurrente(MetodoPago.Debito, agua, u17, "Agua depósito", 2500.0, new DateTime(2023, 5, 1), null);
            AltaPago(pr18);

            // 19) Software IDE adicionales (ongoing)
            PagoRecurrente pr19 = new PagoRecurrente(MetodoPago.Credito, software, u6, "Licencias IDE adicionales", 10000.0, new DateTime(2025, 3, 1), null);
            AltaPago(pr19);

            // 20) Transporte chofer externo (ongoing)
            PagoRecurrente pr20 = new PagoRecurrente(MetodoPago.Debito, transporte, u13, "Transporte chofer externo", 12000.0, new DateTime(2024, 11, 1), null);
            AltaPago(pr20);

            // 21) Datos móviles (ongoing)
            PagoRecurrente pr21 = new PagoRecurrente(MetodoPago.Credito, telefono, u9, "Datos móviles corporativos", 3100.0, new DateTime(2024, 8, 1), null);
            AltaPago(pr21);

            // 22) Seguros vida colectiva (ongoing)
            PagoRecurrente pr22 = new PagoRecurrente(MetodoPago.Debito, seguros, u21, "Seguros vida colectiva", 4500.0, new DateTime(2023, 2, 1), null);
            AltaPago(pr22);

            // 23) Viandas cafetería (con fin futuro -> no está totalmente pago)
            PagoRecurrente pr23 = new PagoRecurrente(MetodoPago.Debito, cafeteria, u20, "Cafetería - viandas", 5500.0, new DateTime(2024, 2, 1), new DateTime(2026, 1, 1));
            AltaPago(pr23);

            // 24) Limpieza proveedores alternos (ongoing)
            PagoRecurrente pr24 = new PagoRecurrente(MetodoPago.Debito, limpieza, u14, "Limpieza proveedores alternos", 8500.0, new DateTime(2025, 7, 1), null);
            AltaPago(pr24);

            // 25) Software monitoreo (fin futuro -> no totalmente pago)
            PagoRecurrente pr25 = new PagoRecurrente(MetodoPago.Credito, software, u16, "Software de monitoreo", 6000.0, new DateTime(2024, 6, 1), new DateTime(2025, 12, 1));
            AltaPago(pr25);

            // =======================
            // Pagos Únicos (17)
            // =======================

            // 1) Depósito garantía alquiler
            PagoUnico pu1 = new PagoUnico(MetodoPago.Efectivo, alquiler, u17, "Depósito garantía alquiler", 80000.0, new DateTime(2023, 1, 5), "R-0001");
            AltaPago(pu1);

            // 2) Reparación de cañerías (Agua)
            PagoUnico pu2 = new PagoUnico(MetodoPago.Debito, agua, u3, "Reparación de cañerías", 15000.0, new DateTime(2024, 2, 12), "R-0002");
            AltaPago(pu2);

            // 3) Cambio tablero eléctrico (Luz)
            PagoUnico pu3 = new PagoUnico(MetodoPago.Credito, luz, u2, "Cambio de tablero eléctrico", 42000.0, new DateTime(2024, 9, 18), "R-0003");
            AltaPago(pu3);

            // 4) Router empresarial (Internet)
            PagoUnico pu4 = new PagoUnico(MetodoPago.Credito, internet, u1, "Router empresarial dual WAN", 23000.0, new DateTime(2024, 5, 22), "R-0004");
            AltaPago(pu4);

            // 5) Conmutador IP (Telefonía)
            PagoUnico pu5 = new PagoUnico(MetodoPago.Debito, telefono, u7, "Conmutador IP 32 líneas", 58000.0, new DateTime(2023, 11, 8), "R-0005");
            AltaPago(pu5);

            // 6) Muebles de recepción (Usamos Cafetería como gasto de área común)
            PagoUnico pu6 = new PagoUnico(MetodoPago.Credito, cafeteria, u20, "Muebles de recepción", 76000.0, new DateTime(2025, 3, 4), "R-0006");
            AltaPago(pu6);

            // 7) Servicio de fumigación (Limpieza)
            PagoUnico pu7 = new PagoUnico(MetodoPago.Debito, limpieza, u12, "Fumigación trimestral", 18000.0, new DateTime(2024, 12, 1), "R-0007");
            AltaPago(pu7);

            // 8) Renovación SSL y dominios (Software)
            PagoUnico pu8 = new PagoUnico(MetodoPago.Credito, software, u6, "Renovación SSL y dominios", 12000.0, new DateTime(2025, 1, 15), "R-0008");
            AltaPago(pu8);

            // 9) Flete equipos (Transporte)
            PagoUnico pu9 = new PagoUnico(MetodoPago.Efectivo, transporte, u13, "Flete equipos a depósito", 9500.0, new DateTime(2023, 10, 9), "R-0009");
            AltaPago(pu9);

            // 10) Ajuste póliza (Seguros)
            PagoUnico pu10 = new PagoUnico(MetodoPago.Debito, seguros, u18, "Ajuste póliza anual", 27000.0, new DateTime(2024, 7, 3), "R-0010");
            AltaPago(pu10);

            // 11) Anticipo proveedor de limpieza
            PagoUnico pu11 = new PagoUnico(MetodoPago.Efectivo, limpieza, u14, "Anticipo proveedor limpieza", 10000.0, new DateTime(2025, 6, 6), "R-0011");
            AltaPago(pu11);

            // 12) Compra terminales VoIP
            PagoUnico pu12 = new PagoUnico(MetodoPago.Credito, telefono, u9, "Compra 10 terminales VoIP", 30000.0, new DateTime(2024, 3, 20), "R-0012");
            AltaPago(pu12);

            // 13) Renovación UPS (Luz)
            PagoUnico pu13 = new PagoUnico(MetodoPago.Credito, luz, u2, "Renovación UPS sala de servidores", 52000.0, new DateTime(2025, 4, 11), "R-0013");
            AltaPago(pu13);

            // 14) Kit emergencia sanitaria (Agua)
            PagoUnico pu14 = new PagoUnico(MetodoPago.Debito, agua, u3, "Kit emergencia sanitaria", 8000.0, new DateTime(2023, 12, 14), "R-0014");
            AltaPago(pu14);

            // 15) Pasajes técnicos (Transporte)
            PagoUnico pu15 = new PagoUnico(MetodoPago.Credito, transporte, u8, "Pasajes técnicos interior", 22000.0, new DateTime(2024, 8, 27), "R-0015");
            AltaPago(pu15);

            // 16) Sillas ergonómicas (Cafetería como área común)
            PagoUnico pu16 = new PagoUnico(MetodoPago.Debito, cafeteria, u20, "Sillas ergonómicas x8", 64000.0, new DateTime(2025, 5, 2), "R-0016");
            AltaPago(pu16);

            // 17) Licencia perpetua herramienta específica (Software)
            PagoUnico pu17 = new PagoUnico(MetodoPago.Credito, software, u6, "Licencia perpetua herramienta específica", 45000.0, new DateTime(2024, 10, 7), "R-0017");
            AltaPago(pu17);

        
        }
    }
}
