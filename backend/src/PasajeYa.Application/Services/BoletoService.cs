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

    public BoletoService(IBoletoRepository boletoRepository, IViajeRepository viajeRepository, TimeProvider timeProvider)
    {
        _boletoRepository = boletoRepository;
        _viajeRepository = viajeRepository;
        _timeProvider = timeProvider;
    }

    public async Task<BoletoDto> ReservarAsync(ReservarBoletoRequest request)
    {
        var errores = Validar(request);
        var ahora = _timeProvider.GetLocalNow().DateTime;
        var viaje = await _viajeRepository.ObtenerPorIdAsync(request.ViajeId);

        if (viaje is null)
            throw new NoEncontradoException($"El viaje {request.ViajeId} no existe");

        if (request.NumeroAsiento < 1 || request.NumeroAsiento > viaje.Bus!.Capacidad)
            errores["numeroAsiento"] = [$"El asiento {request.NumeroAsiento} está fuera de la capacidad del bus"];

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
}