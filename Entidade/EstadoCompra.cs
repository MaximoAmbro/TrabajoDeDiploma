namespace Entidades
{
    // Valores de CK_Compras_Estado. Vencida: la reserva caducó sin pagarse (RN-15). No es un
    // reembolso, y la fila se conserva como rastro del intento (RN-31).
    public enum EstadoCompra
    {
        Pendiente,
        Confirmada,
        Reembolsada,
        Vencida
    }
}
