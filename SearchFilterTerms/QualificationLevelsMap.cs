namespace Ofqual.Common.RegisterFrontend.SearchFilterTerms
{
    public class QualificationLevelsMap
    {
        public static readonly List<(string, string[])> QualificationLevelMapValues =
        [
            ("ENTRY 1", ["Entry 1", "Entry 1,2,3"]),
            ("ENTRY 2", ["Entry 2", "Entry 2,3", "Entry 2,3", "Entry 1,2,3"]),
            ("ENTRY 3", ["Entry 3", "Entry 2,3", "Entry 1,2,3"]),
            ("ENTRY 2 3", ["Entry 2,3"]  ),
            ("ENTRY 23", ["Entry 2,3"]),
            ("ENTRY 2,3", ["Entry 2,3"]),
            ("ENTRY 1 2 3", ["Entry 1,2,3"]),
            ("ENTRY 123", ["Entry 1,2,3"]),
            ("ENTRY 1,2,3",  ["Entry 1,2,3"]),
            ("ENTRY",  ["Entry 1", "Entry 2", "Entry 3", "Entry 2,3", "Entry 1,2,3"]),
            ("ENTRY LEVEL",  ["Entry 1", "Entry 2", "Entry 3", "Entry 2,3", "Entry 1,2,3"]),
            ("LEVEL 1", ["Level 1", "Level 1/2"]),
            ("LEVEL 1 2", ["Level 1/2"]),
            ("LEVEL 12", ["Level 1/2"]),
            ("LEVEL 1,2", ["Level 1/2"]),
            ("LEVEL 1/2", ["Level 1/2"]),
            ("LEVEL 1\\2", ["Level 1/2"]),
            ("LEVEL 2", ["Level 2", "Level 1/2"]),
            ("LEVEL 3", ["Level 3"]),
            ("LEVEL 4", ["Level 4"]),
            ("LEVEL 5", ["Level 5"]),
            ("LEVEL 6", ["Level 6"]),
            ("LEVEL 7", ["Level 7"]),
            ("LEVEL 8", ["Level 8"])
         ];
    }
}
