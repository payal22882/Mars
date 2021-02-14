using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Mars_Rover.Models;
using Newtonsoft.Json;
using System.Net;
using System.Globalization;

namespace Mars_Rover.Services
{
    [ApiController]
    public class NasaMarsAPIService
    {
        private IConfiguration _configuration;
        private static readonly HttpClient nasaclient = new HttpClient();
        private readonly ILogger _logger;

        public NasaMarsAPIService(IConfiguration configuration,ILogger<NasaMarsAPIService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

       [Route("MarsRoverPhotos/{datevalue}")]
       [HttpGet]
        public async Task<MarsRoverPhotos> GetMarsRoverPhotosByDateAsync(string datevalue)
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
                    string baseURL = _configuration.GetValue<string>("ConfigurationSettings:MarsRoverPhotosAPI");
                    string apiKey = "IVSY73JJjs28z7GYrbYdBCm7O5ewzc5KlqsJD87a";
                    string finalURL = baseURL + "earth_date=" + result + "&api_key=" + apiKey;

                    //Configure httpclient for call

                    // Add an Accept header for JSON format.
                    nasaclient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    //send request to provenir
                    var response = await nasaclient.GetAsync(finalURL);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //deserialize to object again                
                        objMarsRoverPhotos = JsonConvert.DeserializeObject<MarsRoverPhotos>(response.Content.ReadAsStringAsync().Result);
                        objMarsRoverPhotos.message = "Success";
                    }
                    else
                    {
                        objMarsRoverPhotos.message = "Error calling Nasa Mars API";
                        _logger.LogCritical("Error occured while performing call to NasaMarsRoverPhotos API. Date: " + datevalue.ToString() + " Exception: " + response.Content.ReadAsStringAsync().Result);
                        return objMarsRoverPhotos;
                    }

                    //deserialize to object again                
                    return objMarsRoverPhotos;
                }

                else
                {
                    objMarsRoverPhotos.message = "Provided Datetime Not Valid";
                    return objMarsRoverPhotos;
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error occured while performing call to GetMarsRoverPhotosByDateAsync. Date: " + datevalue.ToString() + " Exception: " + ex.ToString() + " Exception Trace: " + ex.StackTrace);
                MarsRoverPhotos objMarsRoverPhotos = new MarsRoverPhotos();
                objMarsRoverPhotos.message = "Error occured while processing your request. Please contact your administrator.";
                return objMarsRoverPhotos;
            }
        }
    }
}
