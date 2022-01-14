﻿namespace HorrorTacticsApi2.Domain.IO
{
    public interface IFileIO
    {
        Task CreateAsync(string path, Stream source, int maxBytesToRead,  IReadOnlyList<byte[]> fileSignatures, CancellationToken token);
        void Delete(string path);
    }
}
