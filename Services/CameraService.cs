using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;

namespace TaricSharp.Services
{
    /// <summary>
    /// Service that holds the camera implementation and handles the commands.
    /// A very good idea to hook a camera up to my server, what could possibly go wrong.
    /// </summary>
    public class CameraService
    {
        private readonly DiscordSocketClient _client;
        private MMALCamera _camera;

        static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public CameraService(
            DiscordSocketClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            _camera = MMALCamera.Instance;
        }

        public async Task TakePicture(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("🙈 I have seen enough...");
            return;

            await _semaphoreSlim.WaitAsync();
            try
            {
                await context.Channel.SendMessageAsync("📷");
                using var imgCaptureHandler = new ImageStreamCaptureHandler("images/", "jpg");
                await _camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);

                var file = imgCaptureHandler.ProcessedFiles.Last();
                imgCaptureHandler.Dispose();
                await context.Channel.SendFileAsync($"{file.Directory}/{file.Filename}.{file.Extension}");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task SendInUseMessage(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("📷 Already taking a pic!");
        }
    }
}