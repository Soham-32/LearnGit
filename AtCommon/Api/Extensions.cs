using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AtCommon.Api
{
    public static class Extensions
    {
        public static async Task<ApiResponse<TResponseDto>> PutAsync<TResponseDto>(this HttpClient client, string requestUri, object dto)
        {
            var httpContent = dto.ToStringContent();

            var httpResponse = await client.PutAsync(requestUri, httpContent);
            var apiResponse = await ApiResponse<TResponseDto>.Map(httpResponse);

            return apiResponse;
        }

        public static async Task<ApiResponse<TDto>> GetAsync<TDto>(this HttpClient client, string requestUri)
        {
            var httpResponse = await client.GetAsync(requestUri);
            var apiResponse = await ApiResponse<TDto>.Map(httpResponse);

            return apiResponse;
        }

        public static async Task<ApiResponse<TResponseDto>> PostAsync<TResponseDto>(this HttpClient client, string requestUri, object dto = null)
        {

            var httpContent = dto?.ToStringContent();

            var httpResponse = await client.PostAsync(requestUri, httpContent);
            var apiResponse = await ApiResponse<TResponseDto>.Map(httpResponse);

            return apiResponse;
        }

        public static async Task<ApiResponse<TResponseDto>> PostUploadAsync<TResponseDto>(this HttpClient client, string requestUri, string filePath)
        {
            var httpContent = new MultipartFormDataContent();
            var fs = File.OpenRead(filePath);
            httpContent.Add(new StreamContent(fs), "filename", Path.GetFileName(filePath));

            var httpResponse = await client.PostAsync(requestUri, httpContent);
            var apiResponse = await ApiResponse<TResponseDto>.Map(httpResponse);

            return apiResponse;
        }

        public static async Task<ApiResponse<TDto>> DeleteAsync<TDto>(this HttpClient client, string requestUri)
        {
            var httpResponse = await client.DeleteAsync(requestUri);
            var apiResponse = await ApiResponse<TDto>.Map(httpResponse);

            return apiResponse;
        }
        
        public static async Task<ApiResponse<TDto>> DeleteAsync<TDto>(this HttpClient client, string requestUri, object dto)
        {
            var httpContent = dto.ToStringContent();
            var httpResponse = await client.DeleteAsync(requestUri, httpContent);
            var apiResponse = await ApiResponse<TDto>.Map(httpResponse);

            return apiResponse;
        }

        public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri,
            object dto)
        {

            var request = new HttpRequestMessage
            {
                Content = dto.ToStringContent(),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.RelativeOrAbsolute)
            };

            return await client.SendAsync(request);
        }

        public static async Task<ApiResponse<TDto>> PatchAsync<TDto>(this HttpClient client, string requestUri, object dto)
        {
            var httpResponse = await client.PatchAsync(requestUri, dto);
            var apiResponse = await ApiResponse<TDto>.Map(httpResponse);

            return apiResponse;
        }

        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri,
            object dto = null)
        {

            var request = new HttpRequestMessage
            {
                Content = dto.ToStringContentPatch(),
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(requestUri, UriKind.RelativeOrAbsolute)
            };

            return await client.SendAsync(request);
        }

        public static StringContent ToStringContent(this object dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static StringContent ToStringContentPatch(this object dto)
        {
            var jsonRequest = JsonConvert.SerializeObject(dto);

            return new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
        }

        public static string AddQueryParameter(this string uri, IDictionary<string, object> queryString)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (queryString == null)
                throw new ArgumentNullException(nameof(queryString));
            var num = uri.IndexOf('#');
            var str1 = uri;
            var str2 = "";
            if (num != -1)
            {
                str2 = uri.Substring(num);
                str1 = uri.Substring(0, num);
            }
            var flag = str1.IndexOf('?') != -1;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(str1);
            foreach (var keyValuePair in queryString)
            {
                stringBuilder.Append(flag ? '&' : '?');
                stringBuilder.Append(UrlEncoder.Default.Encode(keyValuePair.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(UrlEncoder.Default.Encode(keyValuePair.Value.ToString()));
                flag = true;
            }
            stringBuilder.Append(str2);
            return stringBuilder.ToString();
        }

        public static string AddQueryParameter(this string uri, string key, object value)
        {
            return uri.AddQueryParameter(new Dictionary<string, object> {{key, value}});
        }

        public static TDto DeserializeJsonObject<TDto>(this string json)
        {
            return JsonConvert.DeserializeObject<TDto>(json);
        }

        public static string GetDescription<TT>(this TT enumerationValue)
            where TT : struct
        {
            var type = enumerationValue.GetType();
            if(!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length <= 0) return enumerationValue.ToString();
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumerationValue.ToString();
        }
    }
}