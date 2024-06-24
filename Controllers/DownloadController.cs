using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.Globalization;
using System.Net;
using Ofqual.Common.RegisterFrontend.Models.FullDataSetCSV;
using CsvHelper;
using Ofqual.Common.RegisterFrontend.BlobStorage;
using static Ofqual.Common.RegisterFrontend.Models.Constants;
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

            //check if blob exists first
            if (_blobService.BlobExists(BLOBNAME_ORGANISATIONS))
            {
                var properties = await _blobService.BlobProperties(BLOBNAME_ORGANISATIONS);

                //check if the blob is newer than 24 hrs
                if ((DateTime.Now - properties.CreatedOn).TotalHours > 24)
                {
                    await FetchOrganisationsUploadBlob();                    
                }
            }
            else
            {
                try
                {
                    await FetchOrganisationsUploadBlob();
                }
                catch (ApiException ex)
                {
                    return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode(500);
                }
            }

            //download blob to CSV 
            var memoryStream = await _blobService.DownloadBlob(BLOBNAME_ORGANISATIONS);

            return File(memoryStream.ToArray(), "text/csv", fileName);
        }

        private async Task FetchOrganisationsUploadBlob()
        {
            APIResponseList<OrganisationCSV> orgs = await _registerAPIClient.GetFullOrganisationsDataSetAsync();

            var stream = CreateCSVStream(orgs.Results!);
            stream.Position = 0;

            await _blobService.UploadBlob(BLOBNAME_ORGANISATIONS, stream);
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
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords((System.Collections.IEnumerable)results);

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
