using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("partId")]
    public class CarInputPartDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
