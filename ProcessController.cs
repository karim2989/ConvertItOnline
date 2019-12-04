using Project;
using System;
using System.Drawing;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Web;

namespace ConvertItOnline
{
    public static class ProcessController
    {
        public static async void AcceptRequest(HttpContext context)
        {
            var req = context.Request;
            var res = context.Response;
            Process process = new Process();

            if (req.Form.Files.Count != 3) { await res.WriteAsync("this operation requires 3 images"); return; }

            process.images = new Bitmap[3];
            for (int i = 0; i < 3; i++)
            {
                process.images[i] = new Bitmap(req.Form.Files[i].OpenReadStream());
            }

            process.doCarveShape();

            process.doColorShape();

            process.doBuildMesh();

            process.doOptimizeMesh();

            process.doCentralizeMesh();
            process.doFlipMesh(Cords3.one);

            await res.WriteAsync(generateHtml(process.mesh.Serialize()));
        }
        private static string generateHtml((string waveFont, string mtl) data)
        {
            return @"" +
            @"<h2>GeneratedMesh.obj</h2>" + Environment.NewLine +
            $@"<p>{data.waveFont.Replace("\n", "<br>")}</p>" + Environment.NewLine +
            @"<h2>GeneratedMesh.mtl</h2>" + Environment.NewLine +
            $@"<p>{data.mtl.Replace("\n", "<br>")}</p>";
        }
    }
}