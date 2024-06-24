using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Models.RegisterModels;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using Ofqual.Common.RegisterFrontend.Models.FullDataSetCSV;
using CsvHelper;
using Ofqual.Common.RegisterFrontend.BlobStorage;
using System.IO;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    [Route("download")]
    public class DownloadController : Controller
    {
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IBlobService _blobService;

        public DownloadController(IRegisterAPIClient registerAPIClient, IBlobService blobService)
        {
            _registerAPIClient = registerAPIClient;
            _blobService = blobService;
        }

        [HttpGet]
        [Route("organisations")]
        public async Task<IActionResult> Organisations()
        {
            string fileName = $"Organisations_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];
            try
            {
                APIResponseList<OrganisationCSV> orgs;
                orgs = await _registerAPIClient.GetFullOrganisationsDataSetAsync();

                var stream =  CreateCSVStream(orgs.Results!);

                await _blobService.UploadBlob("Orgs", stream);

                return File(stream.ToArray(), "text/csv", fileName);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }
        }

        [HttpGet]
        [Route("qualifications")]
        public async Task<IActionResult> Qualifications()
        {
            string fileName = $"Qualifications_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";
            byte[] fileBytes = [];
            try
            {
                APIResponseList<QualificationCSV> quals;
                quals = await _registerAPIClient.GetFullQualificationsDataSetAsync();

                return CreateCSV(fileName, quals.Results!);
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
            }
        }



        private MemoryStream CreateCSVStream(object results)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords((System.Collections.IEnumerable)results);
            }

            return memoryStream;
        }

        private FileContentResult CreateCSV(string fileName, object results)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords((System.Collections.IEnumerable)results);
            }

            return File(memoryStream.ToArray(), "text/csv", fileName);
        }

    }
}
