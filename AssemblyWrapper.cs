using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public class AssemblyWrapper
    {
        public static UndertaleInstruction PushShort(short val)
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.PushI,
                Value = val,
                Type1 = UndertaleInstruction.DataType.Int16,
            };
        }
    }
}