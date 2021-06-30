using Common.Utils;
using Common.Utils.Standard;
using LibSassHost;
using NUglify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public class ResourceGroupCss : ResourceGroupBase
    {
        internal ResourceGroupCss() { }

        protected override string FilesPath { get { return "themes"; } }

        protected override string OutputFileExstension { get { return ".css"; } }

        protected override string Process(string s)
        {
            s = s.Replace("~/", "../");

            return s;
        }

        protected override void WriteFiles(ref string fileText, string newFilePath, string newFileMinPath, string fileName)
        {
            FileSystemHelper.RemoveReadOnly(newFilePath);
            FileSystemHelper.RemoveReadOnly(newFileMinPath);

            string scssFilePath = newFilePath.Replace(".css", ".scss");

            using (var tw = new StreamWriter(scssFilePath, false, Encoding.UTF8))
            {
                tw.Write(fileText);
            }

            var options = new CompilationOptions { SourceMap = true };
            options.IncludePaths.Add(Path.Combine(DiHelper.Environment.WebRootPath, FilesPath));
            CompilationResult result = SassCompiler.CompileFile(scssFilePath, options: options);

            using (var tw = new StreamWriter(newFilePath + ".map", false))
            {
                tw.Write(result.SourceMap);
            }

            using (var tw = new StreamWriter(newFilePath, false, Encoding.UTF8))
            {
                tw.Write(result.CompiledContent);
            }

            using (var tw = new StreamWriter(newFileMinPath, false, Encoding.UTF8))
            {
                tw.Write(Uglify.Css(result.CompiledContent));
            }
        }
    }
}
