using Application.Repositories.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly MeterReadingService _meterReadingService;

        public MeterReadingController(IMeterReadingRepository meterReadingRepository, MeterReadingService meterReadingService)
        {
            _meterReadingRepository = meterReadingRepository;
            _meterReadingService = meterReadingService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadMeterReadings(IFormFile file)
        {
            // Validate the file
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded or file is empty.");

            // Ensure the file is a CSV
            if (Path.GetExtension(file.FileName)?.ToLower() != ".csv")
                return BadRequest("Invalid file format. Only CSV files are allowed.");

            try
            {
                // Process the CSV file
                var result = await _meterReadingService.ProcessMeterReadingsAsync(file.OpenReadStream());
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle any errors during processing
                return StatusCode(500, $"Error processing file: {ex.Message}");
            }
        }
    }
}
