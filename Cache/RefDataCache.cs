using Microsoft.Extensions.Caching.Memory;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.APIModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using static Ofqual.Common.RegisterFrontend.Models.Constants;

namespace Ofqual.Common.RegisterFrontend.Cache
{
    public class RefDataCache : IRefDataCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRefDataAPIClient _refDataAPI;
        private readonly IRegisterAPIClient _registerAPI;

        private readonly TimeSpan ExpirySpan = DateTime.Today.AddDays(1).AddHours(5) - DateTime.Now;

        public RefDataCache(IMemoryCache memoryCache, IRefDataAPIClient refDataAPIClient, IRegisterAPIClient registerAPI)
        {
            _memoryCache = memoryCache;
            _refDataAPI = refDataAPIClient;
            _registerAPI = registerAPI;
        }

        public Task<List<AssessmentMethod>> GetAssessmentMethods()
        {
            var list = _memoryCache.GetOrCreateAsync(
                KEY_ASSESSMENT_METHODS,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = ExpirySpan;
                    return await _refDataAPI.GetAssessmentMethodsAsync();
                });

            return list!;
        }

        public Task<List<Level>> GetLevels()
        {
            var list = _memoryCache.GetOrCreateAsync(
                KEY_LEVELS,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = ExpirySpan;
                    return await _refDataAPI.GetLevelsAsync();
                });

            return list!;
        }

        public Task<List<QualificationType>> GetQualificationTypes()
        {
            var list = _memoryCache.GetOrCreateAsync(
                KEY_QUAL_TYPES,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = ExpirySpan;
                    return await _refDataAPI.GetQualificationTypesAsync();
                });

            return list!;
        }

        public Task<List<SSA>> GetSSA()
        {
            var list = _memoryCache.GetOrCreateAsync(
                KEY_SSA,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = ExpirySpan;
                    return await _refDataAPI.GetSSAAsync();
                });

            return list!;
        }


        Task<APIResponseList<OrganisationListViewModel>> IRefDataCache.GetOrganisations()
        {
            var list = _memoryCache.GetOrCreateAsync(
                            KEY_ORGS,
                            async cacheEntry =>
                            {
                                cacheEntry.AbsoluteExpirationRelativeToNow = ExpirySpan;
                                return await _registerAPI.GetOrganisationsListAsync(null, 1, 500);
                            });

            return list!;
        }
    }
}
