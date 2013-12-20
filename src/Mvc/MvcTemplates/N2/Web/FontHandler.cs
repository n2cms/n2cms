using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace N2.Management.Web
{
    public class FontHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            switch (VirtualPathUtility.GetExtension(context.Request.Url.AbsolutePath))
            {
                case ".eot":
                    Transmit(context, "application/x-eot");
                    return;
                case ".svg":
                    Transmit(context, "image/svg+xml");
                    return;
                case ".ttf":
                    Transmit(context, "application/x-ttf");
                    return;
                case ".woff":
                    Transmit(context, "application/font-woff");
                    return;
                case ".otf":
                    Transmit(context, "application/x-otf");
                    return;
            }
        }

        private void Transmit(HttpContext context, string contentType)
        {
            context.Response.ContentType = contentType;

            if (File.Exists(context.Request.PhysicalPath))
            {
                context.Response.TransmitFile(context.Request.PhysicalPath);
                return;
            }

            var file = HostingEnvironment.VirtualPathProvider.GetFile(context.Request.AppRelativeCurrentExecutionFilePath);
            if (file == null)
                return;

            using (var s = file.Open())
            {
                TransferBetweenStreams(s, context.Response.OutputStream);
            }
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
