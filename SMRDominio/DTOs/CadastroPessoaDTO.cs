using SMRDominio.ClassePessoa;

namespace SMRDominio.DTOs
{
    public class CadastroPessoaDTO
    {
        public int id_pessoa {  get; set; }
        public PessoaTipo id_pessoa_tipo { get; set; }
        public string? nome { get; set; }
        public string? razao_social { get; set; }
        public string? celular { get; set; }
        public string? email { get; set; }
        public string? senha_hash { get; set; }
        public bool? ativo { get; set; }
        public DateTime data_cadastro { get; set; }
        public string? documento { get; set; }
        public string? telefone1 { get; set; }
        public string? telefone2 { get; set; }
    }
}
