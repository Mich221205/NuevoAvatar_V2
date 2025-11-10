using PV_NA_Pagos.Entities;
using PV_NA_Pagos.Repository;
using System.Net.Http.Json;

namespace PV_NA_Pagos.Services
{
    public class PagoService
    {
        private readonly PagoRepository _repository;
        private readonly FacturaRepository _facturaRepository;
        private readonly HttpClient _bitacoraClient;

        public PagoService(PagoRepository repository, FacturaRepository facturaRepository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _facturaRepository = facturaRepository;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<Pago?> CrearAsync(int idFactura, decimal monto, int idUsuario)
        {
            if (idFactura <= 0)
                throw new ArgumentException("Debe indicar un ID de factura válido.");

            if (monto <= 0)
                throw new ArgumentException("El monto del pago debe ser mayor a cero.");

            var factura = await _facturaRepository.ObtenerPorIdAsync(idFactura);
            if (factura == null)
                throw new InvalidOperationException("La factura indicada no existe.");

            if (factura.Estado == "Cancelada")
                throw new InvalidOperationException("No se puede registrar un pago en una factura cancelada.");

            var totalFactura = factura.Monto + factura.Impuesto;
            if (monto != totalFactura)
                throw new InvalidOperationException("El monto del pago no coincide con el total de la factura.");

            var pagos = await _repository.ListarAsync();
            if (pagos.Any(p => p.ID_Factura == idFactura && p.Estado == "Aplicado"))
                throw new InvalidOperationException("Ya existe un pago aplicado para esta factura.");

            var pago = new Pago
            {
                ID_Factura = idFactura,
                Monto = monto,
                Estado = "Aplicado",
                FechaPago = DateTime.Now
            };

            int idPago = await _repository.CrearAsync(pago);

            await _facturaRepository.ReversarAsync(idFactura);

            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} registró el pago {idPago} para la factura {idFactura} por ₡{monto}.");

            return await _repository.ObtenerPorIdAsync(idPago);
        }

        public async Task<Pago?> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Debe indicar un ID de pago válido.");

            var pago = await _repository.ObtenerPorIdAsync(id);
            if (pago == null)
                throw new InvalidOperationException("No se encontró el pago indicado.");

            return pago;
        }

        public async Task<IEnumerable<Pago>> ListarAsync()
        {
            var pagos = await _repository.ListarAsync();
            if (pagos == null || !pagos.Any())
                throw new InvalidOperationException("No existen pagos registrados.");
            return pagos;
        }

        public async Task<bool> ReversarAsync(int id, int idUsuario)
        {
            if (id <= 0)
                throw new ArgumentException("Debe indicar un ID de pago válido.");

            var pago = await _repository.ObtenerPorIdAsync(id);
            if (pago == null)
                throw new InvalidOperationException("El pago indicado no existe.");

            bool ok = await _repository.ReversarAsync(id);
            if (ok)
            {
                await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} anuló el pago {id}.");
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
