using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Checking for updates...");
        await Task.Delay(2000); // Simulate a delay

        // Step 1: Define download URLs
        string downloadUrl = "https://github.com/Skaikru0518/CryptoCalculator/releases/latest/download/CryptoCalculator.exe";
        string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoCalculator.exe");
        string tempExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoCalculator_new.exe");
        string updaterPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        try
        {
            // Step 2: Download the new version
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CryptoCalculator-Updater");
                HttpResponseMessage response = await client.GetAsync(downloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Downloading latest version...");
                    await Task.Delay(2000); // Simulate a delay
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(tempExePath, fileBytes);
                }
                else
                {
                    Console.WriteLine($"Download failed. HTTP Status: {response.StatusCode}");
                    return;
                }
            }

            // Step 3: Kill CryptoCalculator if running
            KillProcess("CryptoCalculator");

            // Step 4: Replace CryptoCalculator.exe safely
            ReplaceCryptoCalculator(exePath, tempExePath);
            await Task.Delay(2000); // Simulate a delay
            Console.WriteLine("Update completed! Restarting application...");

            // Step 5: Self-destruct
            SelfDestruct(updaterPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Update Failed: " + ex.Message);
        }
    }

    static void KillProcess(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length == 0)
        {
            Console.WriteLine($"{processName} is not running.");
            return;
        }

        foreach (var process in processes)
        {
            Console.WriteLine($"Closing {processName}...");
            process.Kill();
            process.WaitForExit();
        }
    }

    static async void ReplaceCryptoCalculator(string exePath, string tempExePath)
    {
        int retryCount = 5;
        int delay = 1000;

        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                if (File.Exists(exePath)) File.Delete(exePath);
                File.Move(tempExePath, exePath);
                Console.WriteLine("CryptoCalculator.exe updated successfully.");
                await Task.Delay(2000); // Simulate a delay

                // Start the updated CryptoCalculator
                Process.Start(exePath);
                return;
            }
            catch (IOException)
            {
                Console.WriteLine("File is locked. Retrying in 1 second...");
                Thread.Sleep(delay);
            }
        }

        Console.WriteLine("❌ Failed to replace CryptoCalculator.exe after multiple attempts.");
    }

    static void SelfDestruct(string updaterPath)
    {
        Console.WriteLine("Deleting updater...");

        // Create a batch file in the same directory as `Updater.exe`
        string batchScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "delete_updater.bat");
        File.WriteAllText(batchScript, $@"
@echo off
timeout /t 2 /nobreak >nul
del /f /q ""{updaterPath}""
del /f /q ""%~f0""
");

        // Run the batch script and exit Updater
        Process.Start(new ProcessStartInfo
        {
            FileName = batchScript,
            CreateNoWindow = true,
            UseShellExecute = false
        });

        Environment.Exit(0); // Exit Updater.exe
    }



}
