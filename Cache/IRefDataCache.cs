using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;

namespace Ofqual.Common.RegisterFrontend.Cache
{
    public interface IRefDataCache
    {
        Task<List<QualificationType>> GetQualificationTypes();
        Task<List<Level>> GetLevels();
        Task<List<SSA>> GetSSA();
        Task<List<AssessmentMethod>> GetAssessmentMethods();
        Task<APIResponseList<OrganisationListViewModel>> GetOrganisations();
    }
}
