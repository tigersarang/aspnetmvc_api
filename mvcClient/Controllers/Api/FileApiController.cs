using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using mvcClient.Utils;

namespace mvcClient.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileApiController : ControllerBase
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<FileController> _logger;

        public FileApiController(ApiClient apiClient, ILogger<FileController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFile([FromBody] DeleteDto deleteDto)
        {

            try
            {
                string filePath = string.Empty;
                if (deleteDto == null)
                {
                    return BadRequest(new { message = "삭제할 파일정보가 null입니다." });
                }

                // if the id is not null, it is a case of deletion from the modification screen.
                // In this case, because it's a deleteion of a file stored in the database, we add a database deletion operation.

                if (deleteDto.Id != null)
                {
                    if (_apiClient.IsTokenExpired())
                    {
                        if (!await _apiClient.RefreshToken())
                        {
                            return RedirectToAction("Login", "Account");
                        }
                    }

                    _apiClient.SetAccessToken();

                    ProductFile productFile = await _apiClient.GetFile(deleteDto.Id.Value);
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, GV.I.UD, productFile.LInkFileName);

                    var response = await _apiClient.DeleteFile(deleteDto.Id.Value);

                    if (response == false)
                    {
                        return BadRequest(new { message = "There was an error deleting the file from the database." });
                    }
                }

                filePath = Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, GV.I.UD, filePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }


                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "error. " });
            }
            finally
            {
            }

        }
    }
}
