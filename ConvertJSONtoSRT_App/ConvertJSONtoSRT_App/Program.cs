using ConvertJSONtoSRT_App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertJSONtoSRT_App
{
    class Program
    {
        static void Main(string[] args)
        {
            string result = string.Empty;
            string path = @"C:\Users\alex\Documents\Visual Studio 2017\Projects\ConvertJSONtoSRT_App\ConvertJSONtoSRT_App\Files";
            //Console.WriteLine("Por favor escriba o pegue el nombre del archivo a recorrer para traducir.");
            string[] Lista = Directory.GetFiles(path)
                .Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();

            foreach (var item in Lista)
            {
                string filename = item.ToString();
                Console.WriteLine(filename);
                result= ReadAndWriteFile(filename, path);
            }

            Console.WriteLine(result);
            Console.ReadKey();
        }

        static private string ReadAndWriteFile(string filename, string path)
        {
            //string idInst = Session["IdInst"].ToString();
            List<ClosedCaption> closedCaptionList = new List<ClosedCaption>();
            string archive = filename;

            try
            {
                string outputJSON = File.ReadAllText($@"{path}\{archive}.JSON");
                closedCaptionList = JsonConvert
                    .DeserializeObject<List<ClosedCaption>>(outputJSON);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            closedCaptionList.Add(new ClosedCaption());

            string fileCreated = $@"{path}\Create\{archive}.srt";
            string content = string.Empty;

            TimeSpan timeInitial = TimeSpan.FromSeconds(0),
                timeEnd = TimeSpan.FromSeconds(0);
            string txt = string.Empty;
            int count = 0;
            double valueEnd = 0;

            foreach (var item in closedCaptionList)
            {
                
                if (closedCaptionList.Count() == count+1)
                {
                    item.DisplayTimeOffset = valueEnd + 15;
                }

                if (count != 0)
                {
                    int Id = count -1;
                    timeEnd = item.DisplayTimeOffset < 0 ?
                            TimeSpan.FromSeconds(0) :
                            TimeSpan.FromSeconds(item.DisplayTimeOffset);
                    string timeI = timeInitial.ToString(@"h\:m\:ss\.fff").Replace(".", ",");
                    string timeE = timeEnd.ToString(@"h\:m\:ss\.fff").Replace(".", ",");

                    content += $"{Id}\r\n" +
                        $"{timeI} --> {timeE}\r\n" +
                        $"{txt}\r\n\r\n";

                    
                }

                if (count == 0)
                {
                    if (item.DisplayTimeOffset < 0)
                    {
                        item.DisplayTimeOffset = -(item.DisplayTimeOffset);
                    }

                    if (item.DisplayTimeOffset == 0)
                    {
                        timeInitial = item.DisplayTimeOffset < 0 ? 
                            TimeSpan.FromSeconds(0) : 
                            TimeSpan.FromSeconds(item.DisplayTimeOffset);
                        txt = item.Text;
                    }
                    else
                    {
                        timeInitial = TimeSpan.FromSeconds(0);
                        timeEnd = TimeSpan.FromSeconds(item.DisplayTimeOffset);
                        txt = item.Text;
                    }

                }
                else
                {
                    timeInitial = timeEnd;
                    timeEnd = TimeSpan.FromSeconds(item.DisplayTimeOffset);
                    valueEnd = item.DisplayTimeOffset;
                    txt = item.Text;
                }


                count++;
            }

            try
            {
                File.WriteAllText(fileCreated, content);
                return "Terminado";
            }
            catch (Exception ex)
            {
                return "Error" + ex.Message;
            }

            
        }
    }
}
