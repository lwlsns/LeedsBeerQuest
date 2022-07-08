using System;
using System.Collections.Generic;

namespace LeedsBeerQuest
{
   public class Location
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }
    public class Venue
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }
        public string Excerpt { get; set; }
        public string Thumbnail { get; set; }
        public Location Location { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public double Stars_beer { get; set; }
        public double Stars_atmosphere { get; set; }
        public double Stars_amenities { get; set; }
        public double Stars_value { get; set; }
        public string Tags { get; set; }

    }
}