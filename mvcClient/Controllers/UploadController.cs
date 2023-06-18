using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;

namespace mvcClient.Controllers
{
    [Route("[controller]/[action]")]
    public class UploadController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("The file size is too large or the extension is not supported.");
                }

                string newFileName = Guid.NewGuid().ToString() + "___" + Path.GetFileName(file.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { newFileName = Path.GetFileName(newFileName) });
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                fileName = System.Net.WebUtility.UrlDecode(fileName);

                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("File name is null or empty");
                }

                string path = Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, GV.I.UD, fileName);

                if (System.IO.File.Exists(path) == false)
                {
                    return BadRequest("파일이 존재하지 않습니다.");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path).Split("___")[1]);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                fileName = System.Net.WebUtility.UrlDecode(fileName);

                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("File name is null or empty");
                }

                string path = Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, fileName);

                if (System.IO.File.Exists(path) == false)
                {
                    return BadRequest("파일이 존재하지 않습니다.");
                }

                System.IO.File.Delete(path);

                return Ok();
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }   

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();   

            if (types.ContainsKey(ext))
            {
                return types[ext];
            } else
            {
                return "application/octet-stream";
            }
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".jpg", "image/jpeg"},
                {".png", "image/png"},
                {".gif", "image/gif"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".csv", "text/csv"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformatsofficedocument.presentationml.presentation"},
                {".zip", "application/zip"},
                {".7z", "application/x-7z-compressed"},
                {".rar", "application/x-rar-compressed"}
            };
        }
    }
}
