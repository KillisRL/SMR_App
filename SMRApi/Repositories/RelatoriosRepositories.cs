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

        public async Task<IEnumerable<DetalheBonificacaoExportDTO>> ObterDetalhesExportacaoAsync(DateTime inicio, DateTime fim, int idEmpresa)
        {
            using IDbConnection db = new MySqlConnection(_connectionString);

            string sql = @"
        SELECT 
            i.data_indicacao AS DataIndicacao,
            b.descricao AS DescricaoBonificacao,
            b.valor AS Valor
        FROM indicacao i
        INNER JOIN bonificacao b ON i.id_bonificacao = b.id
        WHERE i.data_indicacao BETWEEN @DataInicio AND @DataFim
          AND b.id_empresa = @IdEmpresa
          AND i.status_indicacao = 4
        ORDER BY i.data_indicacao DESC;";

            var parametros = new
            {
                DataInicio = inicio.ToString("yyyy-MM-dd 00:00:00"),
                DataFim = fim.ToString("yyyy-MM-dd 23:59:59"),
                IdEmpresa = idEmpresa
            };

            return await db.QueryAsync<DetalheBonificacaoExportDTO>(sql, parametros);
        }

        // DTO para exportação
        public class DetalheBonificacaoExportDTO
        {
            public DateTime DataIndicacao { get; set; }
            public string? DescricaoBonificacao { get; set; }
            public decimal Valor { get; set; }
        }

        public async Task<IEnumerable<RankingPromotorDTO>> ObterRankingPromotoresAsync(DateTime inicio, DateTime fim, int idEmpresa, int status)
        {
            using IDbConnection db = new MySqlConnection(_connectionString);

            string sql = @"
                SELECT 
                    p.nome AS NomePromotor,
                    COUNT(i.id) AS Quantidade
                FROM indicacao i
                INNER JOIN promotor pr ON i.id_promotor_indicou = pr.id -- Liga indicacao ao promotor
                INNER JOIN pessoa p ON pr.id_pessoa = p.id_pessoa       -- Liga promotor a pessoa (para pegar o nome)
                INNER JOIN bonificacao b ON i.id_bonificacao = b.id     -- Liga para pegar a empresa
                WHERE i.data_indicacao BETWEEN @DataInicio AND @DataFim
                  AND b.id_empresa = @IdEmpresa ";

            // Aplica o filtro de status apenas se for maior que zero
            if (status > 0)
            {
                sql += " AND i.status_indicacao = @Status ";
            }

            sql += @"
                GROUP BY p.nome
                ORDER BY Quantidade DESC;";

            var parametros = new
            {
                DataInicio = inicio.ToString("yyyy-MM-dd 00:00:00"),
                DataFim = fim.ToString("yyyy-MM-dd 23:59:59"),
                IdEmpresa = idEmpresa,
                Status = status
            };

            return await db.QueryAsync<RankingPromotorDTO>(sql, parametros);
        }



        public class CustoBonificacaoDTO
        {
            public string? Mes { get; set; }
            public double ValorCusto { get; set; }
        }
        public class RankingPromotorDTO
        {
            public string? NomePromotor { get; set; }
            public int Quantidade { get; set; }
        }

    }
}