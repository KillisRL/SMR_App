public static class ConfiguracoesApp
{
    public static string UrlApi
    {
        get
        {
//#if WINDOWS
//            return "https://localhost:7190/";
//#else
            return "https://api.smrapp.com.br/";
//#endif
        }
    }
}