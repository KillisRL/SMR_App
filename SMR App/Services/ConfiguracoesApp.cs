public static class ConfiguracoesApp
{
    public static string UrlApi
    {
        get
        {
#if WINDOWS
            return "https://localhost:7190/";
#else
            return "http://api-smr-backend-env.eba-fihsn5vm.sa-east-1.elasticbeanstalk.com/";
#endif
        }
    }
}