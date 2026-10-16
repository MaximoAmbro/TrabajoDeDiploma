namespace Entidades
{
    // Sigue el estado de la compra a la que pertenece la entrada. Pendiente: reservada, sin pagar
    // y sin QR. Solo las Confirmadas cuentan como vendidas (RN-08).
    public enum EstadoEntrada
    {
        Pendiente,
        Confirmada,
        Reembolsada,
        Vencida
    }
}
