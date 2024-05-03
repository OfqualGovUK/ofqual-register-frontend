namespace Ofqual.Common.RegisterFrontend.Models.RegisterModels
{
    public class RecognitionScope
    {
        public required List<ScopeType> Inclusions { get; set; }
        public required List<ScopeType> Exclusions { get; set; }
    }

    public class ScopeType
    {
        public required string Type { get; set; }
        public required List<ScopeLevel> Levels { get; set; }
    }

    public class ScopeLevel
    {
        public required string Level { get; set; }
        public required List<string> Recognitions { get; set; }
    }
}
