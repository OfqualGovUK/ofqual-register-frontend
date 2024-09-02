
using Ofqual.Common.RegisterFrontend.Cache;
using Ofqual.Common.RegisterFrontend.Extensions;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.Models.RegisterModels;
using Ofqual.Common.RegisterFrontend.Models.SearchViewModels;
using Ofqual.Common.RegisterFrontend.Models.RefDataModels;
using static Ofqual.Common.RegisterFrontend.Models.Constants;

namespace Ofqual.Common.RegisterFrontend.UseCases.Qualifications
{
    public class QualificationsUseCases : IQualificationsUseCases
    {
        private readonly IRefDataCache _refDataCache;

        public QualificationsUseCases(IRefDataCache refDataCache)
        {
            _refDataCache = refDataCache;
        }

        public Dictionary<string, string[]> CompareQuals(Qualification left, Qualification right)
        {
            var differing = new Dictionary<string, string[]>();

            DiffValues(left.Type, right.Type, QUAL_TYPE, ref differing);
            DiffValues(left.Level, right.Level, QUAL_LEVEL, ref differing);
            DiffValues(left.QualificationNumber, right.QualificationNumber, QUAL_NUMBER, ref differing);
            DiffValues(left.GLH.ToString(), right.GLH.ToString(), GUIDED_LEARNING_HRS, ref differing);
            DiffValues(left.TQT.ToString(), right.TQT.ToString(), TOTAL_QUAL_TIME, ref differing);

            var leftAssessmentMethods = left.AssessmentMethods == null ? null : string.Join("<br />", left.AssessmentMethods);
            var rightAssessmentMethods = right.AssessmentMethods == null ? null : string.Join("<br />", right.AssessmentMethods);

            DiffValues(leftAssessmentMethods, rightAssessmentMethods, ASSESSMENT_METHODS, ref differing);

            DiffValues(left.SSA, right.SSA, Constants.SSA, ref differing);
            DiffValues(left.GradingScale, right.GradingScale, GRADING_SCALE, ref differing);
            DiffValues($"{left.ApprenticeshipStandardTitle} ({left.ApprenticeshipStandardReferenceNumber})", $"{right.ApprenticeshipStandardTitle} ({right.ApprenticeshipStandardReferenceNumber})", END_POINT_ASSESSMENT_STD, ref differing);
            DiffValues(left.OrganisationName, right.OrganisationName, AWARDING_ORG, ref differing);
            DiffValues(left.GradingType, right.GradingType, GRADING_TYPE, ref differing);
            DiffValues(left.TotalCredits.ToString(), right.TotalCredits.ToString(), TOTAL_CREDITS, ref differing);
            DiffValues(left.Specialism, right.Specialism, SPECIALISMS, ref differing);
            DiffValues(left.Status, right.Status, STATUS, ref differing);

            //national availability
            var leftNationalAvailability = "";
            if (left.OfferedInEngland ?? false)
            {
                leftNationalAvailability += $"{ENGLAND}, ";
            }
            if (left.OfferedInNorthernIreland ?? false)
            {
                leftNationalAvailability += $"{NI}, ";
            }
            if (left.OfferedInternationally ?? false)
            {
                leftNationalAvailability += $"{INTERNATIONAL}";
            }

            var rightNationalAvailability = "";
            if (right.OfferedInEngland ?? false)
            {
                rightNationalAvailability += $"{ENGLAND}, ";
            }
            if (right.OfferedInNorthernIreland ?? false)
            {
                rightNationalAvailability += $"{NI}, ";
            }
            if (right.OfferedInternationally ?? false)
            {
                rightNationalAvailability += $"{INTERNATIONAL}";
            }

            DiffValues(leftNationalAvailability, rightNationalAvailability, NATIONAL_AVAILABILITY, ref differing);
            DiffValues(left.OfferedInNorthernIreland.ToString(), right.OfferedInNorthernIreland.ToString(), NI_AVIALABILITY, ref differing);

            DiffValues(left.RegulationStartDate.ToString("dd MMMM yyyy").StripLeadingZeros(), right.RegulationStartDate.ToString("dd MMMM yyyy").StripLeadingZeros(), REGULATION_START_DT, ref differing);
            DiffValues(left.OperationalStartDate.ToString("dd MMMM yyyy").StripLeadingZeros(), right.OperationalStartDate.ToString("dd MMMM yyyy").StripLeadingZeros(), OPERATIONAL_START_DT, ref differing);
            DiffValues(left.OperationalEndDate?.ToString("dd MMMM yyyy").StripLeadingZeros(), right.OperationalEndDate?.ToString("dd MMMM yyyy").StripLeadingZeros(), OPERATIONAL_END_DT, ref differing);
            DiffValues(left.CertificationEndDate?.ToString("dd MMMM yyyy").StripLeadingZeros(), right.CertificationEndDate?.ToString("dd MMMM yyyy").StripLeadingZeros(), CERT_END_DT, ref differing);
            DiffValues(left.EQFLevel, right.EQFLevel, EUROPEAN_QUAL_LEVEL, ref differing);
            DiffValues(left.Pathways, right.Pathways, OPTIONAL_ROUTES, ref differing);

            return differing;
        }

