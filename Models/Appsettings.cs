namespace FieldServiceApp.Models
{
    public class Appsettings
    {
        public string WebBaseURL { get; set; }
        public string DefaultConnection { get; set; }
        public string EncKey { get; set; }
        public QBSettings QBSetting { get; set; }

    }


    public class QBSettings
    {
        public string clientid { get; set; }
        public string clientsecret { get; set; }
        public string redirectUrl { get; set; }
        public string DiscoveryUrl { get; set; }
        public string QBOBaseUrl { get; set; }

    }
}