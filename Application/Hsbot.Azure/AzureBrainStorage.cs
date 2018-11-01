using System.IO;
using System.Threading.Tasks;
using Hsbot.Core;
using Hsbot.Core.Brain;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Hsbot.Azure
{
    public class AzureBrainStorage : IBotBrainStorage<HsbotBrain>
    {
        private readonly IBotBrainSerializer<HsbotBrain> _botBrainSerializer;
        private readonly IHsbotConfig _hsbotConfig;
        private readonly CloudBlobClient _blobClient;

        public AzureBrainStorage(IBotBrainSerializer<HsbotBrain> botBrainSerializer, IHsbotConfig hsbotConfig)
        {
            _botBrainSerializer = botBrainSerializer;
            _hsbotConfig = hsbotConfig;

            var storageAccount = CloudStorageAccount.Parse(hsbotConfig.BrainStorageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<HsbotBrain> Load()
        {
            var blob = GetBrainBlobReference();

            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);
                var blobContent = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

                return _botBrainSerializer.Deserialize(blobContent);
            }
        }

        public Task Save(HsbotBrain brain)
        {
            var blobContent = _botBrainSerializer.Serialize(brain);
            var blob = GetBrainBlobReference();

            return blob.UploadTextAsync(blobContent);
        }

        private CloudBlockBlob GetBrainBlobReference()
        {
            var container = _blobClient.GetContainerReference(_hsbotConfig.BrainStorageKey);
            return container.GetBlockBlobReference(_hsbotConfig.BrainName);
        }
    }
}