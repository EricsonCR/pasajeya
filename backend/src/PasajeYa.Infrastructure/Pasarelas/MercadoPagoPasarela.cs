
using System.Globalization;
using System.Text.Json;
using MercadoPago.Client;
using MercadoPago.Client.Order;
using MercadoPago.Error;
using MercadoPago.Http;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Infrastructure.Pasarelas;

public class MercadoPagoPasarela : IPasarelaPago
{
    private readonly string _accessToken;

    public MercadoPagoPasarela(string accessToken)
    {
        _accessToken = accessToken;
    }

    public async Task<ResultadoCobro> CobrarAsync(SolicitudCobro solicitud)
    {
        var monto = solicitud.Monto.ToString("0.00", CultureInfo.InvariantCulture);
        var request = new OrderCreateRequest
        {
            Type = "online",
            ProcessingMode = "automatic",
            ExternalReference = $"boleto-{solicitud.BoletoId}",
            TotalAmount = monto,
            Payer = new OrderPayerRequest { Email = solicitud.CorreoPagador },
            Transactions = new OrderTransactionRequest
            {
                Payments = new List<OrderPaymentRequest>
                {
                    new OrderPaymentRequest
                    {
                        Amount = monto,
                        PaymentMethod = new OrderPaymentMethodRequest
                        {
                            Id = solicitud.MedioPago,
                            Type = "credit_card",
                            Token = solicitud.Token,
                            Installments = solicitud.Cuotas
                        }
                    }
                }
            }
        };

        var opciones = new RequestOptions { AccessToken = _accessToken };
        opciones.CustomHeaders.Add(Headers.IDEMPOTENCY_KEY, solicitud.ClaveIdempotencia);

        try
        {
            var order = await new OrderClient().CreateAsync(request, opciones);
            var pago = order.Transactions.Payments[0];
            return new ResultadoCobro(pago.Status == "processed", pago.Id);
        }
        catch (MercadoPagoApiException ex) when (ex.StatusCode == 402)
        {
            using var json = JsonDocument.Parse(ex.ApiResponse.Content);
            var pagoId = json.RootElement.GetProperty("data").GetProperty("transactions")
                .GetProperty("payments")[0].GetProperty("id").GetString()!;
            return new ResultadoCobro(false, pagoId);
        }
    }
}