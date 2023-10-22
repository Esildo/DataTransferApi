namespace DataTransferApi.Services
{
    public interface IConverter
    {
        public Task<byte[]> ConvetGrTupZipAsync(IEnumerable<(byte[], string, string)> groupOfTup);
    }
}
