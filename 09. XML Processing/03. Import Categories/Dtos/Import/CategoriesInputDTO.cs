using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("Category")]
    public class CategoriesInputDTO
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }
}
