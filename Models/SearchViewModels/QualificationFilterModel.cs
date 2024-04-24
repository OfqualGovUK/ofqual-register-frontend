namespace Ofqual.Common.RegisterFrontend.Models.SearchViewModels
{
    public class QualificationFilterModel : FilterModel
    {
        public IEnumerable<string>? SSA { get; set; }
        public IEnumerable<string>? QualificationTypes { get; set; }
        public IEnumerable<string>? AssessmentMethods { get; set; }
        public IEnumerable<string>? QualificationLevels { get; set; }
        public IEnumerable<string>? Organisations { get; set; }
    }
}
