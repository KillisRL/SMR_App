using SMRDominio.ClassePessoa;

namespace SMRDominio.DTOs
{
    public class CadastroPessoaDTO
    {
        public int id_pessoa {  get; set; }
        public PessoaTipo id_pessoa_tipo { get; set; }
        public string? nome { get; set; }
<<<<<<< HEAD
<<<<<<< HEAD
        public string? nome_fantasia { get; set; }
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
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
