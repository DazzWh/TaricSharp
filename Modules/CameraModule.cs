using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;
using TaricSharp.Services;

namespace TaricSharp.Modules
{
    /// <summary>
    /// A module of commands that use the camera
    /// </summary>
    [RequireBotPermission(GuildPermission.SendMessages)]
    public class CameraModule : ModuleBase<SocketCommandContext>
    {
        private readonly CameraService _cameraService;

        public CameraModule(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [Command("camera")]
        [Alias("eyes")]
        [Summary("Takes a picture")]
        public async Task Camera()
        {
            await _cameraService.TakePicture(Context);
        }

    }
}