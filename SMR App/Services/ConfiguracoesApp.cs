public static class ConfiguracoesApp
{
    public static string UrlApi
    {
        get
        {
#if WINDOWS
        // URL para testes locais rodando o app no Windows
        return "https://localhost:7190/";
#else
            // URL de produção para Android, iOS, etc.
            return "https://api.smrapp.com.br/";
#endif
        }
    }
}