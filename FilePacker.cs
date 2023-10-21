using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ModShardLauncher.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModShardLauncher
{
    public class FilePacker
    {
        public static void Pack(string path)
        {
            var dir = new DirectoryInfo(path);
            var textures = dir.GetFiles("*.png", SearchOption.AllDirectories).ToList();
            var scripts = dir.GetFiles("*.lua", SearchOption.AllDirectories).ToList();
            FileStream fs = new FileStream(Path.Join(ModLoader.ModPath, dir.Name + ".sml"), FileMode.Create);
            Write(fs, "MSLM");
            var version = DataLoader.GetVersion();
            Write(fs, version);
            Write(fs, textures.Count);
            int offset = 0;
            foreach (var tex in textures)
            {
                var name = dir.Name + tex.FullName.Replace(path, "");
                Write(fs, name.Length);
                Write(fs, name);
                Write(fs, offset);
                var len = CalculateBytesLength(tex);
                Write(fs, len);
                offset += len;
            }
            Write(fs, scripts.Count);
            foreach (var scr in scripts)
            {
                var name = dir.Name + scr.FullName.Replace(path, "");
                Write(fs, name.Length);
                Write(fs, name);
                Write(fs, offset);
                var len = CalculateBytesLength(scr);
                Write(fs, len);
                offset += len;
            }
            foreach (var tex in textures)
                Write(fs, tex);
            foreach (var scr in scripts)
                Write(fs, scr);
            CompileMod(dir.Name, path, out var code, out _);
            Write(fs, code.Length);
            Write(fs, code);
            fs.Close();
        }
        public static void Write(FileStream fs, object obj)
        {
            var type = obj.GetType();
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
                var stream = new FileStream((obj as FileInfo).FullName, FileMode.Open);
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
            var stream = new FileStream(f.FullName, FileMode.Open);
            var len = (int)stream.Length;
            stream.Close();
            return len;
        }
        private static string refPath(string name)
        {
            return Path.Join(Environment.CurrentDirectory, name);
        }
        public static Diagnostic[] RoslynCompile(string name, string[] files, string[] preprocessorSymbols, out byte[] code, out byte[] pdb)
        {
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: false);

            var parseOptions = new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);
            var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);
            IEnumerable<MetadataReference> DefaultReferences =
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),//mscorlib.dll
                    MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),//System.dll
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(refPath("System.Collections.dll")),//System.Core.dll
                    MetadataReference.CreateFromFile(refPath("ModShardLauncher.dll")),
                    MetadataReference.CreateFromFile(refPath("System.Runtime.dll")),
                    MetadataReference.CreateFromFile(refPath("netstandard.dll"))
                };
            var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, f, Encoding.UTF8));
            var comp = CSharpCompilation.Create(name, src, DefaultReferences, options);

            using var peStream = new MemoryStream();
            using var pdbStream = new MemoryStream();

            var results = comp.Emit(peStream, pdbStream, options: emitOptions);

            code = peStream.ToArray();
            pdb = pdbStream.ToArray();

            return results.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).ToArray();
        }
        public static void CompileMod(string name, string path, out byte[] code, out byte[] pdb)
        {
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Where(file => !IgnoreCompletely(name, file)).ToArray();
            var preprocessorSymbols = new List<string>() { "FNA" };
            var result = RoslynCompile(name, files, preprocessorSymbols.ToArray(), out code, out pdb);
        }
        public static bool IgnoreCompletely(string name, string file)
        {
            var relPath = file.Substring(file.IndexOf("ModSources")).Replace("ModSources\\" + name + "\\", "");
            return relPath[0] == '.' ||
                relPath.StartsWith("bin" + Path.DirectorySeparatorChar) ||
                relPath.StartsWith("obj" + Path.DirectorySeparatorChar);
        }
    }
}
