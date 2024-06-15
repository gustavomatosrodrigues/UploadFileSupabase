using System;
using Client = Supabase.Client;

public class SupabaseUploader
{
    private readonly Client _client;

    public SupabaseUploader(string Url, string Key)
    {
       // Url = "https://wnngxjzcvzhjeainxfyb.supabase.co";
       // Key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Indubmd4anpjdnpoamVhaW54ZnliIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MDg2NTYzMjAsImV4cCI6MjAyNDIzMjMyMH0.Z2LiHFSwJtsF1imYumfQ7YYDNksVrZmmpqXzio-XT90";
        _client = new Client(Url, Key);       
    }

    public string Url { get; }

    public async Task<(bool success, string url, string errorMessage)> UploadFileAsync(string filePath, string pathInBucket, string bucketName, string fileName)
    {
        if (!File.Exists(filePath))
            return (false, string.Empty, "File not found");

        try
        {
            await _client.InitializeAsync();

            var storage = _client.Storage;
            var bucket = storage.From(bucketName);       
            var fileBytes = File.ReadAllBytes(filePath);
            var pathUpload = $"{pathInBucket}/{fileName}";         

            var result = await bucket.Upload(fileBytes, pathUpload, new Supabase.Storage.FileOptions {Upsert = true});

            if (result == null)
            {
                return (false, string.Empty, $"Upload error: {result}");
            }

            var signedUrlResponse = await bucket.CreateSignedUrl(pathUpload, 157680000); // Expira em 5 anos

            if (signedUrlResponse == null)
            {
                return (true, string.Empty, signedUrlResponse);
            }

            return (true, signedUrlResponse, string.Empty);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Exception: " + ex.Message);
            return (false, string.Empty, ex.Message);
        }
    }
    public async Task<(bool success, string url, string errorMessage)> UploadFileFromUrlAsync(string fileUrl, string pathInBucket, string bucketName)
    {
        try
        {
            using var client = new HttpClient();
            var fileBytes = await client.GetByteArrayAsync(fileUrl);

            await _client.InitializeAsync();

            var storage = _client.Storage;
            var bucket = storage.From(bucketName);

            var result = await bucket.Upload(fileBytes, pathInBucket, new Supabase.Storage.FileOptions { Upsert = true });

            if (result == null)
            {
                return (false, string.Empty, $"Upload error: {result}");
            }

            var signedUrlResponse = await bucket.CreateSignedUrl(pathInBucket, 157680000); // Expira em 5 anos

            if (signedUrlResponse == null)
            {
                return (true, string.Empty, signedUrlResponse);
            }

            return (true, signedUrlResponse, string.Empty);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Exception: " + ex.Message);
            return (false, string.Empty, ex.Message);
        }
    }
}
