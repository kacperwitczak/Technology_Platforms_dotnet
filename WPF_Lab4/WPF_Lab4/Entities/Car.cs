using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WPF_Lab4.Entities
{
    public class Car
    {
        public string Model { get; set; }

        public Engine Motor { get; set; }

        public int Year { get; set; }

        public Car(string model, Engine engine, int year)
        {
            this.Model = model;
            this.Motor = engine;
            this.Year = year;
        }

        public Car()
        {
            this.Model = "";
            this.Motor = new Engine();
            this.Year = 0;
        }
    }
}
