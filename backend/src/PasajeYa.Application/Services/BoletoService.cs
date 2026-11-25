using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Exceptions;
using PasajeYa.Application.Interfaces;
using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Services;

public class BoletoService : IBoletoService
{
    private readonly IBoletoRepository _boletoRepository;
    private readonly IViajeRepository _viajeRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IPasarelaPago _pasarelaPago;

    public BoletoService(IBoletoRepository boletoRepository, IViajeRepository viajeRepository, TimeProvider timeProvider, IPasarelaPago pasarelaPago)
    {
        _boletoRepository = boletoRepository;
        _viajeRepository = viajeRepository;
        _timeProvider = timeProvider;
        _pasarelaPago = pasarelaPago;
    }

    public async Task<DetalleBoletoDto> ObtenerAsync(int boletoId)
    {
        var boleto = await _boletoRepository.ObtenerPorIdAsync(boletoId);
        if (boleto is null)
            throw new NoEncontradoException($"El boleto {boletoId} no existe");

        var pagoAprobado = boleto.Pagos.FirstOrDefault(x => x.Estado == EstadoPago.Aprobado);

        return new DetalleBoletoDto(
            boleto.Id,
            boleto.Estado,
            boleto.Viaje!.Ruta!.Origen!.Nombre,
            boleto.Viaje!.Ruta!.Destino!.Nombre,
            boleto.Viaje!.Salida,
            boleto.NumeroAsiento,
            boleto.Viaje!.Precio,
            boleto.Nombres,
            boleto.Apellidos,
            boleto.Dni,
            boleto.ReservadoHasta,
            pagoAprobado?.NumeroOperacion,
            pagoAprobado?.Fecha
        );
    }

    public async Task<PagoDto> PagarAsync(int boletoId, PagarBoletoRequest request)
    {
        var errores = ValidarPago(request);
        var boleto = await _boletoRepository.ObtenerPorIdAsync(boletoId);

        if (boleto is null)
            throw new NoEncontradoException($"El boleto {boletoId} no existe");

        if (errores.Count > 0)
            throw new ValidacionException("Error al validar datos de pago", errores);

        var monto = boleto.Viaje!.Precio;
        var solicitudCobro = new SolicitudCobro
        (
            boletoId,
            request.Token,
            request.MedioPago,
            request.Cuotas,
            request.CorreoPagador,
            monto,
            request.ClaveIdempotencia
        );

        var resultado = await _pasarelaPago.CobrarAsync(solicitudCobro);

        if (!resultado.Aprobado)
            throw new InvalidOperationException("Pago rechazado");

        var ahora = _timeProvider.GetLocalNow().DateTime;
        var pago = new Pago
        {
            Estado = EstadoPago.Aprobado,
            NumeroOperacion = resultado.NumeroOperacion,
            Monto = monto,
            Fecha = ahora,
            BoletoId = boletoId
        };
        await _boletoRepository.RegistrarPagoAsync(boleto, pago);

        return new PagoDto(
            boleto.Id,
            EstadoBoleto.Pagado,
            resultado.NumeroOperacion,
            ahora,
            monto
        );
    }

    public async Task<BoletoDto> ReservarAsync(ReservarBoletoRequest request)
    {
        var errores = Validar(request);
        var ahora = _timeProvider.GetLocalNow().DateTime;
        var viaje = await _viajeRepository.ObtenerPorIdAsync(request.ViajeId);

        if (viaje is null)
            throw new NoEncontradoException($"El viaje {request.ViajeId} no existe");

        if (request.NumeroAsiento < 1 || request.NumeroAsiento > viaje.Bus!.Capacidad)
            errores["numeroAsiento"] = [$"El asiento {request.NumeroAsiento} está fuera de la capacidad del bus."];

        if (viaje.Salida <= ahora)
            errores["viajeId"] = ["El viaje ya salió, no se puede reservar."];

        if (errores.Count > 0)
            throw new ValidacionException("Error al validar datos de reserva", errores);

        if (viaje.AsientoOcupado(request.NumeroAsiento, ahora))
            throw new ConflictoException($"El número asiento {request.NumeroAsiento} ya está ocupado");

        var boletoVencido = viaje.Boletos
            .FirstOrDefault(b => b.NumeroAsiento == request.NumeroAsiento && b.Estado == EstadoBoleto.Reservado);

        var boletoNuevo = new Boleto
        {
            ViajeId = request.ViajeId,
            NumeroAsiento = request.NumeroAsiento,
            Estado = EstadoBoleto.Reservado,
            Dni = request.Dni,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Correo = request.Correo,
            Celular = request.Celular,
            ReservadoHasta = ahora.AddMinutes(15)
        };

        await _boletoRepository.ReservarAsync(boletoNuevo, boletoVencido);

        return new BoletoDto(
            boletoNuevo.Id,
            boletoNuevo.ViajeId,
            boletoNuevo.NumeroAsiento,
            boletoNuevo.Estado,
            boletoNuevo.ReservadoHasta
        );

    }

    private Dictionary<string, string[]> Validar(ReservarBoletoRequest request)
    {
        var errores = new Dictionary<string, string[]>();

        if (!Regex.IsMatch(request.Dni ?? "", @"^\d{8}$"))
            errores["dni"] = ["El DNI debe tener 8 dígitos."];

        if (string.IsNullOrWhiteSpace(request.Nombres))
            errores["nombres"] = ["Los nombres son obligatorios."];

        if (string.IsNullOrWhiteSpace(request.Apellidos))
            errores["apellidos"] = ["Los apellidos son obligatorios."];

        if (string.IsNullOrWhiteSpace(request.Correo))
            errores["correo"] = ["El correo es obligatorio."];
        else if (!Regex.IsMatch(request.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            errores["correo"] = ["El correo no es válido."];

        if (!string.IsNullOrWhiteSpace(request.Celular) &&
            !Regex.IsMatch(request.Celular, @"^\d{9}$"))
            errores["celular"] = ["El celular debe tener 9 dígitos."];

        return errores;
    }

    private static Dictionary<string, string[]> ValidarPago(PagarBoletoRequest request)
    {
        var errores = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Token))
            errores["token"] = ["El token es obligatorio."];

        if (string.IsNullOrWhiteSpace(request.MedioPago))
            errores["medioPago"] = ["El medio de pago es obligatorio."];

        if (request.Cuotas < 1)
            errores["cuotas"] = ["Las cuotas deben ser 1 o más."];

        if (string.IsNullOrWhiteSpace(request.CorreoPagador))
            errores["correoPagador"] = ["El correo del pagador es obligatorio."];

        if (string.IsNullOrWhiteSpace(request.ClaveIdempotencia))
            errores["claveIdempotencia"] = ["La clave de idempotencia es obligatoria."];

        return errores;
    }
}