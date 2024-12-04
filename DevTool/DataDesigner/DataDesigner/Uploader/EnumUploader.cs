using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace DataDesigner.Uploader
{
    internal sealed class EnumUploader : IDataUplaoder
    {
        private readonly ILogger<EnumUploader> _logger;

        public EnumUploader(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumUploader>>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation($"EnumUploader initialized.");
        }

        /// <summary>
        /// Uplaod.
        /// </summary>
        public bool Upload(string path)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[EnumUploader] Upload datas.");

            try
            {
                // Delet file if exists.
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // TODO: Create file.
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return true;
        }
    }
}
