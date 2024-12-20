using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IProjectRepository
    {
        public int AddProject(Project.Post Projectdetails);
        public bool DoesProjectExist(string ProjectName);
        public bool DeleteProject(int projectId);
        public int UpdateProject(Project.Update Projectdetails);
        public void AssetsMange(string filePath);
        public List<Project.GET> GetProjects(int id);
        public void AllAmenities(Project.Amenities amenities);
        public List<Project.GET> GetAllProjects(); 
        public Project.GET GetProjectByProjectId(int id);
        public List<Properties.ListGet> GetPropertiesByProjectId(int projectId);
        public List<Amenities.Get> GetProjectAmenitiesByProjectId(int projectId);

    }
}