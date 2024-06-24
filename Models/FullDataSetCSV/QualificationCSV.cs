using CsvHelper.Configuration.Attributes;

namespace Ofqual.Common.RegisterFrontend.Models.FullDataSetCSV
{
    public class QualificationCSV
    {
        [Name("Qualification Number")]
        public required string QualificationNumber { get; set; }
        [Name("Qualification Title")]
        public required string Title { get; set; }
        [Name("Owner Organisation Recognition Number")]
        public required string OrganisationRecognitionNumber { get; set; }
        [Name("Owner Organisation Name")]
        public required string OrganisationName { get; set; }
        [Name("Owner Organisation Acronym")]
        public required string OrganisationAcronym { get; set; }
        [Name("Qualification Level")]
        public string? Level { get; set; }
        [Name("Qualification Sub Level")]
        public string? SubLevel { get; set; }
        [Name("EQF Level")]
        public string? EQFLevel { get; set; }
        [Name("Qualification Type")]
        public string? Type { get; set; }
        [Name("Total Credits")]
        public int? TotalCredits { get; set; }
        [Name("Qualification SSA")]
        public string? SSA { get; set; }
        [Name("Qualification Status")]
        public string? Status { get; set; }
        [Name("Regulation Start Date")]
        public DateTime RegulationStartDate { get; set; }
        [Name("Operational Start Date")]
        public DateTime OperationalStartDate { get; set; }
        [Name("Operational End Date")]
        public DateTime? OperationalEndDate { get; set; }
        [Name("Certification End Date")]
        public DateTime? CertificationEndDate { get; set; }
        [Name("Minimum Guided Learning Hours")]
        public int? MinimumGLH { get; set; }
        [Name("Maximum Guided Learning Hours")]
        public int? MaximumGLH { get; set; }
        [Name("Total Qualification Time")]
        public int? TQT { get; set; }
        [Name("Guided Learning Hours")]
        public int? GLH { get; set; }
        [Name("Offered In England")]
        public bool? OfferedInEngland { get; set; }
        [Name("Offered In Northern Ireland")]
        public bool? OfferedInNorthernIreland { get; set; }
        [Name("Overall Grading Type")]
        public string? GradingType { get; set; }
        [Name("Assessment Methods")]
        public string[]? AssessmentMethods { get; set; }
        [Name("NI Discount Code")]
        public string? NIDiscountCode { get; set; }
        [Name("GCE Size Equivalence")]
        public decimal? GCESizeEquivalence { get; set; }
        [Name("GCSE Size Equivalence")]
        public decimal? GCSESizeEquivalence { get; set; }
        [Name("Entitlement Framework Designation")]
        public string? EntitlementFrameworkDesignation { get; set; }
        [Name("Grading Scale")]
        public string? GradingScale { get; set; }
        public string? Specialism { get; set; }
        public string? Pathways { get; set; }
        [Name("Approved For DEL Funded Programme")]
        public bool? ApprovedForDELFundedProgramme { get; set; }
        [Name("Link To Specification")]
        public string? LinkToSpecification { get; set; }
        [Name("Currently and / or will consider offering internationally")]
        public bool? OfferedInternationally { get; set; }
    }
}
