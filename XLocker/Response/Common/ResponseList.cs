namespace XLocker.Response.Common
{
    public class ResponseList<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
