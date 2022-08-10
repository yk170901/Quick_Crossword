using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCrossword.Model
{
    public class WordDetail
    {
        public short Index { get; set; }
        public string? Word { get; set; }
        public string? Clue { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Isolated { get; set; }
        public Direction WordDirection { get; set; }
    }

    public enum Direction { Horizontal, Vertical };
}