        public string CreatePagedFilters(string? title, string? availability, string? qualificationTypes, string? qualificationLevels, string? awardingOrganisations, string? sectorSubjectAreas, string? gradingTypes, string? assessmentMethods, string? nationalAvailability, int? minTotalQualificationTime, int? maxTotalQualificationTime, int? minGuidedLearninghours, int? maxGuidedLearninghours)
        {
            var pagingURL = "";

            AppendFilterToPaging(ref pagingURL, FILTER_NAME_AVAILABILITY, availability);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_QUAL_TYPES, qualificationTypes);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_QUAL_LEVELS, qualificationLevels);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_AWARDING_ORG, awardingOrganisations);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_SSA, sectorSubjectAreas);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_GRADING_TYPE, gradingTypes);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_ASSESSMENT_METHODS, assessmentMethods);
            AppendFilterToPaging(ref pagingURL, FILTER_NAME_NATIONAL_AVAILABILITY, nationalAvailability);

            if (minTotalQualificationTime != null)
            {
                pagingURL += $"&{FILTER_NAME_MIN_TQT}={minTotalQualificationTime}";
            }

            if (maxTotalQualificationTime != null)
            {
                pagingURL += $"&{FILTER_NAME_MAX_TQT}={maxTotalQualificationTime}";
            }

            if (minGuidedLearninghours != null)
            {
                pagingURL += $"&{FILTER_NAME_MIN_GLH}={minGuidedLearninghours}";
            }

            if (maxGuidedLearninghours != null)
            {
                pagingURL += $"&{FILTER_NAME_MAX_GLH}={maxGuidedLearninghours}";
            }

            return pagingURL;
        }

        public async Task<QualificationFilterModel> GetFilterValues()
        {
            List<SSA> ssa = await _refDataCache.GetSSA();

            List<Level> levels = await _refDataCache.GetLevels();

            List<AssessmentMethod> assessMethods = await _refDataCache.GetAssessmentMethods();

            List<QualificationType> types = await _refDataCache.GetQualificationTypes();

            APIResponseList<OrganisationListViewModel> organisations = await _refDataCache.GetOrganisations();

            return new QualificationFilterModel
            {
                AssessmentMethods = assessMethods.OrderBy(e => e.Description).Select(e => e.Description).ToHashSet(),
                QualificationLevels = levels.OrderBy(e => e.LevelDescription).Select(e => e.LevelDescription).ToHashSet(),
                QualificationTypes = types.OrderBy(e => e.Description).Select(e => e.Description).ToHashSet(),
                SSA = ssa.OrderBy(e => e.SsaDescription2).Select(e => e.SsaDescription2).ToHashSet(),
                Organisations = organisations.Results!.OrderBy(e => e.Name).Select(e => e.Name).ToHashSet()
            };
        }


        // Helper method to append filter to the paging URL if the filter value is not null or empty
        void AppendFilterToPaging(ref string url, string paramName, string? param)
        {
            if (param != null && param.GetSubStrings() != null)
            {
                url += $"&{paramName.ToURL()}={param.ToURL()}";
            }
        }

        // Helper method to compare and add differing fields in the quals compare
        private static void DiffValues(string? left, string? right, string fieldName, ref Dictionary<string, string[]> differing)
        {
            left = string.IsNullOrEmpty(left) ? null : left;
            right = string.IsNullOrEmpty(right) ? null : right;

            if (left != right)
            {
                differing.Add(fieldName, [left ?? "-", right ?? "-"]);
            }
        }
    }
}
