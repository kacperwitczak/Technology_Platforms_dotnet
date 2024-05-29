using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WPF_Lab4.Entities
{
    public class Engine : IComparable<Engine>
    {
        public double Displacement { get; set; }

        public double HorsePower { get; set; }

        public string Model { get; set; }

        public Engine(double displacement, double horsePower, string model)
        {
            this.Displacement = displacement;
            this.HorsePower = horsePower;
            this.Model = model;
        }

        public Engine()
        {
            this.Displacement = 0;
            this.HorsePower = 0;
            this.Model = "";
        }

        public int CompareTo(Engine other)
        {
            if (this.Displacement == other.Displacement)
            {
                if (this.HorsePower == other.HorsePower)
                {
                    return this.Model.CompareTo(other.Model);
                }
                return this.HorsePower.CompareTo(other.HorsePower);
            }
            return this.Displacement.CompareTo(other.Displacement);
        }
    }
}
