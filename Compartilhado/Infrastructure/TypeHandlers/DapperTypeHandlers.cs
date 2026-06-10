using Dapper;
using Compartilhado.Domain.ObjetosDeValor;
using System.Data;

namespace Compartilhado.Infrastructure.TypeHandlers
{
    public static class DapperTypeHandlers
    {
        public static void Registrar()
        {
            SqlMapper.AddTypeHandler(new EmailHandler());
            SqlMapper.AddTypeHandler(new TelefoneHandler());
            SqlMapper.AddTypeHandler(new DocumentoHandler());
            SqlMapper.AddTypeHandler(new PlacaHandler());
            SqlMapper.AddTypeHandler(new DinheiroHandler());
            SqlMapper.AddTypeHandler(new TipoPessoaHandler());
        }
    }

    public class EmailHandler : SqlMapper.TypeHandler<Email>
    {
        public override void SetValue(IDbDataParameter parameter, Email value)
            => parameter.Value = value.Valor;
        public override Email Parse(object value) => new((string)value);
    }

    public class TelefoneHandler : SqlMapper.TypeHandler<Telefone>
    {
        public override void SetValue(IDbDataParameter parameter, Telefone value)
            => parameter.Value = value.Valor;
        public override Telefone Parse(object value) => new((string)value);
    }

    public class DocumentoHandler : SqlMapper.TypeHandler<Documento>
    {
        public override void SetValue(IDbDataParameter parameter, Documento value)
            => parameter.Value = value.Numero;
        public override Documento Parse(object value) => new((string)value);
    }

    public class PlacaHandler : SqlMapper.TypeHandler<Placa>
    {
        public override void SetValue(IDbDataParameter parameter, Placa value)
            => parameter.Value = value.Valor;
        public override Placa Parse(object value) => new((string)value);
    }

    public class DinheiroHandler : SqlMapper.TypeHandler<Dinheiro>
    {
        public override void SetValue(IDbDataParameter parameter, Dinheiro value)
            => parameter.Value = value.Valor;
        public override Dinheiro Parse(object value) => new(Convert.ToDecimal(value));
    }

    public class TipoPessoaHandler : SqlMapper.TypeHandler<TipoPessoa>
    {
        public override void SetValue(IDbDataParameter parameter, TipoPessoa value)
            => parameter.Value = value.ToString();
        public override TipoPessoa Parse(object value) => Enum.Parse<TipoPessoa>((string)value);
    }
}
