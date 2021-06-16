namespace USER_SERVICE_NET.ViewModels.Commons
{
    public class APIResult<T>
    {
        public bool IsSuccessed { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public T Data { get; set; }
    }
}
