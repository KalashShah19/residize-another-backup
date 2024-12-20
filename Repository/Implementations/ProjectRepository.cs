using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Repository.Interfaces;
using Repository.Libraries;
using Repository.Models;

namespace Repository.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly NpgsqlConnection connection;
        public ProjectRepository(IConfiguration configuration)
        {
            connection = new(configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING"));
            connection.Open();
        }

        public int AddProject(Project.Post Projectdetails)
        {
            string ImagePath = "";
            string VideoPath = "";
            string Borchure = "";

            long maxTotalImageSize = 15 * 1024 * 1024 * 5; // 75 MB for 5 images
            long ImageSize = 15 * 1024 * 1024;
            long maxTotalVideoSize = 100 * 1024 * 1024;  // 100 MB for 4 videos
            long maxFolderSize = 185 * 1024 * 1024;  // Folder size limit for both

            if (Projectdetails.Files != null && Projectdetails.Files.Any())
            {
                string propertyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", Projectdetails.ProjectDetailsId.ToString(), "Images");
                ImagePath = propertyDirectory;

                if (!Directory.Exists(propertyDirectory))
                {
                    Directory.CreateDirectory(propertyDirectory);
                }

                // Check number of images already present
                int existingImageCount = Directory.GetFiles(propertyDirectory, "*.jpg", SearchOption.TopDirectoryOnly)
                                                .Concat(Directory.GetFiles(propertyDirectory, "*.jpeg", SearchOption.TopDirectoryOnly))
                                                .Concat(Directory.GetFiles(propertyDirectory, "*.png", SearchOption.TopDirectoryOnly))
                                                .Concat(Directory.GetFiles(propertyDirectory, "*.gif", SearchOption.TopDirectoryOnly))
                                                .Count();

                int imageCount = existingImageCount;
                long totalImageSize = 0;

                foreach (var file in Projectdetails.Files)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        throw new Exception($"Invalid file type for Images. File '{file.FileName}' is not supported.");
                    }

                    imageCount++;
                    totalImageSize += file.Length;
                    Console.WriteLine("imageCount = " + imageCount);
                    Console.WriteLine("maxTotalImageSize = " + maxTotalImageSize);
                    if (file.Length > ImageSize)
                    {
                        throw new Exception($"The image file '{file.FileName}' exceeds the maximum size limit of 15 MB.");
                    }

                    // Validate image count and size limits
                    if (imageCount > 5 || totalImageSize > maxTotalImageSize)
                    {
                        throw new Exception("The total file count or size for Images exceeds the allowed limit.");
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    string fullFilePath = Path.Combine(propertyDirectory, uniqueFileName);

                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            if (Projectdetails.VideoFiles != null && Projectdetails.VideoFiles.Any())
            {
                string propertyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", Projectdetails.ProjectDetailsId.ToString(), "Videos");
                VideoPath = propertyDirectory;

                if (!Directory.Exists(propertyDirectory))
                {
                    Directory.CreateDirectory(propertyDirectory);
                }

                // Check number of videos already present
                int existingVideoCount = Directory.GetFiles(propertyDirectory, "*.mp4", SearchOption.TopDirectoryOnly)
                                                   .Concat(Directory.GetFiles(propertyDirectory, "*.avi", SearchOption.TopDirectoryOnly))
                                                   .Concat(Directory.GetFiles(propertyDirectory, "*.mov", SearchOption.TopDirectoryOnly))
                                                   .Concat(Directory.GetFiles(propertyDirectory, "*.mkv", SearchOption.TopDirectoryOnly))
                                                   .Count();

                int videoCount = existingVideoCount;
                long totalVideoSize = 0;

                foreach (var file in Projectdetails.VideoFiles)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".mp4" && extension != ".avi" && extension != ".mov" && extension != ".mkv")
                    {
                        throw new Exception($"Invalid file type for Videos. File '{file.FileName}' is not supported.");
                    }

                    videoCount++;
                    totalVideoSize += file.Length;

                    // Validate video count and size limits
                    if (videoCount > 1 || totalVideoSize > maxTotalVideoSize)
                    {
                        throw new Exception("The total file count or size for Videos exceeds the allowed limit.");
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    string fullFilePath = Path.Combine(propertyDirectory, uniqueFileName);

                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            if (Projectdetails.BorchureFile != null)
            {
                string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", Projectdetails.ProjectDetailsId.ToString(), "Brochure");
                Borchure = baseDirectory;

                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                string extension = Path.GetExtension(Projectdetails.BorchureFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    throw new Exception("Only PDF files are allowed for brochures.");
                }

                if (Projectdetails.BorchureFile.Length > 10 * 1024 * 1024)
                {
                    throw new Exception("The brochure file size exceeds the maximum limit of 10 MB.");
                }

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string fullFilePath = Path.Combine(baseDirectory, uniqueFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    Projectdetails.BorchureFile.CopyTo(stream);
                }
            }

            Console.WriteLine("Borchurepath = " + Borchure);
            Console.WriteLine("videopath = " + VideoPath);
            Console.WriteLine("imagepath = " + ImagePath);



            if (DoesProjectExist(Projectdetails.ProjectName))
                throw new UserException("This Project Name is already registered.");
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            NpgsqlCommand ProjectCommand = new NpgsqlCommand(@"INSERT INTO t_projects (c_project_name, c_picture_path, c_brochure_path, c_property_detail_id,c_video_path) VALUES (@ProjectName, @PicturePath, @BorchurePath, @ProjectDetailsId,@VideoPath)RETURNING c_project_id;", connection);

            ProjectCommand.Parameters.AddWithValue("@ProjectName", Projectdetails.ProjectName);
            ProjectCommand.Parameters.AddWithValue("@PicturePath", ImagePath);
            ProjectCommand.Parameters.AddWithValue("@VideoPath", VideoPath);
            ProjectCommand.Parameters.AddWithValue("@BorchurePath", Borchure);
            ProjectCommand.Parameters.AddWithValue("@ProjectDetailsId", Projectdetails.ProjectDetailsId);

            // Execute the command and retrieve the inserted ID
            object insertedId = ProjectCommand.ExecuteScalar();
            return Convert.ToInt32(insertedId);

        }

        public bool DoesProjectExist(string ProjectName)
        {
            Console.WriteLine("exits");
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            NpgsqlCommand checkProjectCommand = new NpgsqlCommand("SELECT c_project_name FROM t_projects WHERE c_project_name = @ProjectName", connection);
            checkProjectCommand.Parameters.AddWithValue("ProjectName", ProjectName);

            var result = checkProjectCommand.ExecuteScalar();
            return result != null;

        }

        public int UpdateProject(Project.Update Projectdetails)
        {
            string ImagePath = "";
            string VideoPath = "";
            string Borchure = "";

            long maxTotalImageSize = 15 * 1024 * 1024 * 5; // 75 MB for 5 images
            long ImageSize = 15 * 1024 * 1024;
            long maxTotalVideoSize = 100 * 1024 * 1024;  // 100 MB for 4 videos
            long maxFolderSize = 185 * 1024 * 1024;  // Folder size limit for both

            if (Projectdetails.Files != null && Projectdetails.Files.Any())
            {
                string propertyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", Projectdetails.ProjectDetailsId.ToString(), "Images");
                ImagePath = propertyDirectory;

                if (!Directory.Exists(propertyDirectory))
                {
                    Directory.CreateDirectory(propertyDirectory);
                }

                // Check number of images already present
                int existingImageCount = Directory.GetFiles(propertyDirectory, "*.jpg", SearchOption.TopDirectoryOnly)
                                                .Concat(Directory.GetFiles(propertyDirectory, "*.jpeg", SearchOption.TopDirectoryOnly))
                                                .Concat(Directory.GetFiles(propertyDirectory, "*.png", SearchOption.TopDirectoryOnly))
                                                .Concat(Directory.GetFiles(propertyDirectory, "*.gif", SearchOption.TopDirectoryOnly))
                                                .Count();

                int imageCount = existingImageCount;
                long totalImageSize = 0;

                foreach (var file in Projectdetails.Files)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        throw new Exception($"Invalid file type for Images. File '{file.FileName}' is not supported.");
                    }

                    imageCount++;
                    totalImageSize += file.Length;
                    Console.WriteLine("imageCount = " + imageCount);
                    Console.WriteLine("maxTotalImageSize = " + maxTotalImageSize);
                    if (file.Length > ImageSize)
                    {
                        throw new Exception($"The image file '{file.FileName}' exceeds the maximum size limit of 15 MB.");
                    }

                    // Validate image count and size limits
                    if (imageCount > 5 || totalImageSize > maxTotalImageSize)
                    {
                        throw new Exception("The total file count or size for Images exceeds the allowed limit.");
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    string fullFilePath = Path.Combine(propertyDirectory, uniqueFileName);

                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            if (Projectdetails.VideoFiles != null && Projectdetails.VideoFiles.Any())
            {
                string propertyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", Projectdetails.ProjectDetailsId.ToString(), "Videos");
                VideoPath = propertyDirectory;

                if (!Directory.Exists(propertyDirectory))
                {
                    Directory.CreateDirectory(propertyDirectory);
                }

                // Check number of videos already present
                int existingVideoCount = Directory.GetFiles(propertyDirectory, "*.mp4", SearchOption.TopDirectoryOnly)
                                                   .Concat(Directory.GetFiles(propertyDirectory, "*.avi", SearchOption.TopDirectoryOnly))
                                                   .Concat(Directory.GetFiles(propertyDirectory, "*.mov", SearchOption.TopDirectoryOnly))
                                                   .Concat(Directory.GetFiles(propertyDirectory, "*.mkv", SearchOption.TopDirectoryOnly))
                                                   .Count();

                int videoCount = existingVideoCount;
                long totalVideoSize = 0;

                foreach (var file in Projectdetails.VideoFiles)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".mp4" && extension != ".avi" && extension != ".mov" && extension != ".mkv")
                    {
                        throw new Exception($"Invalid file type for Videos. File '{file.FileName}' is not supported.");
                    }

                    videoCount++;
                    totalVideoSize += file.Length;

                    // Validate video count and size limits
                    if (videoCount > 1 || totalVideoSize > maxTotalVideoSize)
                    {
                        throw new Exception("The total file count or size for Videos exceeds the allowed limit.");
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    string fullFilePath = Path.Combine(propertyDirectory, uniqueFileName);

                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            if (Projectdetails.BorchureFile != null)
            {
                string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", Projectdetails.ProjectDetailsId.ToString(), "Brochure");
                Borchure = baseDirectory;

                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                string extension = Path.GetExtension(Projectdetails.BorchureFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    throw new Exception("Only PDF files are allowed for brochures.");
                }

                if (Projectdetails.BorchureFile.Length > 10 * 1024 * 1024)
                {
                    throw new Exception("The brochure file size exceeds the maximum limit of 10 MB.");
                }

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string fullFilePath = Path.Combine(baseDirectory, uniqueFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    Projectdetails.BorchureFile.CopyTo(stream);
                }
            }

            Console.WriteLine("Borchurepath = " + Borchure);
            Console.WriteLine("videopath = " + VideoPath);
            Console.WriteLine("imagepath = " + ImagePath);




            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }



            NpgsqlCommand UpdateProjectCommand = new NpgsqlCommand(@"UPDATE public.t_projects SET  c_project_name=@ProjectName WHERE c_property_detail_id = @ProjectDetailsId;", connection);

            UpdateProjectCommand.Parameters.AddWithValue("@ProjectName", Projectdetails.ProjectName);
            UpdateProjectCommand.Parameters.AddWithValue("@ProjectDetailsId", Projectdetails.ProjectDetailsId);

            int UpdateProjectId = UpdateProjectCommand.ExecuteNonQuery();
            return UpdateProjectId;

        }

        public void AssetsMange(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                    Console.WriteLine($"File deleted successfully: {filePath}");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error deleting file at {filePath}.", ex);
                }
            }
            else
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

        }

        public bool DeleteProject(int projectId)
        {

            int ProjectDetailsId = 0;

            NpgsqlCommand SelectProjectIdCommand = new NpgsqlCommand(@"SELECT c_property_detail_id FROM t_projects WHERE c_project_id = @c_project_id", connection);
            SelectProjectIdCommand.Parameters.AddWithValue("@c_project_id", projectId);

            var result = SelectProjectIdCommand.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                ProjectDetailsId = Convert.ToInt32(result);
            }

            Console.WriteLine("delete id = " + ProjectDetailsId);

            NpgsqlCommand DeleteProjectCommand = new NpgsqlCommand(@"DELETE FROM t_projects WHERE c_project_id = @c_project_id", connection);
            DeleteProjectCommand.Parameters.AddWithValue("@c_project_id", projectId);
            int rowForProject = DeleteProjectCommand.ExecuteNonQuery();
            if (rowForProject > 0)
            {

                NpgsqlCommand DeleteProjectDetailCommand = new NpgsqlCommand(@"DELETE FROM t_details WHERE c_property_detail_id=@ProjectDetailsId", connection);
                DeleteProjectDetailCommand.Parameters.AddWithValue("@ProjectDetailsId", ProjectDetailsId);

                int row = DeleteProjectDetailCommand.ExecuteNonQuery();


                if (row > 0)
                {

                    Console.WriteLine("Row is delete . ");
                    Console.WriteLine("delete id = " + ProjectDetailsId);

                    string propertyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProjectdetailsData", ProjectDetailsId.ToString());

                    if (Directory.Exists(propertyDirectory))
                    {
                        Directory.Delete(propertyDirectory, true);
                    }



                    return true;

                }
            }

            return true; // Return success status
        }

        public List<Project.GET> GetProjects(int id)
        {
            List<Project.GET> projects = new List<Project.GET>();

            NpgsqlCommand GetProjectForUser = new NpgsqlCommand(@"SELECT P.c_project_id,P.c_project_name, P.c_picture_path, P.c_brochure_path, P.c_is_rera_verified, P.c_property_detail_id, P.c_video_path, D.c_property_age, D.c_availability_status, D.c_city, D.c_locality, D.c_address, D.c_pin_code, D.c_posted_date, D.c_user_id 
                                                         FROM t_projects AS P 
                                                         JOIN t_details AS D ON P.c_property_detail_id = D.c_property_detail_id 
                                                         WHERE D.c_user_id = @ID", connection);
            GetProjectForUser.Parameters.AddWithValue("@ID", id);

            using (var reader = GetProjectForUser.ExecuteReader())
            {
                while (reader.Read())
                {
                    var project = new Project.GET
                    {
                        ProjectId = reader.GetInt32(reader.GetOrdinal("c_project_id")),
                        ProjectName = reader.GetString(reader.GetOrdinal("c_project_name")),
                        PicturePath = reader.IsDBNull(reader.GetOrdinal("c_picture_path")) ? null : reader.GetString(reader.GetOrdinal("c_picture_path")),
                        BrochurePath = reader.IsDBNull(reader.GetOrdinal("c_brochure_path")) ? null : reader.GetString(reader.GetOrdinal("c_brochure_path")),
                        IsReraVerified = reader.GetBoolean(reader.GetOrdinal("c_is_rera_verified")),
                        PropertyDetailId = reader.GetInt32(reader.GetOrdinal("c_property_detail_id")),
                        VideoPath = reader.IsDBNull(reader.GetOrdinal("c_video_path")) ? null : reader.GetString(reader.GetOrdinal("c_video_path")),
                        PropertyAge = reader.GetInt32(reader.GetOrdinal("c_property_age")),
                        AvailabilityStatus = reader.GetBoolean(reader.GetOrdinal("c_availability_status")),
                        City = reader.GetString(reader.GetOrdinal("c_city")),
                        Locality = reader.GetString(reader.GetOrdinal("c_locality")),
                        Address = reader.GetString(reader.GetOrdinal("c_address")),
                        PinCode = reader.GetInt32(reader.GetOrdinal("c_pin_code")),
                        PostedDate = reader.GetDateTime(reader.GetOrdinal("c_posted_date")),
                        UserId = reader.IsDBNull(reader.GetOrdinal("c_user_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("c_user_id"))
                    };

                    // List files in the directories
                    if (!string.IsNullOrEmpty(project.PicturePath) && Directory.Exists(project.PicturePath))
                    {
                        project.PictureFiles.AddRange(Directory.GetFiles(project.PicturePath));
                    }

                    if (!string.IsNullOrEmpty(project.BrochurePath) && Directory.Exists(project.BrochurePath))
                    {
                        project.BrochureFiles.AddRange(Directory.GetFiles(project.BrochurePath));
                    }

                    if (!string.IsNullOrEmpty(project.VideoPath) && Directory.Exists(project.VideoPath))
                    {
                        project.VideoFiles.AddRange(Directory.GetFiles(project.VideoPath));
                    }

                    projects.Add(project);
                }
            }

            return projects;
        }

        public void AllAmenities(Project.Amenities amenities)
        {
            foreach (var amenity in amenities.AllAmenities)
            {
                Console.WriteLine(amenity);
                NpgsqlCommand AllAmenitiesCommad = new NpgsqlCommand(@"INSERT INTO public.t_property_amenities(c_amenities_id, c_property_detail_id)VALUES (@amenity, @ProjectDetailsId);", connection);

                AllAmenitiesCommad.Parameters.AddWithValue("@amenity", amenity);
                AllAmenitiesCommad.Parameters.AddWithValue("@ProjectDetailsId", amenities.ProjectDetailsId);
                AllAmenitiesCommad.ExecuteNonQuery();
            }
        }

        public List<Project.GET> GetAllProjects()
        {
            List<Project.GET> projects = new List<Project.GET>();

            NpgsqlCommand GetProjectForUser = new NpgsqlCommand(@"SELECT P.c_project_id, P.c_project_name, P.c_picture_path, P.c_brochure_path, P.c_is_rera_verified, P.c_property_detail_id, P.c_video_path, D.c_property_age, D.c_availability_status, D.c_city, D.c_locality, D.c_address, D.c_pin_code, D.c_posted_date, D.c_user_id 
                                                         FROM t_projects AS P 
                                                         JOIN t_details AS D ON P.c_property_detail_id = D.c_property_detail_id ", connection);
            // GetProjectForUser.Parameters.AddWithValue("@ID", id);

            using (var reader = GetProjectForUser.ExecuteReader())
            {
                while (reader.Read())
                {
                    var project = new Project.GET
                    {
                        ProjectId = Convert.ToInt32(reader["c_project_id"]),
                        ProjectName = reader.GetString(reader.GetOrdinal("c_project_name")),
                        PicturePath = reader.IsDBNull(reader.GetOrdinal("c_picture_path")) ? null : reader.GetString(reader.GetOrdinal("c_picture_path")),
                        BrochurePath = reader.IsDBNull(reader.GetOrdinal("c_brochure_path")) ? null : reader.GetString(reader.GetOrdinal("c_brochure_path")),
                        IsReraVerified = reader.GetBoolean(reader.GetOrdinal("c_is_rera_verified")),
                        PropertyDetailId = reader.GetInt32(reader.GetOrdinal("c_property_detail_id")),
                        VideoPath = reader.IsDBNull(reader.GetOrdinal("c_video_path")) ? null : reader.GetString(reader.GetOrdinal("c_video_path")),
                        PropertyAge = reader.GetInt32(reader.GetOrdinal("c_property_age")),
                        AvailabilityStatus = reader.GetBoolean(reader.GetOrdinal("c_availability_status")),
                        City = reader.GetString(reader.GetOrdinal("c_city")),
                        Locality = reader.GetString(reader.GetOrdinal("c_locality")),
                        Address = reader.GetString(reader.GetOrdinal("c_address")),
                        PinCode = reader.GetInt32(reader.GetOrdinal("c_pin_code")),
                        PostedDate = reader.GetDateTime(reader.GetOrdinal("c_posted_date")),
                        UserId = reader.IsDBNull(reader.GetOrdinal("c_user_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("c_user_id"))
                    };

                    // List files in the directories
                    if (!string.IsNullOrEmpty(project.PicturePath) && Directory.Exists(project.PicturePath))
                    {
                        project.PictureFiles.AddRange(Directory.GetFiles(project.PicturePath));
                    }

                    if (!string.IsNullOrEmpty(project.BrochurePath) && Directory.Exists(project.BrochurePath))
                    {
                        project.BrochureFiles.AddRange(Directory.GetFiles(project.BrochurePath));
                    }

                    if (!string.IsNullOrEmpty(project.VideoPath) && Directory.Exists(project.VideoPath))
                    {
                        project.VideoFiles.AddRange(Directory.GetFiles(project.VideoPath));
                    }
                    projects.Add(project);
                }
            }
            return projects;
        }
        public Project.GET GetProjectByProjectId(int id)
        {
            Project.GET project = new();

            NpgsqlCommand GetProjectForUser = new NpgsqlCommand(@"SELECT P.c_project_id, P.c_project_name, P.c_picture_path, P.c_brochure_path, P.c_is_rera_verified, P.c_property_detail_id, P.c_video_path, D.c_property_age, D.c_availability_status, D.c_city, D.c_locality, D.c_address, D.c_pin_code, D.c_posted_date, D.c_user_id 
                                                         FROM t_projects AS P 
                                                         JOIN t_details AS D ON P.c_property_detail_id = D.c_property_detail_id where P.c_project_id = @id", connection);
            GetProjectForUser.Parameters.AddWithValue("@ID", id);

            using (var reader = GetProjectForUser.ExecuteReader())
            {
                if (reader.Read())
                {
                    project = new Project.GET()
                    {
                        ProjectId = Convert.ToInt32(reader["c_project_id"]),
                        ProjectName = reader.GetString(reader.GetOrdinal("c_project_name")),
                        PicturePath = reader.IsDBNull(reader.GetOrdinal("c_picture_path")) ? null : reader.GetString(reader.GetOrdinal("c_picture_path")),
                        BrochurePath = reader.IsDBNull(reader.GetOrdinal("c_brochure_path")) ? null : reader.GetString(reader.GetOrdinal("c_brochure_path")),
                        IsReraVerified = reader.GetBoolean(reader.GetOrdinal("c_is_rera_verified")),
                        PropertyDetailId = reader.GetInt32(reader.GetOrdinal("c_property_detail_id")),
                        VideoPath = reader.IsDBNull(reader.GetOrdinal("c_video_path")) ? null : reader.GetString(reader.GetOrdinal("c_video_path")),
                        PropertyAge = reader.GetInt32(reader.GetOrdinal("c_property_age")),
                        AvailabilityStatus = reader.GetBoolean(reader.GetOrdinal("c_availability_status")),
                        City = reader.GetString(reader.GetOrdinal("c_city")),
                        Locality = reader.GetString(reader.GetOrdinal("c_locality")),
                        Address = reader.GetString(reader.GetOrdinal("c_address")),
                        PinCode = reader.GetInt32(reader.GetOrdinal("c_pin_code")),
                        PostedDate = reader.GetDateTime(reader.GetOrdinal("c_posted_date")),
                        UserId = reader.IsDBNull(reader.GetOrdinal("c_user_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("c_user_id"))
                    };

                    // List files in the directories

                    project.PictureFiles = FileHandler.GetFiles(project.PicturePath).ToList<string>();



                    project.BrochureFiles = FileHandler.GetFiles(project.BrochurePath).ToList<string>();



                    project.VideoFiles = FileHandler.GetFiles(project.VideoPath).ToList<string>();

                    return project;
                }
            }

            return project;
        }

        public List<Properties.ListGet> GetPropertiesByProjectId(int projectId)
        {
            List<Properties.ListGet> properties = new();

            // Updated query to fetch properties based on the given projectId
            using NpgsqlCommand propertyComd = new(@"SELECT 
                                                p.c_property_id,
                                                p.c_project_id,
                                                pro.c_project_name,
                                                p.c_property_name,
                                                p.c_property_square_ft,
                                                p.c_property_amount,
                                                p.c_bhk,
                                                p.c_area_type,
                                                p.c_property_type,
                                                p.c_property_listing_type,
                                                p.c_furnished,
                                                p.c_picture_path,
                                                d.c_property_age,
                                                d.c_availability_status,
                                                d.c_address,
                                                d.c_locality,
                                                d.c_city
                                            FROM 
                                                t_properties p
                                            JOIN 
                                                t_details d ON p.c_property_detail_id = d.c_property_detail_id
                                            LEFT JOIN 
                                                t_projects pro ON p.c_project_id = pro.c_project_id
                                            WHERE 
                                                p.c_project_id = @ProjectId", connection);

            // Add parameter for the projectId
            propertyComd.Parameters.AddWithValue("@ProjectId", projectId);

            using var reader = propertyComd.ExecuteReader();
            
                while (reader.Read())
                {
                    Properties.ListGet property = new()
                    {
                        PropertyId = Convert.ToInt32(reader["c_property_id"]),
                        ProjectId = reader["c_project_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["c_project_id"]),
                        ProjectName = reader["c_project_name"] == DBNull.Value ? (string?)null : Convert.ToString(reader["c_project_name"]),
                        PropertyName = Convert.ToString(reader["c_property_name"]),
                        PropertyAmount = Convert.ToInt32(reader["c_property_amount"]),
                        PropertySize = Convert.ToInt32(reader["c_property_square_ft"]),
                        Bedrooms = Convert.ToInt32(reader["c_bhk"]),
                        PropertyListingType = Convert.ToString(reader["c_property_listing_type"]),
                        Furnished = Convert.ToString(reader["c_furnished"]),
                        AreaType = Convert.ToString(reader["c_area_type"]),
                        PropertyAge = Convert.ToInt32(reader["c_property_age"]),
                        PropertyPicture = FileHandler.GetFirstImagePath(Convert.ToString(reader["c_picture_path"])),
                        City = Convert.ToString(reader["c_city"]),
                        Locality = Convert.ToString(reader["c_locality"]),
                        ReadytoMove = Convert.ToBoolean(reader["c_availability_status"]),
                        Address = Convert.ToString(reader["c_address"]),
                        PropertyType = Convert.ToString(reader["c_property_type"])
                    };
                    properties.Add(property);
                }
            
            return properties;
        }

        public List<Amenities.Get> GetProjectAmenitiesByProjectId(int projectId)
        {
            List<Amenities.Get> amenities = new();

            // Query to fetch amenities for the given projectId
            using NpgsqlCommand amenityComd = new(@"SELECT a.c_amenities_id, a.c_amenities_name
FROM public.t_property_amenities pa
JOIN public.t_amenities a ON pa.c_amenities_id = a.c_amenities_id
JOIN public.t_details d ON pa.c_property_detail_id = d.c_property_detail_id
JOIN public.t_projects p ON d.c_property_detail_id = p.c_property_detail_id
WHERE p.c_project_id =@ProjectId", connection);

            // Add parameter for the projectId
            amenityComd.Parameters.AddWithValue("@ProjectId", projectId);

            using var reader = amenityComd.ExecuteReader();

            while (reader.Read())
            {
                Amenities.Get amenity = new();
                amenity.id = Convert.ToInt32(reader["c_amenities_id"]);
                amenity.name = Convert.ToString(reader["c_amenities_name"]);
                amenities.Add(amenity);
            }


            return amenities;
        }

    }
}
