using Mars_Rover.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mars_Rover.Services;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Mars_Rover.Controllers
{
    public class HomeController : Controller
    {
        private readonly NasaMarsAPIService _nasaMarsapiService;
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, NasaMarsAPIService nasaMarsapiService, IConfiguration configuration)
        {
            _logger = logger;
            _nasaMarsapiService = nasaMarsapiService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> GetMarsRoverPhotosByDate(string datevalue)
        {
            try
            {
                MarsRoverPhotos objMarsRoverPhotos = new MarsRoverPhotos();
                DateTime currentdate = new DateTime();
                string result;
                CultureInfo provider = CultureInfo.InvariantCulture;
                if (DateTime.TryParse(datevalue, out currentdate))
                {
                    result = Convert.ToDateTime(datevalue).ToString("yyyy-MM-dd");
                    objMarsRoverPhotos = await _nasaMarsapiService.GetMarsRoverPhotosByDateAsync(result.ToString());

                    if (!(Directory.Exists(_configuration.GetValue<string>("ConfigurationSettings:Path"))))
                    {
                        Directory.CreateDirectory(_configuration.GetValue<string>("ConfigurationSettings:Path"));
                    }

                    foreach (Photo photo in objMarsRoverPhotos.photos)
                    {
                        using (WebClient client = new WebClient())
                        {
                            string fileName = Path.GetFileName(photo.img_src);
                            client.DownloadFileTaskAsync(new Uri(photo.img_src), _configuration.GetValue<string>("ConfigurationSettings:Path") + datevalue + "-" + fileName).Wait();
                        }
                    }
                }
                else
                {
                    objMarsRoverPhotos.message = "Provided Datetime Not Valid";
                }

                return View ("~/Views/MarsRoverImages.cshtml", objMarsRoverPhotos);
            }
            catch (Exception ex)
            {
                MarsRoverPhotos objMarsRoverPhotos = new MarsRoverPhotos();
                objMarsRoverPhotos.message = "Error occured while processing your request. Please contact your administrator.";
                _logger.LogCritical("Error occured while performing call to GetMarsRoverPhotosByDate. Date: " + datevalue.ToString() + " Exception: " + ex.ToString() + " Exception Trace: " + ex.StackTrace);
                return View("~/Views/MarsRoverImages.cshtml", objMarsRoverPhotos);
            }
        }

        //public bool DownloadFiles(MarsRoverPhotos objMarsRoverPhotos)
        //{
        //    try
        //    {
        //        foreach (Photo photo in objMarsRoverPhotos.photos)
        //        {
        //            using (WebClient client = new WebClient())
        //            {
        //                string fileName = Path.GetFileName(photo.img_src);
        //                client.DownloadFileTaskAsync(new Uri(photo.img_src), _configuration.GetValue<string>("ConfigurationSettings:Path") + photo.earth_date + "-" + fileName).Wait();
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical("Error occured while performing call to DownloadFiles. Exception: " + ex.ToString() + " Exception Trace: " + ex.StackTrace);
        //        return false;
        //    }
        //}
    }
}
