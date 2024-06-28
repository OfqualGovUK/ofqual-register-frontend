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
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;

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

            //check if blob exists first
            if (_blobService.BlobExists(BLOBNAME_ORGANISATIONS))
            {
                var properties = await _blobService.BlobProperties(BLOBNAME_ORGANISATIONS);

                //check if the blob is newer than 24 hrs
                if (DateTime.Now > properties.LastModified.AddHours(24))
                {
                    try
                    {
                        await FetchUploadOrganisationsFullDataset();
                    }
                    catch (ApiException ex)
                    {
                        return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                    }
                }
            }
            else
            {
                try
                {
                    await FetchUploadOrganisationsFullDataset();
                }
                catch (ApiException ex)
                {
                    return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                }
            }

            //download blob to CSV 
            var memoryStream = await _blobService.DownloadBlob(BLOBNAME_ORGANISATIONS);

            return File(memoryStream.ToArray(), "text/csv", fileName);
        }


        [HttpGet]
        [Route("qualifications")]
        public async Task<IActionResult> Qualifications()
        {
            string fileName = $"Qualifications_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";

            //check if blob exists first
            if (_blobService.BlobExists(BLOBNAME_QUALIFICATIONS))
            {
                var properties = await _blobService.BlobProperties(BLOBNAME_QUALIFICATIONS);

                //check if the blob is newer than 24 hrs
                if (DateTime.Now > properties.LastModified.AddHours(24))
                {
                    await FetchUploadQualificationsFullDataSet();
                }
            }
            else
            {
                try
                {
                    await FetchUploadQualificationsFullDataSet();
                }
                catch (ApiException ex)
                {
                    return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                }
            }

            //download blob to CSV 
            var memoryStream = await _blobService.DownloadBlob(BLOBNAME_QUALIFICATIONS);

            return File(memoryStream.ToArray(), "text/csv", fileName);
        }


        [HttpGet]
        [Route("qualificationsparallel")]
        public async Task<IActionResult> QualificationsParallel()
        {
            string fileName = $"Qualifications_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";

            //check if blob exists first
            if (_blobService.BlobExists(BLOBNAME_QUALIFICATIONS))
            {
                var properties = await _blobService.BlobProperties(BLOBNAME_QUALIFICATIONS);

                //check if the blob is newer than 24 hrs
                if (DateTime.Now > properties.LastModified.AddHours(24))
                {
                    await FetchUploadQualificationsFullDataSetParallel();
                }
            }
            else
            {
                try
                {
                    await FetchUploadQualificationsFullDataSetParallel();
                }
                catch (ApiException ex)
                {
                    return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                }
            }

            //download blob to CSV 
            var memoryStream = await _blobService.DownloadBlob(BLOBNAME_QUALIFICATIONS);

            return File(memoryStream.ToArray(), "text/csv", fileName);
        }

        [HttpGet]
        [Route("qualificationstask")]
        public async Task<IActionResult> QualificationsTask()
        {
            string fileName = $"Qualifications_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.csv";

            //check if blob exists first
            if (_blobService.BlobExists(BLOBNAME_QUALIFICATIONS))
            {
                var properties = await _blobService.BlobProperties(BLOBNAME_QUALIFICATIONS);

                //check if the blob is newer than 24 hrs
                if (DateTime.Now > properties.LastModified.AddHours(24))
                {
                    await FetchUploadQualificationsFullDataSetTask();
                }
            }
            else
            {
                try
                {
                    await FetchUploadQualificationsFullDataSetTask();
                }
                catch (ApiException ex)
                {
                    return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
                }
            }

            //download blob to CSV 
            var memoryStream = await _blobService.DownloadBlob(BLOBNAME_QUALIFICATIONS);

            return File(memoryStream.ToArray(), "text/csv", fileName);
        }

        #region Helper Methods
        private MemoryStream CreateCSVStream(object results)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords((System.Collections.IEnumerable)results);

            return memoryStream;
        }

        private async Task FetchUploadOrganisationsFullDataset()
        {
            APIResponseList<OrganisationCSV> orgs = await _registerAPIClient.GetFullOrganisationsDataSetAsync();

            var stream = CreateCSVStream(orgs.Results!);
            stream.Position = 0;

            await _blobService.UploadBlob(BLOBNAME_ORGANISATIONS, stream);
        }

        private async Task FetchUploadQualificationsFullDataSetParallel()
        {
            APIResponseList<QualificationCSV> quals = await _registerAPIClient.GetFullQualificationsDataSetAsync(null, 1, 1);

            int pages = Convert.ToInt32(Math.Ceiling(quals.Count / 10000m));
            var allQuals = new List<QualificationCSV>();

            var fetchQualsActions = new List<Action>();

            for (int i = 1; i <= pages; i++)
            {
                fetchQualsActions.Add(async () =>
                {
                    var batch = await _registerAPIClient.GetFullQualificationsDataSetAsync(null, page: i, limit: 10000);
                    allQuals.AddRange(batch.Results!);
                });
            }

            Parallel.Invoke(fetchQualsActions.ToArray());

            allQuals = allQuals.OrderBy(e => e.QualificationNumber).ToList();

            var stream = CreateCSVStream(allQuals);
            stream.Position = 0;

            await _blobService.UploadBlob(BLOBNAME_QUALIFICATIONS, stream);
        }

        private async Task FetchUploadQualificationsFullDataSetTask()
        {
            APIResponseList<QualificationCSV> resposne = await _registerAPIClient.GetFullQualificationsDataSetAsync(null, 1, 1);

            int pages = Convert.ToInt32(Math.Ceiling(resposne.Count / 10000m));
            var allQuals = new List<QualificationCSV>();

            var tasks = new List<Task>();

            for (int i = 1; i <= pages; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var batch = await _registerAPIClient.GetFullQualificationsDataSetAsync(null, page: i, limit: 10000);
                    allQuals.AddRange(batch.Results!);
                }));
            }

            Task.WaitAll([.. tasks]);

            allQuals = allQuals.OrderBy(e => e.QualificationNumber).ToList();

            var stream = CreateCSVStream(allQuals);
            stream.Position = 0;

            await _blobService.UploadBlob(BLOBNAME_QUALIFICATIONS, stream);
        }

        private async Task FetchUploadQualificationsFullDataSet()
        {
            APIResponseList<QualificationCSV> response = await _registerAPIClient.GetFullQualificationsDataSetAsync(null, 1, 1);

            int pages = Convert.ToInt32(Math.Ceiling(response.Count / 10000m));

            var allQuals = new List<QualificationCSV>();

            for (int i = 1; i <= pages; i++)
            {
                var batch = await _registerAPIClient.GetFullQualificationsDataSetAsync(null, page: i, limit: 10000);

                allQuals.AddRange(batch.Results!);
            }

            var stream = CreateCSVStream(allQuals);
            stream.Position = 0;

            await _blobService.UploadBlob(BLOBNAME_QUALIFICATIONS, stream);
        }
        #endregion
    }
}
