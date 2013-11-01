# SlideRoom API for C\# 

## Example

    using System;
    using System.IO;
    using SlideRoom.API;

    namespace TestApplication
    {
        class Program
        {
            static void Main(string[] args)
            {
                var client = new SlideRoomClient("api key "access key "email address "organization code");

                var requestRes = client.Export.Request("export name", SlideRoom.API.Resources.RequestFormat.Txt);
                PrintExport(client, requestRes.Token);
                Console.ReadLine();
            }

            static void PrintExport(SlideRoomClient client, int token)
            {
                bool pending = true;
                while (pending == true)
                {
                    var downloadRes = client.Export.Download(token);
                    pending = downloadRes.Pending;
                    if (pending == false)
                    {
                        var responseText = String.Empty;
                        using (var reader = new StreamReader(downloadRes.ExportStream))
                        {
                            responseText = reader.ReadToEnd();
                        }

                        Console.Write(responseText);
                        break;
                    }

                    // wait 10 seconds before trying again
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
        }
    }

## Install From NuGet

    PM> Install-Package SlideRoom

