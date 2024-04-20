using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LINQ_XML_XPATH_XHTML.Entities
{
    [XmlRoot("cars")]
    [XmlType("car")]
    [Serializable]
    public class Car
    {
        public string model { get; set; }

        [XmlElement("engine")]
        public Engine motor { get; set; }
        public int year { get; set; }

        public Car(string model, Engine engine, int year)
        {
            this.model = model;
            this.motor = engine;
            this.year = year;
        }

        public Car()
        {
            this.model = "";
            this.motor = new Engine();
            this.year = 0;
        }
    }
}
