using System.Data;
using Dapper;
using MySqlConnector;

namespace SMRApi.Repositories
{
    public class RelatoriosRepository
    {
        private readonly string _connectionString;

        public RelatoriosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<CustoBonificacaoDTO>> ObterCustoIndicacaoAsync(DateTime inicio, DateTime fim, int idEmpresa)
        {
            using IDbConnection db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT 
                    DATE_FORMAT(i.data_indicacao, '%b/%y') AS Mes,
                    SUM(b.valor) AS ValorCusto
                FROM indicacao i
                INNER JOIN bonificacao b ON i.id_bonificacao = b.id
                WHERE i.data_indicacao BETWEEN @DataInicio AND @DataFim
                  AND b.id_empresa = @IdEmpresa
                  AND i.status_indicacao = 4
                GROUP BY YEAR(i.data_indicacao), MONTH(i.data_indicacao)
                ORDER BY YEAR(i.data_indicacao), MONTH(i.data_indicacao);";

            var parametros = new
            {
                DataInicio = inicio.ToString("yyyy-MM-dd 00:00:00"),
                DataFim = fim.ToString("yyyy-MM-dd 23:59:59"),
                IdEmpresa = idEmpresa
            };

            return await db.QueryAsync<CustoBonificacaoDTO>(sql, parametros);
        }
    }

    public class CustoBonificacaoDTO
    {
        public string? Mes { get; set; }
        public double ValorCusto { get; set; }
    }
}