
namespace EpxSplita.Model
{
    class WebRequestModel
    {
        public bool IsAutoRepeat { get; set; }
        public bool IsRequestRunning { get; set; }
        public bool IsHttpPost { get; set; }
        public string RunMessage { get; set; }
        public string RequestResult { get; set; }
        public System.Net.CookieContainer cookieContainer { get; set; }
        public int DelayTime { get; set; } = 2000;


        public WebRequestModel()
        {
        }
    }
}
