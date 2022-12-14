using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Plugins;
using System.IO;
using MediaBrowser.Model.Drawing;

namespace Emby.Notification.Join
{
    public class Plugin : BasePlugin, IHasWebPages, IHasThumbImage, IHasTranslations
    {
        private const string EditorJsName = "joinnotificationeditorjs";

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = EditorJsName,
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.entryeditor.js"
                },
                new PluginPageInfo
                {
                    Name = "joineditortemplate",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.entryeditor.template.html",
                    IsMainConfigPage = false
                }
            };
        }

        public string NotificationSetupModuleUrl => GetPluginPageUrl(EditorJsName);

        public TranslationInfo[] GetTranslations()
        {
            var basePath = GetType().Namespace + ".strings.";

            return GetType()
                .Assembly
                .GetManifestResourceNames()
                .Where(i => i.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                .Select(i => new TranslationInfo
                {
                    Locale = Path.GetFileNameWithoutExtension(i.Substring(basePath.Length)),
                    EmbeddedResourcePath = i

                }).ToArray();
        }

        public static string StaticName = "Join";

        /// <summary>
        /// Gets the name of the plugin
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get { return StaticName + " Notifications"; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Sends notifications to Join - a notification framework by joaoapps.";
            }
        }

        private Guid _id = new Guid("dae89dfe-a910-4eb4-8b3e-1d642a8ce075");
        public override Guid Id
        {
            get { return _id; }
        }

        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
        }

        public ImageFormat ThumbImageFormat
        {
            get
            {
                return ImageFormat.Png;
            }
        }
    }
}
