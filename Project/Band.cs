using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    class Band
    {

        private string Name { set; get; }
        private Type Genre { set; get; }
        private int Year { set; get; }
        private int QuantityOfSongs { set; get; }

        public enum Type
        {
            Rock,
            Classic,
            Jazz,
            Pop,
            Electronic
        }

        public Band(string Name, Type Genre, int Year, int QuantityOfSongs)
        {
            this.Genre = Genre;
            this.Name = Name;
            this.Year = Year;
            this.QuantityOfSongs = QuantityOfSongs;
        }
    }
}
