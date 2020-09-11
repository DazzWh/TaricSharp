using System.Diagnostics.CodeAnalysis;
using Discord;

namespace TaricSharp.Services.Games
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class GameInfo
    {
        public readonly string GameName;
        public readonly string RoleName;
        public readonly Color Color;
        public readonly string ImageUrl;
        
        private GameInfo(string roleName, string gameName, Color color, string imageUrl)
        {
            RoleName = roleName;
            GameName = gameName;
            Color = color;
            ImageUrl = imageUrl;
        }
        
        public static readonly GameInfo None = 
            new GameInfo("",
                "",
                Color.Default,
                "");
        
        public static readonly GameInfo Dota =
            new GameInfo("Dota",
                "Dota",
                Color.DarkRed,
                "https://steamcdn-a.akamaihd.net/steam/apps/570/header.jpg");
        
        public static readonly GameInfo ProjectWinter =
            new GameInfo("Winter",
                "Project Winter",
                Color.Blue,
                "https://steamcdn-a.akamaihd.net/steam/apps/774861/header.jpg");
        
        public static readonly GameInfo FallGuys =
            new GameInfo("Fall Guys",
                "Fall Guys",
                Color.Magenta,
                "https://steamcdn-a.akamaihd.net/steam/apps/1097150/header.jpg");
        
        public static readonly GameInfo Pavlov =
            new GameInfo("Pavlov",
                "Pavlov",
                Color.DarkGreen,
                "https://steamcdn-a.akamaihd.net/steam/apps/555160/header.jpg");
        
        public static readonly GameInfo KillingFloor =
            new GameInfo("KF2",
                "Killing Floor",
                Color.Red,
                "https://steamcdn-a.akamaihd.net/steam/apps/232090/header.jpg");
        
        public static readonly GameInfo Jackbox =
            new GameInfo("Jackbox",
                "JackBox",
                Color.Blue,
                "https://steamcdn-a.akamaihd.net/steam/apps/331670/header.jpg");
        
        public static readonly GameInfo AmongUs =
            new GameInfo("AmongUs",
                "Among Us",
                Color.DarkBlue,
                "https://cdn.cloudflare.steamstatic.com/steam/apps/945360/header.jpg");
        
        public static readonly GameInfo Foxhole =
            new GameInfo("Foxhole",
                "Fox Hole",
                Color.DarkGreen,
                "https://cdn.cloudflare.steamstatic.com/steam/apps/505460/header_alt_assets_3.jpg");
    }
}