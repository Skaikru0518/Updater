using System.Diagnostics;
using System.Net;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Checking for updates...");

        // Step 1: define download urls
        string downloadUrl = "https://github.com/Skaikru0518/CryptoCalculator/releases/latest/download/CryptoCalculator.exe";
        string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoCalculator.exe");
        string tempExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoCalculator_new.exe");


        try
        {
            // Step 2: download the new version
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CryptoCalculator-Updater");
                HttpResponseMessage response = await client.GetAsync(downloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Downloading latest version...");
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(tempExePath, fileBytes);
                }
                else
                {
                    Console.WriteLine($"Download failed. HTTP Status:  {response.StatusCode}");
                    return;
                }
            }

            // Step 3 Kill CryptoCalculator if running
            KillProcess("CryptoCalculator");

            // Step 4: repleace the old version with the new one
            if (File.Exists(exePath)) File.Delete(exePath);
            File.Move(tempExePath, exePath);

            Console.WriteLine("Update completed! Restarting application...");
            Process.Start(exePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Update Failed: " + ex.Message);
        }
    }
    static void KillProcess(string processName)
    {
        foreach (var process in Process.GetProcessesByName(processName))
        {
            Console.WriteLine($"Closing {processName}");
            process.Kill();
            process.WaitForExit();
        }
    }
}