using System.Collections.Generic;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static class RoomUtils
    {
        public static IEnumerable<UndertaleRoom> GetRooms()
        {
            return ModLoader.Data.Rooms;
        }
    }
}