using PasajeYa.Domain.Entities;

namespace PasajeYa.Application.Interfaces;

public interface IBoletoRepository
{
    Task ReservarAsync(Boleto boletoNuevo, Boleto? boletoVencido);
}