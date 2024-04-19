using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Refit;
using System.Net;

namespace Ofqual.Common.RegisterFrontend.RegisterAPI
{
    public interface IRegisterAPIClient
    {
        [Get("/api/organisations/{recognitionNumber}")]
        Task<Organisation> GetOrganisationAsync(string recognitionNumber);

        [Get("/api/organisations?search={search}&page={page}&limit={limit}")]
        Task<APIResponseList<OrganisationListViewModel>> GetOrganisationListAsync(string? search, int page, int limit);
    }
}
