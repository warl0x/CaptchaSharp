﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Gets the base64-encoded string from an image.
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <param name="format">The format of the bitmap image</param>
        /// <returns>The base64-encoded string</returns>
        public static string ToBase64(this Bitmap image, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            byte[] imageBytes = stream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public static Bitmap ToBitmap(this string base64, ImageFormat format)
        {
            throw new NotImplementedException();
        }

        // <summary>
        /// Gets a memory stream from an image.
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <param name="format">The format of the bitmap image</param>
        /// <returns>The memory stream</returns>
        public static MemoryStream ToStream(this Bitmap image, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            return stream;
        }

        /// <summary>
        /// Gets the bytes from an image.
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <returns>The bytes of the image</returns>
        public static byte[] ToBytes(this Bitmap image, ImageFormat format)
        {
            return Convert.FromBase64String(image.ToBase64(format));
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<T> GetJsonAsync<T>
            (this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            var json = await GetStringAsync(httpClient, url, cancellationToken).ConfigureAwait(false);
            var obj = JsonConvert.DeserializeObject<T>(json);

            if (obj == null)
                throw new JsonException($"Error while deserializing the response to type {typeof(T)}");

            return obj;
        }

        public static async Task<string> GetStringAsync
            (this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<T> PostMultipartJsonAsync<T>
            (this HttpClient httpClient, string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var json = await PostMultipartAsync(httpClient, url, content, cancellationToken).ConfigureAwait(false);
            var obj = JsonConvert.DeserializeObject<T>(json);

            if (obj == null)
                throw new JsonException($"Error while deserializing the response to type {typeof(T)}");

            return obj;
        }

        public static async Task<string> PostMultipartAsync
            (this HttpClient httpClient, string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<Bitmap> DownloadBitmapAsync
            (this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var imageStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return new Bitmap(imageStream);
        }
    }

    public static class BoolExtensions
    {
        public static int ToInt(this bool boolean)
        {
            return boolean ? 1 : 0;
        }
    }

    public static class IEnumerableExtensions
    {
        public static MultipartFormDataContent ToMultipartFormDataContent(this IEnumerable<(string, string)> stringContents)
        {
            var content = new MultipartFormDataContent();

            stringContents.ToList()
                .ForEach(p => content.Add(new StringContent(p.Item2, Encoding.UTF8), p.Item1));

            return content;
        }
    }
}
