using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Serilog;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace ModShardLauncher
{
    public static class FilePacker
    {
        public static void Pack(string path)
        {
            Log.Information(string.Format("Starting packing {0}", path));

            DirectoryInfo dir = new(path);
            FileInfo[] textures = dir.GetFiles("*.png", SearchOption.AllDirectories);
            FileInfo[] scripts = dir.GetFiles("*.lua", SearchOption.AllDirectories);
            FileInfo[] codes = dir.GetFiles("*.gml", SearchOption.AllDirectories);
            FileInfo[] assemblies = dir.GetFiles("*.asm", SearchOption.AllDirectories);
            int offset = 0;
            FileStream fs = new(Path.Join(ModLoader.ModPath, dir.Name + ".sml"), FileMode.Create);

            Write(fs, "MSLM");
            Log.Information("Writting header...");
            
            // work around to find the FileVersion of ModShardLauncher.dll for single file publishing
            // see: https://github.com/dotnet/runtime/issues/13051
            Assembly assembly = Assembly.GetExecutingAssembly() ?? throw new NullReferenceException("ExecutingAssembly");
            string mod_version = "v" + FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            Write(fs, mod_version);
            Log.Information("Writting version...");

            Write(fs, textures.Length);
            foreach (FileInfo tex in textures)
            {
                string name = dir.Name + tex.FullName.Replace(path, "");
                Write(fs, name.Length);
                Write(fs, name);
                Write(fs, offset);
                int len = CalculateBytesLength(tex);
                Write(fs, len);
                offset += len;
            }
            Log.Information("Preparing textures...");

            Write(fs, scripts.Length);
            foreach (FileInfo scr in scripts)
            {
                string name = dir.Name + scr.FullName.Replace(path, "");
                Write(fs, name.Length);
                Write(fs, name);
                Write(fs, offset);
                int len = CalculateBytesLength(scr);
                Write(fs, len);
                offset += len;
                
            }
            Log.Information("Preparing scripts...");

            Write(fs, codes.Length);
            foreach (FileInfo cds in codes)
            {
                string name = dir.Name + cds.FullName.Replace(path, "");
                Write(fs, name.Length);
                Write(fs, name);
                Write(fs, offset);
                int len = CalculateBytesLength(cds);
                Write(fs, len);
                offset += len;
            }
            Log.Information("Preparing codes...");

            Write(fs, assemblies.Length);
            foreach (FileInfo asm in assemblies)
            {
                string name = dir.Name + asm.FullName.Replace(path, "");
                Write(fs, name.Length);
                Write(fs, name);
                Write(fs, offset);
                int len = CalculateBytesLength(asm);
                Write(fs, len);
                offset += len;
            }
            Log.Information("Preparing assemblies...");

            foreach (FileInfo tex in textures)
                Write(fs, tex);
            Log.Information("Writting textures...");

            foreach (FileInfo scr in scripts)
                Write(fs, scr);
            Log.Information("Writting scripts...");

            foreach (FileInfo cds in codes)
                Write(fs, cds);
            Log.Information("Writting codes...");

            foreach (FileInfo asm in assemblies)
                Write(fs, asm);
            Log.Information("Writting assemblies...");

            Log.Information("Starting compilation...");
            bool successful = CompileMod(dir.Name, path, out byte[] code, out _);
            if (!successful)
            {
                fs.Close();
                File.Delete(fs.Name);
                Log.Information(string.Format("Failed packing {0}", dir.Name));
                return;
            }

            Write(fs, code.Length);
            Write(fs, code);
            Log.Information(string.Format("Successfully packed {0}", dir.Name));
            fs.Close();
        }
        public static void Write(FileStream fs, object obj)
        {
            Type type = obj.GetType();
            if (type == typeof(int))
            {
                byte[]? bytes = BitConverter.GetBytes((int)obj);
                fs.Write(bytes, 0, bytes.Length);
            }
            else if(type == typeof(string))
            {
                byte[]? bytes = Encoding.UTF8.GetBytes((string)obj);
                fs.Write(bytes, 0, bytes.Length);
            }
            else if(type == typeof(FileInfo))
            {
                FileStream stream = new(((FileInfo)obj).FullName, FileMode.Open);
                byte[]? bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                fs.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            else if(type == typeof(byte[]))
            {
                fs.Write((byte[])obj);
            }
        }
        public static int CalculateBytesLength(FileInfo f)
        {
            FileStream stream = new(f.FullName, FileMode.Open);
            int len = (int)stream.Length;
            stream.Close();
            return len;
        }
        public static Diagnostic[] RoslynCompile(string name, IEnumerable<string> files, IEnumerable<string> preprocessorSymbols, out byte[] code, out byte[] pdb)
        {
            // creating default namespaces
            IEnumerable<string> DefaultNamespaces = new[] { "System.Collections.Generic" };

            // creating compilation options
            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, checkOverflow: true, optimizationLevel: OptimizationLevel.Release, allowUnsafe: false)
                .WithUsings(DefaultNamespaces);

            // creating parse options
            CSharpParseOptions parseOptions = new(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);

            // creating emit options
            EmitOptions emitOptions = new(debugInformationFormat: DebugInformationFormat.PortablePdb);

            // getting all dlls
            string[] dlls = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");

            // convert string of dll into MetadataReference
            List<MetadataReference> defaultReferences = dlls
                .ToList()
                .ConvertAll(
                    new Converter<string, MetadataReference>(delegate(string str) { return MetadataReference.CreateFromFile(str); })
                );

            // add more references
            defaultReferences.AddRange(new List<MetadataReference>() { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            IEnumerable<SyntaxTree> src = files.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, f, Encoding.UTF8));
            Log.Information("Compilation: Writting ast...");
            CSharpCompilation comp = CSharpCompilation.Create(name, src, defaultReferences, options);

            Log.Information("Compilation: used Assemblies...");
            foreach(MetadataReference usedAssemblyReferences in comp.GetUsedAssemblyReferences())
            {
                if (usedAssemblyReferences.Display != null)
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(usedAssemblyReferences.Display);
                    Log.Information(string.Format("{{{0}}} {{{1}}} {{{2}}}", 
                        usedAssemblyReferences.Display,
                        fileVersionInfo.FileVersion,
                        fileVersionInfo.ProductName
                    ));
                }
                else
                {
                    Log.Error("Cannot find the assembly");
                }
                
            }

            foreach(SyntaxTree tree in comp.SyntaxTrees)
            {
                // get the semantic model for this tree
                SemanticModel model = comp.GetSemanticModel(tree);
                
                // find everywhere in the AST that refers to a type
                SyntaxNode root = tree.GetRoot();
                IEnumerable<TypeSyntax> allTypeNames = root.DescendantNodesAndSelf().OfType<TypeSyntax>();
                
                foreach(TypeSyntax typeName in allTypeNames)
                {
                    // what does roslyn think the type _name_ actually refers to?
                    Microsoft.CodeAnalysis.TypeInfo effectiveType = model.GetTypeInfo(typeName);
                    if(effectiveType.Type != null && effectiveType.Type.TypeKind == TypeKind.Error)
                    {
                        // if it's an error type (ie. couldn't be resolved), cast and proceed
                        Log.Error("Cannot understand type {0} of variable {1}", (IErrorTypeSymbol)effectiveType.Type, typeName);
                    }
                }
            }

            using MemoryStream peStream = new();
            using MemoryStream pdbStream = new();

            EmitResult results = comp.Emit(peStream, pdbStream, options: emitOptions);

            code = peStream.ToArray();
            pdb = pdbStream.ToArray();

            return results.Diagnostics.ToArray();
        }
        public static bool CompileMod(string name, string path, out byte[] code, out byte[] pdb)
        {
            IEnumerable<string> files = Directory
                .GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(file => !IgnoreCompletely(name, file));
            
            Log.Information("Compilation: Gathering files...");
            Diagnostic[] result = RoslynCompile(name, files, new[] { "FNA" }, out code, out pdb);

            Log.Information("Compilation: Gathering results...");
            
            foreach(Diagnostic err in result.Where(e => e.Severity == DiagnosticSeverity.Error))
            {
                Log.Error(err.ToString());
            }
            foreach(Diagnostic warning in result.Where(e => e.Severity == DiagnosticSeverity.Warning))
            {
                Log.Warning(warning.ToString());
            }
            foreach(Diagnostic info in result.Where(e => e.Severity == DiagnosticSeverity.Info))
            {
                Log.Information(info.ToString());
            }

            if(Array.Exists(result, e => e.Severity == DiagnosticSeverity.Error))
            {
                MessageBox.Show(
                    string.Format("{0}\nFound {1} error(s), check log for more information.", 
                    Application.Current.FindResource("CompileError"), 
                    result.Count(e => e.Severity == DiagnosticSeverity.Error)
                ));
                return false;
            }
            return true;
        }
        public static bool IgnoreCompletely(string name, string file)
        {
            string relPath = file.Substring(file.IndexOf("ModSources")).Replace("ModSources\\" + name + "\\", "");
            return relPath[0] == '.' ||
                relPath.StartsWith("bin" + Path.DirectorySeparatorChar) ||
                relPath.StartsWith("obj" + Path.DirectorySeparatorChar);
        }
    }
}
