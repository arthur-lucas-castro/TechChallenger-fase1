namespace Domain.ObjetosDeValor
{
    public record Placa
    {
        public string Valor { get; }

        public Placa(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor) || valor.Replace("-", "").Length != 7)
                throw new ArgumentException("Placa deve ter 7 caracteres.", nameof(valor));

            Valor = valor.Replace("-", "").ToUpperInvariant();
        }

        public static implicit operator Placa(string valor) => new(valor);
        public static implicit operator string(Placa placa) => placa.Valor;

        public override string ToString() => Valor;
    }
}

