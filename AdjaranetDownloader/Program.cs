/*
 * 
 * 1. xcode-select --install
 * 2. export PATH="/opt/homebrew/bin:$PATH"
 * 3. brew install ffmpeg --with-options
 * 4. ffmpeg -f concat -safe 0 -i fileList.txt -c copy output.mp4
 * 
 */

using System.Text;

var sb = new StringBuilder();

for (int i = 1; i <= 757; i++)
{
    sb.AppendLine($"file 'seg-{i}-v1-a1.ts'");
}

File.WriteAllText("fileList.txt", sb.ToString());

Console.ReadLine();
return;

// https://as9p3ktgpeiv.milocdn.com/hls2/01/01248/uffw7bxg9zmj_h/seg-6-v1-a1.ts?t=lMf0la5ma3TGD0eyHmy10pbjUK70jZt1udghfEZ8ggc&s=1737134195&e=129600&f=7504654&srv=tcQHM9WY1K6Ab&i=0.4&sp=500&p1=tcQHM9WY1K6Ab&p2=tcQHM9WY1K6Ab&asn=16010

int totalSegments = 757;
int batchSize = 100;
string baseUrl = "https://as9p3ktgpeiv.milocdn.com/hls2/01/01248/uffw7bxg9zmj_h/";
string queryParams = "?t=lMf0la5ma3TGD0eyHmy10pbjUK70jZt1udghfEZ8ggc&s=1737134195&e=129600&f=7504654&srv=tcQHM9WY1K6Ab&i=0.4&sp=500&p1=tcQHM9WY1K6Ab&p2=tcQHM9WY1K6Ab&asn=16010";

string downloadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Segments");
Directory.CreateDirectory(downloadDirectory);

using (HttpClient client = new HttpClient())
{
    for (int start = 1; start <= totalSegments; start += batchSize)
    {
        int end = Math.Min(start + batchSize - 1, totalSegments);
        List<Task> downloadTasks = new List<Task>();

        Console.WriteLine($"Starting batch download for segments {start} to {end}...");

        for (int i = start; i <= end; i++)
        {
            int segmentNumber = i; // To avoid closure issues
            downloadTasks.Add(Task.Run(async () =>
            {
                string segmentFileName = $"seg-{segmentNumber}-v1-a1.ts";
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

        Console.WriteLine($"Batch {start}-{end} download completed.");
    }
}

Console.WriteLine("All downloads completed.");
