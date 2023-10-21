using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher.Extensions
{
    public class ModShard : UndertaleExtensionFile
    {
        public ModShard() 
        {
            Filename = DataLoader.data.Strings.MakeString("ModShard.dll");
            CleanupScript = DataLoader.data.Strings.MakeString("");
            InitScript = DataLoader.data.Strings.MakeString("");
            Kind = UndertaleExtensionKind.Dll;
            CreateFunc();
        }
        public void CreateFunc()
        {
            var ScriptThread = new UndertaleExtensionFunction()
            {
                Name = DataLoader.data.Strings.MakeString("ScriptThread"),
                ExtName = DataLoader.data.Strings.MakeString("ScriptThread"),
                RetType = UndertaleExtensionVarType.Double,
                Arguments = new UndertaleSimpleList<UndertaleExtensionFunctionArg>(),
                Kind = 11,
                ID = DataLoader.data.ExtensionFindLastId()
            };
            Functions.Add(ScriptThread);
            var GetScript = new UndertaleExtensionFunction()
            {
                Name = DataLoader.data.Strings.MakeString("GetScript"),
                ExtName = DataLoader.data.Strings.MakeString("GetScript"),
                RetType = UndertaleExtensionVarType.String,
                Arguments = new UndertaleSimpleList<UndertaleExtensionFunctionArg>(),
                Kind = 11,
                ID = ScriptThread.ID + 1
            };
            Functions.Add(GetScript);
            var PopScript = new UndertaleExtensionFunction()
            {
                Name = DataLoader.data.Strings.MakeString("PopScript"),
                ExtName = DataLoader.data.Strings.MakeString("PopScript"),
                RetType = UndertaleExtensionVarType.Double,
                Arguments = new UndertaleSimpleList<UndertaleExtensionFunctionArg>(),
                Kind = 11,
                ID = GetScript.ID + 1
            };
            Functions.Add(PopScript);
            var RunCallBack = new UndertaleExtensionFunction()
            {
                Name = DataLoader.data.Strings.MakeString("RunCallBack"),
                ExtName = DataLoader.data.Strings.MakeString("RunCallBack"),
                RetType = UndertaleExtensionVarType.Double,
                Arguments = new UndertaleSimpleList<UndertaleExtensionFunctionArg>()
                {
                    new UndertaleExtensionFunctionArg()
                    {
                        Type = UndertaleExtensionVarType.String
                    }
                },
                Kind = 11,
                ID = PopScript.ID + 1
            };
            Functions.Add(RunCallBack);

        }
    }
}
