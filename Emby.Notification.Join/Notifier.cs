using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Notifications;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using Emby.Notifications;
using MediaBrowser.Controller;
using System.Net;

namespace Emby.Notification.Join
{
    public class Notifier : IUserNotifier
    {
        private ILogger _logger;
        private IServerApplicationHost _appHost;
        private IHttpClient _httpClient;

        public Notifier(ILogger logger, IServerApplicationHost applicationHost, IHttpClient httpClient)
        {
            _logger = logger;
            _appHost = applicationHost;
            _httpClient = httpClient;
        }

        private Plugin Plugin => _appHost.Plugins.OfType<Plugin>().First();

        public string Name => Plugin.StaticName;

        public string Key => "joinnotifications";

        public string SetupModuleUrl => Plugin.NotificationSetupModuleUrl;

        public async Task SendNotification(InternalNotificationRequest request, CancellationToken cancellationToken)
        {
            var options = request.Configuration.Options;

            options.TryGetValue("DeviceId", out string deviceId);
            options.TryGetValue("ApiKey", out string apiKey);

            var message = new Dictionary<string, string> {
                { "deviceId", deviceId },
                { "icon", IconURL.ToString() },
                { "apikey", apiKey }
            };

            message.Add("title", request.Title);

            if (!string.IsNullOrEmpty(request.Description))
            {
                message.Add("text", request.Description);
            }

            var _httpRequest = new HttpRequestOptions
            {
                Url = ApiV1Endpoint + ToQueryString(message),
                CancellationToken = cancellationToken
            };

            using (await _httpClient.Get(_httpRequest).ConfigureAwait(false))
            {

            }
        }

        private Uri IconURL
        {
            get { return new Uri("https://raw.githubusercontent.com/MediaBrowser/Emby.Resources/master/images/Logos/logoicon.png"); }
        }

        private string ApiV1Endpoint
        {
            get { return "https://joinjoaomgcd.appspot.com/_ah/api/messaging/v1/sendPush"; }
        }

        private string ToQueryString(Dictionary<string, string> variables)
        {
            var array = (from KeyValuePair<string, string> pair in variables
                         select string.Format("{0}={1}", WebUtility.UrlEncode(pair.Key), WebUtility.UrlEncode(pair.Value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }
    }
}

