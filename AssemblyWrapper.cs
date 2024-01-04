using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static class AssemblyWrapper
    {
        public static UndertaleInstruction PushShort(short val)
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.PushI,
                Value = val,
                Type1 = UndertaleInstruction.DataType.Int16,
            };
        }
        
        public static UndertaleInstruction PushInt(int val)
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Value = val,
                Type1 = UndertaleInstruction.DataType.Int32,
            };
        }

        public static UndertaleInstruction PushString(string val) 
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Value = ModLoader.GetStringOrCreate(val),
                Type1 = UndertaleInstruction.DataType.String,
            };
        }

        public static UndertaleInstruction PushGlb(string val) 
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.PushGlb,
                Value = ModLoader.GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Global),
                Type1 = UndertaleInstruction.DataType.Variable,
                TypeInst = UndertaleInstruction.InstanceType.Global,
            };
        }

        public static UndertaleInstruction PopIntGlb(string val)
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.Pop,
                Destination = ModLoader.GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Global),
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = UndertaleInstruction.DataType.Int32,
                TypeInst = UndertaleInstruction.InstanceType.Global,
            };
        }
        public static UndertaleInstruction PopIntLcl(string val)
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.Pop,
                Destination = ModLoader.GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Local),
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = UndertaleInstruction.DataType.Int32,
                TypeInst = UndertaleInstruction.InstanceType.Local,
            };
        }
        
        public static UndertaleInstruction PopIntSelf(string val)
        {
            return new() {
                Kind = UndertaleInstruction.Opcode.Pop,
                Destination = ModLoader.GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Self),
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = UndertaleInstruction.DataType.Int32,
                TypeInst = UndertaleInstruction.InstanceType.Self,
            };
        }
    }
}