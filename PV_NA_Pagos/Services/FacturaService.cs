using PV_NA_Pagos.Entities;
using PV_NA_Pagos.Repository;
using System.Net.Http.Json;

namespace PV_NA_Pagos.Services
{
    public class FacturaService
    {
        private readonly FacturaRepository _repository;
        private readonly HttpClient _bitacoraClient;

        public FacturaService(FacturaRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<Factura?> CrearFacturaAsync(int idEstudiante, int idUsuario)
        {
            if (idEstudiante <= 0)
                throw new ArgumentException("Debe indicar un estudiante válido.");

            const decimal precioCurso = 30000m;
            if (precioCurso <= 0)
                throw new ArgumentException("El monto de la factura debe ser mayor a cero.");

            var facturas = await _repository.ListarAsync();
            if (facturas != null && facturas.Any(f => f.ID_Estudiante == idEstudiante && f.Estado == "Pendiente"))
                throw new InvalidOperationException("El estudiante tiene facturas pendientes.");

            decimal impuesto = Math.Round(precioCurso * 0.02m, 2);

            var factura = new Factura
            {
                ID_Estudiante = idEstudiante,
                Monto = precioCurso,
                Impuesto = impuesto,
                Estado = "Pendiente",
                Fecha = DateTime.Now
            };

            int idFactura = await _repository.CrearAsync(factura);
            await _repository.CrearDetalleAsync(idFactura);

            var accion = $"El usuario {idUsuario} creó la factura {idFactura} para el estudiante {idEstudiante}.";
            await RegistrarBitacoraAsync(idUsuario, accion);

            return await _repository.ObtenerPorIdAsync(idFactura);
        }

        public async Task<Factura?> ObtenerPorIdAsync(int idFactura)
        {
            if (idFactura <= 0)
                throw new ArgumentException("Debe indicar un ID de factura válido.");

            var factura = await _repository.ObtenerPorIdAsync(idFactura);
            if (factura == null)
                throw new InvalidOperationException("No se encontró la factura indicada.");

            return factura;
        }

        public async Task<IEnumerable<Factura>> ListarAsync()
        {
            var facturas = await _repository.ListarAsync();
            if (facturas == null || !facturas.Any())
                throw new InvalidOperationException("No existen facturas registradas.");
            return facturas;
        }

        public async Task<bool> ReversarAsync(int idFactura, int idUsuario)
        {
            if (idFactura <= 0)
                throw new ArgumentException("Debe indicar un ID de factura válido.");

            var factura = await _repository.ObtenerPorIdAsync(idFactura);
            if (factura == null)
                throw new InvalidOperationException("La factura indicada no existe.");

            if (factura.Estado == "Cancelada")
                throw new InvalidOperationException("No se puede reversar una factura cancelada.");

            bool ok = await _repository.ReversarAsync(idFactura);
            if (ok)
            {
                var accion = $"El usuario {idUsuario} reversó la factura {idFactura}.";
                await RegistrarBitacoraAsync(idUsuario, accion);
            }
            return ok;
        }

        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            try
            {
                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = idUsuario,
                    Accion = accion
                });
            }
            catch
            {
            }
        }
    }
}