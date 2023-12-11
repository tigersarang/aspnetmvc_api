using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace mvcClient.Utils
{
    public partial class ApiClient
    {

        ////////////////////////////////////////////////////////////////////////////////
        // File 작업
        ////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="deleteDto"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(int Id)
        {

            try
            {
                var response = await _httpClient.DeleteAsync($"/api/FilesApi/{Id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                } else
                {
                    _logger.LogError(await response.Content.ReadAsStringAsync());
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            finally
            {
            }
            
        }

        public async Task<ProductFile> GetFile(int id)
        {
            var response = await _httpClient.GetAsync($"/api/FilesApi/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductFile>();
            }
            else
            {
                return null;
            }
        }
    }
}
