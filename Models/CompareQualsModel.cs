using Ofqual.Common.RegisterFrontend.Models.APIModels;

namespace Ofqual.Common.RegisterFrontend.Models
{
    public class CompareQualsModel
    {
        public string SelectedQuals { get; set; }
        public string UnselectedQuals { get; set; }

        public Qualification Left { get; set; }
        public Qualification Right { get; set; }

        public Dictionary<string, string[]> Differing { get; set; }
    }
}
