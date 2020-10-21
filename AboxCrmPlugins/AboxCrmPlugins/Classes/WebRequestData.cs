namespace AboxCrmPlugins.Classes
{
    public class WebRequestData
    {
        public string ContentType { get; set; }
        public string Authorization { get; set; }

        public string InputData { get; set; }
        public string Url { get; set; }

        public WebRequestData()
        {
            ContentType = "";
            Authorization = "";
            InputData = "";
        }
    }
}