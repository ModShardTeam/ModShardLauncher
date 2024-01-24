using System.Collections.Generic;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static IEnumerable<UndertaleRoom> GetRooms()
        {
            return ModLoader.Data.Rooms;
        }
    }
}