namespace Website.Web.Models
{
    public class ErrorViewModel
    {
        public string Header { get; set; }
        public string RequestId { get; set; }
        public int? ErrorId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; }
    }
}