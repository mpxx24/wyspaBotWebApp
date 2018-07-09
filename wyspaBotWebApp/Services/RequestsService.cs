using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace wyspaBotWebApp.Services {
    public class RequestsService : IRequestsService {
        public string GetData(string address) {
            var request = (HttpWebRequest) WebRequest.Create(address);
            request.Method = "GET";
            request.ContentType = "application/json";

            try {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream()) {
                    if (responseStream != null) {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var result = reader.ReadToEnd();
                        return result;
                    }
                    return string.Empty;
                }
            }
            catch (WebException ex) {
                var errorResponse = ex.Response;
                using (var responseStream = errorResponse.GetResponseStream()) {
                    if (responseStream != null) {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var errorText = reader.ReadToEnd();
                        //TODO: log
                    }
                    return string.Empty;
                }
            }
            catch (Exception e) {
                //TODO: log
                throw;
            }
        }

        public string PostData(string address, string parameters) {
            var request = (HttpWebRequest) WebRequest.Create(address);
            var data = Encoding.ASCII.GetBytes(parameters);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream()) {
                stream.Write(data, 0, data.Length);
            }

            try {
                var response = (HttpWebResponse) request.GetResponse();
                using (var responseStream = response.GetResponseStream()) {
                    if (responseStream == null) {
                        return string.Empty;
                    }
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    var result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception e) {
                //TODO: log
                throw;
            }
        }

        public string PostData(string address, NameValueCollection parameters) {
            try {
                using (var client = new WebClient()) {
                    var response = Encoding.UTF8.GetString(client.UploadValues(address, parameters));

                    if (!Uri.TryCreate(response, UriKind.Absolute, out var _)) {
                        throw new WebException("Post Error", WebExceptionStatus.SendFailure);
                    }

                    return response;
                }
            }
            catch (Exception e) {
                //TODO: log;
                throw;
            }
        }
    }
}