namespace USER_SERVICE_NET.ViewModels.Commons
{
    public class APIResultSuccess<T>: APIResult<T>
    {
        public APIResultSuccess(T data)
        {
            this.IsSuccessed = true;
            this.Data = data;
        }
        public APIResultSuccess()
        {
            this.IsSuccessed = true;
        }
    }
}
