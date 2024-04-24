using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Refit;
using System.Net;

namespace Ofqual.Common.RegisterFrontend.RegisterAPI
{
    public interface IRefDataAPIClient
    {
        [Get("/qualificationtypes")]
        Task<List<QualificationType>> GetQualificationTypesAsync();

        [Get("/levels")]
        Task<List<Level>> GetLevelsAsync();

        [Get("/sectorsubjectareas")]
        Task<List<SSA>> GetSSAAsync();

        [Get("/assessmentmethods")]
        Task<List<AssessmentMethod>> GetAssessmentMethodsAsync();
    }
}
