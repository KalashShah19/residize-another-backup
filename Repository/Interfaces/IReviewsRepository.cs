using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Models;
namespace Repository.Interfaces
{
    public interface IReviewsRepository
    {
      public bool DoesReviewExist(int UserId); 
      public void AddReviews(Reviews.Post reviews);
      public List<Reviews.Get> ViewLatestReviews(int id);
      bool RemoveReview(int id);
    }
}