﻿using System.ComponentModel.DataAnnotations;

namespace QuickCrossword.Model
{
    public class WordAndClue
    {
        public uint Id { get; set; }
        public string? Word { get; set; }
        public string? Clue { get; set; }
    }
}
