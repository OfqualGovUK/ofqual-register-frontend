using CsvHelper.Configuration.Attributes;

namespace Ofqual.Common.RegisterFrontend.Models.FullDataSetCSV
{
    public class OrganisationCSV
    {
        [Name("Recognition Number")]
        public required string RecognitionNumber { get; set; }

        public required string Name { get; set; }

        [Name("Legal Name")]
        public required string LegalName { get; set; }

        public required string Acronym { get; set; }

        [Name("Email")]
        public required string ContactEmail { get; set; }

        [Name("Website")]
        public string? Website { get; set; }

        [Name("Head Office Address Line 1")]
        public string? AddressLine1 { get; set; }

        [Name("Head Office Address Line 2")]
        public string? AddressLine2 { get; set; }

        [Name("Head Office Address Town / City")]
        public string? AddressCity { get; set; }

        [Name("Head Office Address County")]
        public string? AddressCounty { get; set; }

        [Name("Head Office Address Postcode")]
        public string? AddressPostCode { get; set; }

        [Name("Head Office Address Country")]
        public string? AddressCountry { get; set; }

        [Name("Head Office Address Telephone Number")]
        public string? PhoneNumber { get; set; }

        [Name("Ofqual Status")]
        public string? OfqualOrganisationStatus { get; set; }

        [Name("Ofqual Recognised From")]
        public DateTime? OfqualRecognisedOn { get; set; }

        [Name("Ofqual Recognised To")]
        public DateTime? OfqualRecognisedTo { get; set; }

        [Name("CCEA Status")]
        public string? CceaOrganisationStatus { get; set; }

        [Name("CCEA Recognised From")]
        public DateTime? CceaRecognisedOn { get; set; }

        [Name("CCEA Recognised To")]
        public DateTime? CceaRecognisedTo { get; set; }
    }
}
