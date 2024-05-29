using LINQ_XML_XPATH_XHTML.Entities;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;


//xml manipulation source https://www.c-sharpcorner.com/article/xml-manipulation-in-c-sharp/

namespace LINQ_XML_XPATH_XHTML
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Car> myCars = new List<Car>(){
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };

            //query1
            var cars_1 = myCars.Where(c => c.model == "A6").Select(c => new
            {
                engineType = c.motor.model == "TDI" ? "diesel" : "petrol",
                hppl = c.motor.horsePower / c.motor.displacement
            });

            foreach (var car in cars_1)
            {
                Console.WriteLine("engine: " + car.engineType + " hppl: " + car.hppl);
            }

            //query2
            var grouped_cars = cars_1.GroupBy( c => c.engineType).Select( g => new
            {
                engineType = g.Key,
                avgHppl = g.Average(c => c.hppl)
            });

            foreach (var group in grouped_cars)
            {
                Console.WriteLine(group.engineType + ": " + group.avgHppl);
            }

            SerializeCars(myCars, "CarsCollection.xml");

            List<Car> cars_deserialized = DeserializeCars("CarsCollection.xml");

            foreach (var car in cars_deserialized)
            {
                Console.WriteLine(car.model + " " + car.motor.model + " " + car.motor.horsePower + " " + car.motor.displacement + " " + car.year);
            }


            double avgHP = CalculateAverageHorsePower("CarsCollection.xml");
            Console.WriteLine("Average HP: " + avgHP);

            IEnumerable<XElement> models = GetUniqueCarElements("CarsCollection.xml");
            foreach (var model in models)
            {
                Console.WriteLine(model.Value);
            }

            CreateXmlFromLinq(myCars, "CarsFromLinq.xml");

            CreateHtmlTable(myCars, "template.html");

            ModifyCarsXml("CarsCollection.xml", "ModifiedCarsCollection.xml");
        }


        //https://stackoverflow.com/questions/17043663/how-to-serialize-a-listt-into-xml
        private static void CreateXmlFromLinq(List<Car> myCars, string path)
        {
            IEnumerable<XElement> nodes = myCars.Select(c => new XElement("car",
                new XElement("model", c.model),
                new XElement("engine",
                    new XAttribute("model", c.motor.model),
                    new XElement("horsePower", c.motor.horsePower),
                    new XElement("displacement", c.motor.displacement)
                    ),
                new XElement("year", c.year)
                ));
            XElement rootNode = new XElement("cars", nodes); // stwórz węzeł zawierający wyniki zapytania
            rootNode.Save(path);
        }

        private static void ModifyCarsXml(string source, string destination)
        {
            XElement carsXml = XElement.Load(source);

            carsXml.Descendants("horsePower").ToList().ForEach(d => d.Name = "hp");

            carsXml.Descendants("car").ToList().ForEach(c =>
            {
                c.Element("model").SetAttributeValue("year", c.Element("year").Value);
                c.Element("year").Remove();
            });

            carsXml.Save(destination);
        }

        private static void CreateHtmlTable(List<Car> myCars, string path)
        {
            XElement table = new XElement("table",
                new XElement("tr",
                    new XElement("th", "Model"),
                    new XElement("th", "Motor"),
                    new XElement("th", "Displacement"),
                    new XElement("th", "HorsePower"),
                    new XElement("th", "Year")
                ),
                myCars.Select(c => new XElement("tr",
                    new XElement("td", c.model),
                    new XElement("td", c.motor.model),
                    new XElement("td", c.motor.displacement),
                    new XElement("td", c.motor.horsePower),
                    new XElement("td", c.year)
                ))
            );
            table.Save(path);
        }


        //choose only those models which are not repeated later in the xml
        //<A>1</A> wont be chosen as it is repeated later
        //<A>2</A>
        //<A>1</A>
        //. means current node
        private static IEnumerable<XElement> GetUniqueCarElements(string path)
        {
            XElement rootNode = XElement.Load(path);
            IEnumerable<XElement> models = rootNode.XPathSelectElements("/car/model[not(. = following::model)]");

            return models;
        }


        //https://www.w3schools.com/xml/xml_xpath.asp
        //[] means condition
        private static double CalculateAverageHorsePower(string path)
        {
            XElement rootNode = XElement.Load(path);
            double avgHP = (double)rootNode.XPathEvaluate("sum(/car[engine/@model != 'TDI']/engine/horsePower) div count(/car[engine/@model != 'TDI'])");
            
            return avgHP;
        }

        //https://code-maze.com/csharp-xml-serialization/
        private static void SerializeCars(List<Car> myCars, string path)
        {
            var serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, myCars);
            }
        }

        private static List<Car> DeserializeCars(string path)
        {
            var serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            using (TextReader reader = new StreamReader(path))
            {
                return (List<Car>)serializer.Deserialize(reader);
            }
        }
    }
}
