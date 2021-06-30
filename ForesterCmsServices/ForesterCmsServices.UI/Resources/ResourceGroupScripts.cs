using Common.Utils.Standard;
using NUglify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public class ResourceGroupScripts : ResourceGroupBase
    {
        internal ResourceGroupScripts() { }

        protected override string FilesPath { get { return "scripts"; } }

        protected override string OutputFileExstension { get { return ".js"; } }

        protected override void WriteFiles(ref string fileText, string newFilePath, string newFileMinPath, string fileName)
        {
            FileSystemHelper.RemoveReadOnly(newFilePath);
            FileSystemHelper.RemoveReadOnly(newFileMinPath);

            using (var tw = new StreamWriter(newFilePath, false, Encoding.UTF8))
            {
                tw.Write(fileText);
            }

            using (var tw = new StreamWriter(newFileMinPath, false, Encoding.UTF8))
            {
                var res = Uglify.Js(fileText);
                tw.Write(res);
            }
        }
    }
}
