using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Output
{
    [XmlType("customers")]
    public class CustomerOutputDTO
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; }
        //  <customer full-name="Hai Everton" bought-cars="1" spent-money="2544.67" />

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
    }
}
