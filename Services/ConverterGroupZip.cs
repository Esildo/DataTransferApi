using DataTransferApi.Entities;
using System.IO.Compression;

namespace DataTransferApi.Services
{
    public class ConverterGroupZip: IConverterGroup
    {
        public async Task<(byte[],string,string)> ConvetGrTupleAsync(IEnumerable<(byte[], string, string)> groupOfTup)
        {
            return await Task.Run(() =>
            {
                using (var compressedFileStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                    {
                        foreach (var file in groupOfTup)
                        {
                            var entry = archive.CreateEntry(file.Item3);
                            using (var stream = entry.Open())
                            {
                                stream.Write(file.Item1, 0, file.Item1.Length);
                            }
                        }
                    }

                    var zipBytes = compressedFileStream.ToArray();
                    return (zipBytes, "application/zip", "group.zip");
                }
            });
        }
    }
}

