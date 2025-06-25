using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AtCommon.Api
{
    public class ApiResponse<TDto>
    {
        public TDto Dto { get; set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Uri LocationHeader { get; private set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public static async Task<ApiResponse<TDto>> Map(HttpResponseMessage httpResponseMessage)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                var errorMessage =
                    $"There was an Internal Server Error\nContent: <{content}>\nOriginal Request: <{httpResponseMessage.RequestMessage}>";
                throw new HttpRequestException(errorMessage);
            }
            
            TDto dto;
            try
            {
                dto = JsonConvert.DeserializeObject<TDto>(content);
            }
            catch (JsonSerializationException)
            {
                throw new JsonSerializationException(
                    $"Could not deserialize response body. Expected Type: <{typeof(TDto)}>. Actual Json: <{content}>.\nOriginal Request: <{httpResponseMessage.RequestMessage}>");
            }
            catch (JsonReaderException e)
            {
                throw new JsonReaderException($"{e.Message} Content: {content}");
            }

            return new ApiResponse<TDto>
            {
                Dto = dto,
                StatusCode = httpResponseMessage.StatusCode,
                LocationHeader = httpResponseMessage.Headers?.Location,
                HttpResponseMessage = httpResponseMessage
            };
        }

        public void EnsureSuccess()
        {
            if (HttpResponseMessage.IsSuccessStatusCode) return;
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)HttpResponseMessage.StatusCode} ({HttpResponseMessage.ReasonPhrase}). \nOriginal Request: <{HttpResponseMessage.RequestMessage}>");
        }
    }
}