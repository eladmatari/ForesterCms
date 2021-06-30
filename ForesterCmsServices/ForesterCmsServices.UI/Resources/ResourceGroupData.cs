using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public class ResourceGroupData
    {
        public ResourceGroupData()
        {
            UniqueId = CryptHelper.GetRandomStringAlphaNumeric(20);
        }

        public string Key { get; set; }
        public string[] Files { get; set; }
        public string UniqueId { get; private set; }

        public bool EqualsFiles(string[] files)
        {
            if (files.Length != Files.Length)
                return false;

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != Files[i])
                    return false;
            }

            return true;
        }
    }
}
