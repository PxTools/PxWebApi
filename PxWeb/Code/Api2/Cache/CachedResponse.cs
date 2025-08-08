namespace PxWeb.Code.Api2.Cache
{
    public class CachedResponse
    {
        public byte[] Content { get; set; }
        public string? ContentType { get; set; }
        public int ResponseCode { get; set; }
        public string ContentDisposition { get; set; }

        public string? TableId { get; set; }
        public string? Format { get; set; }
        public int? MatrixSize { get; set; }


        public CachedResponse(byte[] content, string? responseType, int responseCode, string contentDisposition)
        {
            this.Content = content;
            ContentType = responseType;
            this.ResponseCode = responseCode;
            ContentDisposition = contentDisposition;
        }
    }
}
