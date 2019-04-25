using Blitz.Rpc.Client.BaseClasses;
using Blitz.Rpc.Shared;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Test.Client")]

namespace Blitz.Rpc.Client.Helper
{


    /// <summary>
    /// This is the default implementation for a ApiClient, the serializer is replaceable.
    /// The serializer must ofcourse match the serializer in the other end.
    /// It is based on HTTP(S) and works with det default implementation of Blitz.Rpc.Server
    /// </summary>
    public class HttpApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        readonly ISerializer serializer;

        public HttpApiClient(HttpClient httpClient,ISerializer serializers)
        {
            this.httpClient = httpClient;
            this.serializer = serializers;
        }

        public async Task<object> Invoke(RpcMethodInfo toCall, object[] param)
        {
            if (param == null) param = new object[0];
            //This format is recognized by the default implementation of the server.
            //The base url for the request is expected to be set on the HttpClient
            string requestUri = CreateIdString(toCall);
            var theHttpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);

            var outstream = new System.IO.MemoryStream();

            var theSerializer = serializer;

            switch (param.Length)
            {
                case 0:
                    break;

                case 1:
                    theSerializer.ToStream(outstream, param[0]);
                    break;

                default:
                    theSerializer.ToStream(outstream, param);
                    break;
            }

            outstream.Position = 0;
            theHttpRequest.Content = new StreamContent(outstream);

            theHttpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(theSerializer.ProduceMimeType);

            var response = await httpClient.SendAsync(theHttpRequest);

            if (response.IsSuccessStatusCode)
            {
                if (toCall.ReturnType == typeof(void) | toCall.ReturnType == typeof(Task)) return null;
                var ret = theSerializer.FromStream(await response.Content.ReadAsStreamAsync(), toCall.ReturnType);

                return ret;
            }
            else if ((int)response.StatusCode >= 500)
            {
                var remoteExceptionInfo = theSerializer.FromStream(await response.Content.ReadAsStreamAsync(), typeof(RemoteExceptionInfo));
                throw new WebRpcCallFailedException((RemoteExceptionInfo)remoteExceptionInfo);
            }
            else
            {
                throw new HttpRequestException($"{response.StatusCode} {response.Content.ReadAsStringAsync().Result}");
            }
        }

       internal string CreateIdString(RpcMethodInfo toCall)
        {
            return $"{toCall.ServiceId}.{toCall.Name}-{string.Join("-",toCall.ParamType.Select(p=> p.FullName).ToArray())}";
        }
    }
}