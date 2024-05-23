using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileTransfer;
using Grpc.Core;
using static FileTransfer.FileTransferService;

namespace server
{
    public class FileTransferService : FileTransferServiceBase
    {
        public override async Task SendFile(IAsyncStreamReader<FileTransferRequest> requestStream,
            IServerStreamWriter<FileTransferResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var path = "./image/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
               

                var data = new List<byte>();
                var fileName = "";
                var counter = 0;
                while (await requestStream.MoveNext())
                {
                    counter++;
                    var chunk = requestStream.Current;
                    if (fileName == "")
                    {
                        fileName = "1"+chunk.FileName;
                    }

                    Console.WriteLine("data wrote  " + chunk.Data.Count());
                    data.AddRange(chunk.Data);
                    var bytes = chunk.Data.ToByteArray();
                }

                Console.WriteLine("Received file with size: " + counter);

                await responseStream.WriteAsync(new FileTransferResponse()
                {
                    Success = true,
                    FileName = fileName
                });

                var dataStream = new MemoryStream(data.ToArray());

                File.WriteAllBytes(path + fileName, data.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void CopyStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}