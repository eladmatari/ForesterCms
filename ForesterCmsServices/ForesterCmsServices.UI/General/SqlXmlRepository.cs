using Common.Utils.Standard;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ForesterCmsServices.UI.General
{
    public class SqlXmlRepository : IXmlRepository
    {
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var dt = DBHelper.Database.ExecuteDataTable("select * from cms_xml_repository");

            var resultList = new List<XElement>();

            foreach (DataRow row in dt.Rows)
            {
                string xml = CryptHelper.TryDecrypt(row.Field<string>("Xml"), nameof(SqlXmlRepository));

                if (xml != null)
                    resultList.Add(XElement.Parse(xml));
            }

            return resultList.AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            if (string.IsNullOrWhiteSpace(friendlyName))
                throw new ArgumentNullException(nameof(friendlyName));

            if (element == null || element.IsEmpty)
                throw new ArgumentNullException(nameof(element));

            string xml = CryptHelper.Encrypt(element.ToString(SaveOptions.DisableFormatting), nameof(SqlXmlRepository));

            DBHelper.Database.ExecuteNonQuery(@"INSERT INTO cms_xml_repository (`key`, `xml`, `createdate`) VALUES(@friendlyName, @xml, curdate()) 
ON DUPLICATE KEY UPDATE xml = @xml", (cmd) =>
            {
                cmd.Parameters.AddWithValue("@friendlyName", friendlyName);
                cmd.Parameters.AddWithValue("@xml", xml);
            });
        }
    }
}
