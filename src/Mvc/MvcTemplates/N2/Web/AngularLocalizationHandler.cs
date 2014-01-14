using N2.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace N2.Management.Web
{
    public class AngularLocalizationHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";

            if (!context.IsDebuggingEnabled)
                context.Response.SetOutputCache(DateTime.Now.AddHours(1));

            context.Response.Write("(function (module) {");
            context.Response.Write(@"
module.factory('Translate', function () {
    return function (key, fallback) {
        var t = translations;
        var k = key.split('.');
        for (var i in k) {
            t = t[k[i]];
            if (!t) return fallback;
        }
        return t;
    }
});
");
            context.Response.Write("var translations = ");

            var jsPath = context.Request.AppRelativeCurrentExecutionFilePath.Substring(0, context.Request.AppRelativeCurrentExecutionFilePath.Length - 5);
            var file = HostingEnvironment.VirtualPathProvider.GetFile(jsPath);
            using (var s = file.Open())
            {
                TransferBetweenStreams(s, context.Response.OutputStream);
            }
            context.Response.Write(";" + Environment.NewLine);
            context.Response.Write("module.value('n2translations', translations);");

            context.Response.Write("})(angular.module('n2.localization', []));");
        }

        long TransferBetweenStreams(Stream inputStream, Stream outputStream)
        {
            long inputStreamLength = 0;
            byte[] buffer = new byte[32768];
            while (true)
            {
                int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                    break;

                outputStream.Write(buffer, 0, bytesRead);
                inputStreamLength += bytesRead;
            }
            return inputStreamLength;
        }

    }
}
