namespace Domain.ObjetosDeValor
{
    public record Documento
    {
        public string Numero { get; }

        public Documento(string numero)
        {
            var digitos = new string(numero.Where(char.IsDigit).ToArray());

            if (digitos.Length != 11 && digitos.Length != 14)
                throw new ArgumentException("Documento deve ser CPF (11 dÃ­gitos) ou CNPJ (14 dÃ­gitos).", nameof(numero));

            Numero = digitos;
        }

        public static implicit operator Documento(string numero) => new(numero);
        public static implicit operator string(Documento documento) => documento.Numero;

        public bool EhCpf => Numero.Length == 11;
        public bool EhCnpj => Numero.Length == 14;

        public override string ToString() => Numero;
    }
}

