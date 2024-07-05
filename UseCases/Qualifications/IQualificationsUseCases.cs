using Ofqual.Common.RegisterFrontend.Models.RegisterModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;

namespace Ofqual.Common.RegisterFrontend.UseCases.Qualifications
{
    public interface IQualificationsUseCases
    {
        Dictionary<string, string[]> CompareQuals(Qualification leftRef, Qualification rightRef);
        string CreatePagedFilters(string? title, string? availability, string? qualificationTypes, string? qualificationLevels, string? awardingOrganisations, string? sectorSubjectAreas, string? gradingTypes, string? assessmentMethods, string? nationalAvailability, int? minTotalQualificationTime, int? maxTotalQualificationTime, int? minGuidedLearninghours, int? maxGuidedLearninghours);

        Task<QualificationFilterModel> GetFilterValues();
    }
}
