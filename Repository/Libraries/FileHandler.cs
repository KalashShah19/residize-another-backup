// using Microsoft.AspNetCore.Http;

// namespace Repository.Libraries;

// public static class FileHandler
// {
//     public static string StoreUserFile(IFormFile profilepicture)
//     {
//         if (profilepicture == null) return "userdata/default.jpeg";
//         string filename = $"userdata/{Guid.NewGuid()}{Path.GetExtension(profilepicture.FileName)}";
//         using FileStream fileStream = new($"wwwroot/{filename}", FileMode.Create);
//         profilepicture.CopyTo(fileStream);
//         return filename;
//     }

//     public static string StorePropertyImages(List<IFormFile> propertyImages, int propertyid)
//     {
//         string directoryname = $"wwwroot/propertydata/{propertyid}";
//         foreach (IFormFile image in propertyImages) if (!CheckFileSize(image.Length, 15)) throw new Exception("Uploaded Images should be less than 15 MB in size.");
//         if (!Directory.Exists(directoryname)) Directory.CreateDirectory(directoryname);
//         foreach (IFormFile image in propertyImages)
//         {
//             using FileStream fileStream = new($"{directoryname}/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}", FileMode.Create);
//             image.CopyTo(fileStream);
//         }
//         return directoryname;
//     }

//     public static void StorePropertyVideo(IFormFile propertyVideo, int propertyid)
//     {
//         if (propertyVideo == null) return;
//         if (!CheckFileSize(propertyVideo.Length, 100)) throw new Exception("Video size must be under 100 MB");
//         string filename = $"wwwroot/propertydata/{propertyid}/{Guid.NewGuid()}{Path.GetExtension(propertyVideo.FileName)}";
//         Directory.CreateDirectory($"wwwroot/propertydata/{propertyid}");
//         using FileStream fileStream = new($"{filename}", FileMode.Create);
//         propertyVideo.CopyTo(fileStream);
//     }

//     public static bool CheckFileSize(long actual, int target)
//     {
//         return target >= actual / (1024 * 1024);
//     }
// }
using Microsoft.AspNetCore.Http;

namespace Repository.Libraries;

public static class FileHandler
{
    public static string StoreUserFile(IFormFile profilepicture)
    {
        if (profilepicture == null) return "userdata/default.jpeg";
        string filename = $"userdata/{Guid.NewGuid()}{Path.GetExtension(profilepicture.FileName)}";
        using FileStream fileStream = new($"wwwroot/{filename}", FileMode.Create);
        profilepicture.CopyTo(fileStream);
        return filename;
    }

    public static string StorePropertyImages(List<IFormFile> propertyImages, int propertyid)
    {
        string directoryname = $"propertydata/{propertyid}";
        foreach (IFormFile image in propertyImages) if (!CheckFileSize(image.Length, 15)) throw new Exception("Uploaded Images should be less than 15 MB in size.");
        if (!Directory.Exists($"wwwroot/{directoryname}")) Directory.CreateDirectory($"wwwroot/{directoryname}");
        foreach (IFormFile image in propertyImages)
        {
            using FileStream fileStream = new($"wwwroot/{directoryname}/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}", FileMode.Create);
            image.CopyTo(fileStream);
        }
        return directoryname;
    }

    public static void StorePropertyVideo(IFormFile propertyVideo, int propertyid)
    {
        if (propertyVideo == null) return;
        if (!CheckFileSize(propertyVideo.Length, 100)) throw new Exception("Video size must be under 100 MB");
        string filename = $"propertydata/{propertyid}/{Guid.NewGuid()}{Path.GetExtension(propertyVideo.FileName)}";
        using FileStream fileStream = new($"wwwroot/{filename}", FileMode.Create);
        propertyVideo.CopyTo(fileStream);
    }

    public static bool CheckFileSize(long actual, int target)
    {
        return target >= actual / (1024 * 1024);
    }

    public static void DeleteFile(string filepath)
    {
        if (!File.Exists(filepath)) throw new Exception("File does not exists");
        File.Delete($@"wwwroot/{filepath}");
    }

    public static void DeleteDirectory(string directory)
    {
        if (!Directory.Exists($@"wwwroot/{directory}")) throw new Exception("Directory does not exists");
        Directory.Delete($@"wwwroot/{directory}", true);
    }

    public static string[] GetFiles(string directory)
    {
        string[] files = Directory.GetFiles(@$"wwwroot\{directory}");
        files = files.Select(s => s.Replace("wwwroot\\", "http://localhost:5293/")).ToArray();
        return files;
    }
    // public static
    public static string GetFirstImagePath(string folderPath)
    {
        var defaultPath = "https://www.shutterstock.com/image-vector/no-photo-blank-image-icon-260nw-1955339317.jpg";
        if(Directory.Exists(@$"wwwroot\{folderPath}")){
        var files = GetFiles(folderPath)
        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)|| f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (files.Length > 0) return files[0];
        else return defaultPath;
        }else return defaultPath;
    }
}
