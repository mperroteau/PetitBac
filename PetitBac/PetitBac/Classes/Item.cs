using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac.Classes
{
    class Item
    {
        private string name;
        public int line;
        public int column;

        public Item(string _name, int _line, int _column)
        {
            this.name = _name;
            this.line = _line;
            this.column = _column;
        }

        public string getName()
        {
            return this.name;
        }
    }
}
