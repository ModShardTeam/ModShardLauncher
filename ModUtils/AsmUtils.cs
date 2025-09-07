using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        /// <summary>
        /// Check pop variables for intructions as string and create them if needed.
        /// </summary>
        /// <param name="instructions"></param>
        public static void CheckInstructionsVariables(UndertaleCode originalCode, string instructions)
        {
            Regex variableRegex = new(@"\bpop\.v\.\w\s(?<var>\w+)\.(?<name>\w+)");
            foreach (string instruction in instructions.Split('\n').Where(x => x.Contains("pop.v")))
            {
                System.Text.RegularExpressions.Match matches = variableRegex.Match(instruction);
                if (matches.Success)
                {
                    string instanceValue = matches.Groups["var"].Value;
                    if (instanceValue == "self")
                    {
                        AssemblyWrapper.CheckRefVariableOrCreate(matches.Groups["name"].Value, UndertaleInstruction.InstanceType.Self);
                    }
                    else if (instanceValue == "global")
                    {
                        AssemblyWrapper.CheckRefVariableOrCreate(matches.Groups["name"].Value, UndertaleInstruction.InstanceType.Global);
                    }
                    else if (instanceValue == "local")
                    {
                        AssemblyWrapper.CheckRefLocalVariableOrCreate(originalCode, matches.Groups["name"].Value);
                    }
                    else
                    {
                        Log.Warning(
                            "Cannot infer the instance type of {0}. There is a risk it will lead to an undefined variable.",
                            instruction
                        );
                    }
                }
            }
        }
        public static string GetAssemblyString(string fileName)
        {
            UndertaleCode originalCode = GetUMTCodeFromFile(fileName);
            return originalCode.Disassemble(ModLoader.Data.Variables, ModLoader.Data.CodeLocals.For(originalCode));
        }
        public static void SetAssemblyString(string codeAsString, string fileName)
        {
            UndertaleCode originalCode = GetUMTCodeFromFile(fileName);
            originalCode.Replace(Assembler.Assemble(codeAsString, ModLoader.Data));
        }
        public static void InsertAssemblyString(string codeAsString, string fileName, int position)
        {
            Log.Information("Trying insert assembly in: {0}", fileName);

            List<string>? originalCode = GetAssemblyString(fileName).Split("\n").ToList();
            originalCode.Insert(position, codeAsString);
            SetAssemblyString(string.Join("\n", originalCode), fileName);

            Log.Information("Patched function with InsertAssemblyString: {0}", fileName);
        }
        public static void ReplaceAssemblyString(string codeAsString, string fileName, int position)
        {
            Log.Information("Trying replace assembly in: {0}", fileName);

            List<string>? originalCode = GetAssemblyString(fileName).Split("\n").ToList();
            originalCode[position] = codeAsString;
            SetAssemblyString(string.Join("\n", originalCode), fileName);

            Log.Information("Patched function with ReplaceAssemblyString: {0}", fileName);
        }
        public static void ReplaceAssemblyString(string codeAsString, string fileName, int start, int len)
        {
            Log.Information("Trying replace assembly in: {0}", fileName);

            List<string>? originalCode = GetAssemblyString(fileName).Split("\n").ToList();
            originalCode[start] = codeAsString;
            for (int i = 1; i < Math.Min(len, originalCode.Count - start); i++) {
                originalCode[start + i] = "";
            }
            SetAssemblyString(string.Join("\n", originalCode), fileName);

            Log.Information("Patched function with ReplaceAssemblyString: {0}", fileName);
        }
        public static void InjectAssemblyInstruction(string name, Func<IEnumerable<UndertaleInstruction>, IEnumerable<UndertaleInstruction>> patch)
        {
            Log.Information("Trying inject assembly in: {0}", name);

            UndertaleCode originalCode = GetUMTCodeFromFile(name);
            originalCode.Replace(patch(originalCode.Instructions).ToList());

            Log.Information("Patched function with InjectAssemblyInstruction: {0}", name);
        }
        public static IEnumerable<UndertaleInstruction> ReplaceTable(this IEnumerable<UndertaleInstruction> original, IEnumerable<UndertaleInstruction> injected)
        {
            bool pushFound = false;
            bool callFound = false;
            foreach ((int index, UndertaleInstruction instruction) in original.Enumerate())
            {
                if (!pushFound && AssemblyWrapper.IsPushString(instruction))
                {
                    pushFound = true;
                    foreach (UndertaleInstruction replacement in injected) yield return replacement;
                }
                else if (pushFound
                    && instruction.Kind == UndertaleInstruction.Opcode.Call
                    && instruction.Type1 == UndertaleInstruction.DataType.Int32
                )
                {
                    callFound = true;
                    yield return instruction;
                }
                else if (!pushFound || callFound)
                {
                    Log.Information(string.Format("yield line {0}: {1}", index, instruction));
                    yield return instruction;
                }
            }

            if (!pushFound)
                throw new InvalidOperationException("Didnt find any string in that file, is that a legitimate table ?");
            if (!callFound)
                throw new InvalidOperationException("Didnt find any final call in that file, the function is maybe ill-formed.");
        }
    }
    public static class AssemblyWrapper
    {
        public static UndertaleInstruction.Reference<UndertaleVariable> CreateRefVariable(string name, UndertaleInstruction.InstanceType instanceType)
        {
            if (ModLoader.Data == null)
            {
                throw new NullReferenceException("Data is null");
            }

            UndertaleString str = ModLoader.Data.Strings.MakeString(name, out int id);
            bool bytecode14 = ModLoader.Data.GeneralInfo?.BytecodeVersion <= 14;
            uint oldId = ModLoader.Data.VarCount1;

            if (bytecode14)
                instanceType = UndertaleInstruction.InstanceType.Undefined;

            if (!bytecode14)
            {
                if (ModLoader.Data.IsVersionAtLeast(2, 3))
                {
                    ModLoader.Data.VarCount1++;
                    ModLoader.Data.VarCount2 = ModLoader.Data.VarCount1;
                    oldId = (uint)id;
                }
                else if (!ModLoader.Data.DifferentVarCounts)
                {
                    // Bytecode 16+
                    ModLoader.Data.VarCount1++;
                    ModLoader.Data.VarCount2++;
                }
                else
                {
                    // Bytecode 15
                    if (instanceType == UndertaleInstruction.InstanceType.Self)
                    {
                        oldId = ModLoader.Data.VarCount2;
                        ModLoader.Data.VarCount2++;
                    }
                    else if (instanceType == UndertaleInstruction.InstanceType.Global)
                    {
                        ModLoader.Data.VarCount1++;
                    }
                }
            }

            UndertaleVariable variable = new()
            {
                Name = str,
                InstanceType = instanceType,
                VarID = bytecode14 ? 0 : (int)oldId,
                NameStringID = id
            };
            ModLoader.Data.Variables.Add(variable);
            Log.Information("Created {0} variable: {1} {2}", variable.InstanceType, variable.Name.Content, variable.VarID);

            return new UndertaleInstruction.Reference<UndertaleVariable>(variable, UndertaleInstruction.VariableType.Normal);
        }
        public static void CheckRefLocalVariableOrCreate(UndertaleCode code, string name)
        {
            UndertaleCodeLocals locals = ModLoader.Data.CodeLocals.For(code);
            UndertaleCodeLocals.LocalVar? localvar = locals.Locals.FirstOrDefault(t => t.Name?.Content == name);

            if (localvar == null)
            {
                UndertaleInstruction.Reference<UndertaleVariable> refVariable = CreateRefVariable(name, UndertaleInstruction.InstanceType.Local);
                localvar = new()
                {
                    Index = (uint)refVariable.Target.VarID,
                    Name = refVariable.Target.Name
                };
                locals.Locals.Add(localvar);
            }
            else
            {
                Log.Information("Found local variable: {0}", localvar.Name.Content);
            }
        }
        public static void CheckRefVariableOrCreate(string name, UndertaleInstruction.InstanceType instanceType)
        {
            UndertaleVariable? variable = null;
            if (instanceType == UndertaleInstruction.InstanceType.Local)
            {
                throw new ArgumentException("Wrong method used for checking Local Variables");
            }
            else
            {
                variable = ModLoader.Data.Variables.FirstOrDefault(t => t.Name?.Content == name && t.InstanceType == instanceType);
            }

            if (variable == null)
            {
                CreateRefVariable(name, instanceType);
            }
            else
            {
                Log.Information("Found variable: {0} of type {1}", variable.Name.Content, variable.InstanceType);
            }
        }
        public static UndertaleInstruction.Reference<UndertaleVariable> GetRefVariableOrCreate(string name, UndertaleInstruction.InstanceType instanceType)
        {
            UndertaleInstruction.Reference<UndertaleVariable> refVariable;
            UndertaleVariable? variable = ModLoader.Data.Variables.FirstOrDefault(t => t.Name?.Content == name);
            
            if (variable == null)
                refVariable = CreateRefVariable(name, instanceType);
            else
                refVariable = new UndertaleInstruction.Reference<UndertaleVariable>(variable, UndertaleInstruction.VariableType.Normal);

            Log.Information("Found variable: {0}", refVariable);

            return refVariable;
        }
        public static string CreateLocalVarAssemblyAsString(UndertaleCode code)
        {
            IEnumerable<string> originalLocalVarsName = code.FindReferencedLocalVars().Select(x => x.Name.Content);
            IEnumerable<UndertaleCodeLocals.LocalVar> newLocalVars = ModLoader.Data.CodeLocals.For(code).Locals;
            StringBuilder sb = new();

            foreach (UndertaleCodeLocals.LocalVar newLocalVar in newLocalVars)
            {
                if (originalLocalVarsName.Contains(newLocalVar.Name.Content)) continue;
                UndertaleVariable? refVar = ModLoader.Data.Variables.FirstOrDefault(x => x.Name.Content == newLocalVar.Name.Content && x.VarID == newLocalVar.Index);
                sb.Append($".localvar {newLocalVar.Index} {newLocalVar.Name.Content}");

                if (refVar != null) sb.Append($" {ModLoader.Data.Variables.IndexOf(refVar)}\n");
                else sb.Append('\n');
            }
            if (sb.Length != 0)
            {
                Log.Information("New local var to inject.");
            }

            return sb.ToString();
        }
        public static UndertaleResourceById<UndertaleString, UndertaleChunkSTRG> CreateString(string name)
        {
            UndertaleString str = ModLoader.Data.Strings.MakeString(name, out int ind);
            Log.Information("Created string: {0}", str);
            return new UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>(str, ind);
        }
        public static UndertaleResourceById<UndertaleString, UndertaleChunkSTRG> GetStringOrCreate(string name)
        {
            UndertaleResourceById<UndertaleString, UndertaleChunkSTRG> stringById;
            (int ind, UndertaleString str) = ModLoader.Data.Strings.Enumerate().FirstOrDefault(x => x.Item2.Content == name);
            
            if (str == null)
            {
                stringById = CreateString(name);
                Log.Information(string.Format("Created string: {0}", stringById.ToString()));
            }
            else stringById = new UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>(str, ind);

            return stringById;
        }
        public static UndertaleInstruction PushShort(short val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.PushI,
                Value = val,
                Type1 = UndertaleInstruction.DataType.Int16,
            };
        }

        public static UndertaleInstruction PushInt(int val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.Push,
                Value = val,
                Type1 = UndertaleInstruction.DataType.Int32,
            };
        }

        public static UndertaleInstruction PushString(string val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.Push,
                Value = GetStringOrCreate(val),
                Type1 = UndertaleInstruction.DataType.String,
            };
        }

        public static UndertaleInstruction PushGlb(string val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.PushGlb,
                Value = GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Global),
                Type1 = UndertaleInstruction.DataType.Variable,
                TypeInst = UndertaleInstruction.InstanceType.Global,
            };
        }

        public static UndertaleInstruction PopIntGlb(string val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.Pop,
                Destination = GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Global),
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = UndertaleInstruction.DataType.Int32,
                TypeInst = UndertaleInstruction.InstanceType.Global,
            };
        }
        public static UndertaleInstruction PopIntLcl(string val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.Pop,
                Destination = GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Local),
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = UndertaleInstruction.DataType.Int32,
                TypeInst = UndertaleInstruction.InstanceType.Local,
            };
        }

        public static UndertaleInstruction PopIntSelf(string val)
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.Pop,
                Destination = GetRefVariableOrCreate(val, UndertaleInstruction.InstanceType.Self),
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = UndertaleInstruction.DataType.Int32,
                TypeInst = UndertaleInstruction.InstanceType.Self,
            };
        }
        public static UndertaleInstruction ConvString()
        {
            return new()
            {
                Kind = UndertaleInstruction.Opcode.Conv,
                Type1 = UndertaleInstruction.DataType.String,
                Type2 = UndertaleInstruction.DataType.Variable,
            };
        }
        public static IEnumerable<UndertaleInstruction> CreateStringLine(string s)
        {
            yield return PushString(s);
            yield return ConvString();
        }
        public static bool IsPushString(UndertaleInstruction instruction)
        {
            return instruction.Kind == UndertaleInstruction.Opcode.Push && instruction.Value is UndertaleResourceById<UndertaleString, UndertaleChunkSTRG> stringRef;
        }
    }
}