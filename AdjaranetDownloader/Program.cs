// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");


var sb = new StringBuilder();

for (int i = 0; i < 500; i++)
{
    sb.AppendLine($"file 'seg-{i+1}-f2-v1-a1.ts'");
}

File.WriteAllText("fileList.txt", sb.ToString());

Console.ReadLine();

string baseUrl = "https://ggpjq6gmqddb.milocdn.com/hls2/01/01528/nmfa2o38lncl_,l,n,.urlset/";
string queryParams = "?t=7CXrwIfxrKFxkDYHpKg0L6oXt_gpyn7bTxTom58SSvc&s=1732639729&e=129600&f=7642651&srv=bqycOuNuR1y4&i=0.4&sp=500&p1=bqycOuNuR1y4&p2=bqycOuNuR1y4&asn=16010";


int totalSegments = 500;
    string downloadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Segments");
    Directory.CreateDirectory(downloadDirectory);

    using (HttpClient client = new HttpClient())
    {
        List<Task> downloadTasks = new List<Task>();

        for (int i = 400; i <= totalSegments; i++)
        {
            int segmentNumber = i; // To avoid closure issues
            downloadTasks.Add(Task.Run(async () =>
            {
                string segmentFileName = $"seg-{segmentNumber}-f2-v1-a1.ts";
                string fullUrl = $"{baseUrl}{segmentFileName}{queryParams}";
                string filePath = Path.Combine(downloadDirectory, segmentFileName);

                try
                {
                    Console.WriteLine($"Downloading {segmentFileName}...");
                    byte[] data = await client.GetByteArrayAsync(fullUrl);
                    await File.WriteAllBytesAsync(filePath, data);
                    Console.WriteLine($"{segmentFileName} downloaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading {segmentFileName}: {ex.Message}");
                }
            }));
        }

        await Task.WhenAll(downloadTasks);
    }

    Console.WriteLine("All downloads completed.");
