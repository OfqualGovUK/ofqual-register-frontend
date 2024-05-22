namespace Ofqual.Common.RegisterFrontend.Models
{
    public class QualificationsSearchFiltersModel
    {
        public readonly string? title;
        public readonly int page = 1;
        public readonly string? assessmentMethods = null;
        public readonly string? gradingTypes = null;
        public readonly string? awardingOrganisations = null;
        public string? availability = null;
        public readonly string? qualificationTypes = null;
        public readonly string? qualificationLevels = null;
        public readonly string? nationalAvailability = null;
        public readonly string? sectorSubjectAreas = null;
        public readonly int? minTotalQualificationTime = null;
        public readonly int? maxTotalQualificationTime = null;
        public readonly int? minGuidedLearninghours = null;
        public readonly int? maxGuidedLearninghours = null;
    }
}
