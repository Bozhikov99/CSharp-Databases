using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("Sale")]
    public class SaleInputDTO
    {
        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("carId")]
        public int CarId { get; set; }

        [XmlElement("customerId")]
        public int CustomerId { get; set; }
    }
}
