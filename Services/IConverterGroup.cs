namespace DataTransferApi.Services
{
    public interface IConverterGroup
    {
        public Task<(byte[], string, string)> ConvetGrTupleAsync(IEnumerable<(byte[], string, string)> groupOfTup);
    }
}
