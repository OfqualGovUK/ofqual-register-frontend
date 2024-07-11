using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ofqual.Common.RegisterFrontend.BlobStorage;
using Ofqual.Common.RegisterFrontend.Models;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;
using System.IO;
using System.Net;
using System.Text.Json;
using static Ofqual.Common.RegisterFrontend.Models.Constants;

namespace Ofqual.Common.RegisterFrontend.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IRegisterAPIClient _registerAPIClient;
        private readonly IBlobService _blobService;

        public SitemapController(IRegisterAPIClient registerAPIClient, IBlobService blobService)
        {
            _registerAPIClient = registerAPIClient;
            _blobService = blobService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("sitemap/organisations")]
        public IActionResult Organisations()
        {
            return View();
        }

        [HttpGet]
        [Route("sitemap/qualifications")]
        [Route("sitemap/qualifications/{page}")]
        public async Task<IActionResult> Qualifications(int? page)
        {
            var model = new QualificationSitemap() { 
                Paging = new PagingModel() 
            };

            try
            {
                var quals = await FetchSitemapQuals();

                model.Paging.CurrentPage = page ?? 1;
                model.Paging.PagingURL = $"/qualifications?page=||_page_||";
                model.Paging.PagingList = Utilities.GeneratePageList(page ?? 1, quals.Count, 500);

                model.Count = quals.Count;
                model.Qualifications = quals.Skip(((page ?? 1) - 1) * 500).Take(500).ToList();
            }
            catch (ApiException ex)
            {
                return ex.StatusCode == HttpStatusCode.NotFound ? NotFound() : StatusCode((int)ex.StatusCode);
            }

            return View(model);
        }

        private async Task<List<QualificationSitemapData>> FetchSitemapQuals()
        {
            if (_blobService.BlobExists(BLOBNAME_SITEMAP_QUALS))
            {
                var blobProperties = await _blobService.BlobProperties(BLOBNAME_SITEMAP_QUALS);

                //check if the blob is newer than a week
                if (DateTime.Now > blobProperties.LastModified.AddDays(7))
                {
                    await UploadSiteMapQuals();
                }
            }
            else
            {
                await UploadSiteMapQuals();
            }

            //fetch the quals
            var qualsMemoryStream = await _blobService.DownloadBlob(BLOBNAME_SITEMAP_QUALS);
            qualsMemoryStream.Position = 0;

            var reader = new StreamReader(qualsMemoryStream);
            var qualsJson = reader.ReadToEnd();

            var quals = JsonSerializer.Deserialize<List<QualificationSitemapData>>(qualsJson);

            return quals ?? [];
        }

        private async Task UploadSiteMapQuals()
        {
            var qualsResponse = await _registerAPIClient.GetQualificationsForSitemap();

            if (qualsResponse.Results != null)
            {
                //quals = qualsResponse.Results;
                var json = JsonSerializer.Serialize(qualsResponse.Results);

                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream);

                streamWriter.Write(json);

                memoryStream.Position = 0;

                await _blobService.UploadBlob(BLOBNAME_SITEMAP_QUALS, memoryStream);
            }
        }
    }
}
