namespace AboxCrmPlugins.Classes
{
    public class WebRequestResponse
    {
        public string Data { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string Code { get; set; }
    }
}