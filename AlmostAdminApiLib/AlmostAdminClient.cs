using Newtonsoft.Json;
using RestSharp;
using System;

namespace AlmostAdmin.API
{
    public class AlmostAdminClient
    {
        private string _apiRoute = "http://localhost:61556/api/client/";
        private int _projectId;
        private string _login;
        private string _statusUrl;
        private string _privateKey;

        public AlmostAdminClient(int projectId, string login, string projectPrivateKey, string statusUrl, string apiLink = null)
        {
            if (!string.IsNullOrEmpty(apiLink))
                _apiRoute = apiLink;

            _projectId = projectId;
            _login = login;
            _statusUrl = statusUrl;
            _privateKey = projectPrivateKey;
        }

        private string CreateSignature(string jsonData, string privateKey)
        {
            var base64EncodedData = CryptoUtils.Base64Encode(jsonData);
            var stringToBeHashed = privateKey + base64EncodedData + privateKey;
            var sha1HashedString = CryptoUtils.HashSHA1(stringToBeHashed);
            var base64EncodedSha1String = CryptoUtils.Base64Encode(sha1HashedString);

            return base64EncodedSha1String;
        }

        public AnswerOnRequest SendQuestion(string questionText)
        {
            try
            {
                if (string.IsNullOrEmpty(questionText))
                    return null;

                var question = new QuestionToApi
                {
                    Login = _login,
                    ProjectId = _projectId,
                    StatusUrl = _statusUrl,
                    Text = questionText
                };

                var questionJson = JsonConvert.SerializeObject(question);
                var signature = CreateSignature(questionJson, _privateKey);
                var data = CryptoUtils.Base64Encode(questionJson);

                var request = new RestRequest(Method.POST);
                request.AddParameter("data", questionJson);
                request.AddParameter("signature", signature);

                var response = new RestClient(_apiRoute).Execute(request);

                return JsonConvert.DeserializeObject<AnswerOnRequest>(response.Content);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
