using Project;
using System;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace ConvertItOnline
{
    public static class ProcessController
    {
        public static Random RNG = new Random();
        public static async void AcceptRequest(HttpContext context)
        {
            var req = context.Request;
            var res = context.Response;
            try
            {
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

                string operationName = "ConverItOnlineOperation" + RNG.Next();
                string OperationFolderPath = Directory.CreateDirectory(Path.GetTempPath() + operationName).FullName;
                process.path = OperationFolderPath;
                process.doSaveAsWavefont();

                ZipFile.CreateFromDirectory(OperationFolderPath, OperationFolderPath + ".zip");

                StatsController.RegisterProcessComplete();
                res.Headers.Add("Content-Type", "application/zip");
                res.Headers.Add("content-disposition", "attachment; filename= GeneratedMesh.zip");
                await res.SendFileAsync(OperationFolderPath + ".zip");
            }
            catch (Exception e)
            {
                StatsController.RegisterProcessFailed();
                await res.WriteAsync("Process failed" + e);
            }
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