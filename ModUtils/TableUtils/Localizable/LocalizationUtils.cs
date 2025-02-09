using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class Msl
{
    public class LocalizedStrings
    {
        public string Russian { get; }
        public string English { get; }
        public string Chinese { get; }
        public string German { get; }
        public string SpanishLatam { get; }
        public string French { get; }
        public string Italian { get; }
        public string Portuguese { get; }
        public string Polish { get; }
        public string Turkish { get; }
        public string Japanese { get; }
        public string Korean { get; }
        public string Czech { get; }
        public string SpanishSpain { get; }

        // Constructor assumes empty translations are the same as English
        public LocalizedStrings(
            string english,
            string? russian = null,
            string? chinese = null,
            string? german = null,
            string? spanishLatam = null,
            string? french = null,
            string? italian = null,
            string? portuguese = null,
            string? polish = null,
            string? turkish = null,
            string? japanese = null,
            string? korean = null,
            string? czech = null,
            string? spanishSpain = null
            )
        {
            Russian = russian ?? english;
            English = english;
            Chinese = chinese ?? english;
            German = german ?? english;
            SpanishLatam = spanishLatam ?? english;
            French = french ?? english;
            Italian = italian ?? english;
            Portuguese = portuguese ?? english;
            Polish = polish ?? english;
            Turkish = turkish ?? english;
            Japanese = japanese ?? english;
            Korean = korean ?? english;
            Czech = czech ?? english;
            SpanishSpain = spanishSpain ?? english;
        }
    }
}