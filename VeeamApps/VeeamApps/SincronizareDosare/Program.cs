using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class SincronizareDosare
{
    private string caleSursa;
    private string caleReplica;
    private int intervalSincronizare;
    private string caleFisierLog;

    public SincronizareDosare(string caleSursa, string caleReplica, int intervalSincronizare, string caleFisierLog)
    {
        this.caleSursa = caleSursa;
        this.caleReplica = caleReplica;
        this.intervalSincronizare = intervalSincronizare;
        this.caleFisierLog = caleFisierLog;

        // Creează directoarele necesare dacă nu există
        if (!Directory.Exists(caleSursa))
        {
            Directory.CreateDirectory(caleSursa);
        }

        if (!Directory.Exists(caleReplica))
        {
            Directory.CreateDirectory(caleReplica);
        }

        string caleFisierLogDirectory = Path.GetDirectoryName(caleFisierLog);
        if (!Directory.Exists(caleFisierLogDirectory))
        {
            Directory.CreateDirectory(caleFisierLogDirectory);
        }
    }

    public void Start()
    {
        while (true)
        {
            try
            {
                SincronizeazaDosarele();
            }
            catch (Exception ex)
            {
                Log($"Eroare: {ex.Message}");
            }

            Thread.Sleep(intervalSincronizare);
        }
    }

    private void SincronizeazaDosarele()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "robocopy",
            Arguments = $"\"{caleSursa}\" \"{caleReplica}\" /MIR",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
            {
                Log(output);
            }
            if (!string.IsNullOrEmpty(error))
            {
                Log(error);
            }
        }
    }

    private void Log(string mesaj)
    {
        var mesajLog = $"{DateTime.Now}: {mesaj}";
        Console.WriteLine(mesajLog);
        File.AppendAllText(caleFisierLog, mesajLog + Environment.NewLine);
    }

    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Utilizare: SincronizareDosare <caleSursa> <caleReplica> <intervalSincronizare> <caleFisierLog>");
            return;
        }

        var caleSursa = args[0];
        var caleReplica = args[1];
        var intervalSincronizare = int.Parse(args[2]);
        var caleFisierLog = args[3];

        var sincronizareDosare = new SincronizareDosare(caleSursa, caleReplica, intervalSincronizare, caleFisierLog);
        sincronizareDosare.Start();
    }
}
