using PV_NA_Pagos.Dtos;
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

        public async Task<Factura?> CrearFacturaAsync(
            int idEstudiante,
            int idMatricula,
            int idPeriodo,
            int idUsuario)
        {
            if (idEstudiante <= 0)
                throw new ArgumentException("Debe indicar un estudiante válido.");

            if (idMatricula <= 0)
                throw new ArgumentException("Debe indicar una matrícula válida.");

            if (idPeriodo <= 0)
                throw new ArgumentException("Debe indicar un período válido.");

            const decimal precioCurso = 30000m;
            if (precioCurso <= 0)
                throw new ArgumentException("El monto de la factura debe ser mayor a cero.");

            decimal impuesto = Math.Round(precioCurso * 0.02m, 2);

            var existentes = await _repository.ListarAsync(idEstudiante, idPeriodo, "Pendiente");
            if (existentes.Any(f => f.ID_Matricula == idMatricula))
                throw new InvalidOperationException("Ya existe una factura pendiente para esa matrícula en ese período.");

            var factura = new Factura
            {
                ID_Estudiante = idEstudiante,
                ID_Matricula = idMatricula,
                ID_Periodo = idPeriodo,
                Monto = precioCurso,
                Impuesto = impuesto,
                Estado = "Pendiente",
                Fecha = DateTime.Now,
                MotivoReversion = string.Empty
            };

            int idFactura = await _repository.CrearAsync(factura);
            await _repository.CrearDetalleAsync(idFactura);

            var accion = $"El usuario {idUsuario} creó la factura {idFactura} " +
                         $"para la matrícula {idMatricula} del estudiante {idEstudiante}.";
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

        public async Task<IEnumerable<Factura>> ListarAsync(
            int? idPeriodo,
            int? idEstudiante,
            string? estado)
        {
            var facturas = await _repository.ListarAsync(idEstudiante, idPeriodo, estado);
            return facturas;
        }

        public async Task<bool> ReversarAsync(
            int idFactura,
            FacturaReversarDto dto,
            int idUsuario)
        {
            if (idFactura <= 0)
                throw new ArgumentException("Debe indicar un ID de factura válido.");

            if (dto == null || string.IsNullOrWhiteSpace(dto.Motivo))
                throw new ArgumentException("Debe indicar un motivo de reversión.");

            var factura = await _repository.ObtenerPorIdAsync(idFactura);
            if (factura == null)
                throw new InvalidOperationException("La factura indicada no existe.");

            if (factura.Estado == "Pagada")
                throw new InvalidOperationException("No se puede reversar una factura pagada.");

            bool ok = await _repository.ReversarAsync(idFactura, dto.Motivo);

            if (ok)
            {
                var accion = $"El usuario {idUsuario} reversó la factura {idFactura}. Motivo: {dto.Motivo}";
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
