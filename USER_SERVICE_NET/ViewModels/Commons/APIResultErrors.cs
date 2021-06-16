namespace USER_SERVICE_NET.ViewModels.Commons
{
    public class APIResultErrors<T>: APIResult<T>
    {
        public APIResultErrors(string message)
        {
            IsSuccessed = false;
            this.Message = message;
        }
        public APIResultErrors()
        {
            IsSuccessed = false;
        }

    }
}
