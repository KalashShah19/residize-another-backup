using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Models;
using Repository.Interfaces;
using Repository.Libraries;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Repository.Implementations;

public class ReviewsRepository : IReviewsRepository
{
    private readonly NpgsqlConnection connection;
    public ReviewsRepository(IConfiguration configuration)
    {
        connection = new(configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING"));
        connection.Open();
    }
    public void AddReviews(Reviews.Post reviews)
    {
        if (!DoesUserExist(reviews.UserId))
        {
            throw new UserException("The specified user does not exist.");
        }
        if (DoesReviewExist(reviews.UserId)) throw new UserException("One Review Already Posted.");
        NpgsqlCommand addReviewCommand = new("INSERT INTO t_Reviews(c_rating,c_comments,c_user_id) VALUES (@rating,@comments,@userid)", connection);
        int currentYear = DateTime.Now.Year;
        addReviewCommand.Parameters.AddWithValue("rating", reviews.Rating);
        addReviewCommand.Parameters.AddWithValue("comments", reviews.Comments);
        addReviewCommand.Parameters.AddWithValue("userid", reviews.UserId);
        addReviewCommand.ExecuteNonQuery();
    }
    private bool DoesUserExist(int userId)
    {
        NpgsqlCommand checkUserCommand = new("SELECT c_user_id FROM t_users WHERE c_user_id = @userid", connection);
        checkUserCommand.Parameters.AddWithValue("userid", userId);
        using (var reader = checkUserCommand.ExecuteReader())
        {
            return reader.HasRows;
        }
    }
    public bool DoesReviewExist(int UserId)
    {
        NpgsqlCommand checkReviewCommand = new("SELECT c_user_id FROM t_Reviews where c_user_id = @userid", connection);
        checkReviewCommand.Parameters.AddWithValue("userid", UserId);
        using NpgsqlDataReader reader = checkReviewCommand.ExecuteReader();
        return reader.HasRows;
    }
    public List<Reviews.Get> ViewLatestReviews(int id)
    {
        List<Reviews.Get> reviews = new();
        string query = GetReviewQuery("=");
        string NotMineQuery = GetReviewQuery("<>");
        reviews.AddRange(GetListReviews(query, id));
        reviews.AddRange(GetListReviews(NotMineQuery, id));
        return reviews;
    }
    public List<Reviews.Get> GetListReviews(string query, int id)
    {
        var reviews = new List<Reviews.Get>();
        using NpgsqlCommand fetchLatestReviewCommand = new(query, connection);
        fetchLatestReviewCommand.Parameters.AddWithValue("id", id);
        using (NpgsqlDataReader reader = fetchLatestReviewCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                Reviews.Get review = new()
                {
                    Rating = reader.GetInt16(0),
                    Comments = reader.GetString(1),
                    CreatedAt = reader.GetDateTime(2) + "",
                    ImagePath = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                    UserId = reader.GetInt32(4),
                    Username = reader.GetString(5)
                };
                reviews.Add(review);
            }
        }
        return reviews;
    }
    public bool RemoveReview(int id)
    {
        using NpgsqlCommand remCmd = new("DELETE FROM t_reviews WHERE c_user_id=@userId", connection);
        remCmd.Parameters.AddWithValue("userId", id);
        return remCmd.ExecuteNonQuery() > 0;
    }
    public string GetReviewQuery(string op)
    {
        return $"SELECT t_Reviews.c_rating, t_Reviews.c_comments, t_Reviews.c_created_at, t_users.c_image_path,t_Reviews.c_user_id,concat(t_users.c_first_name,' ',t_users.c_last_name) FROM t_Reviews JOIN t_users ON t_Reviews.c_user_id = t_users.c_user_id where t_Reviews.c_user_id {op} @id ORDER BY t_Reviews.c_rating DESC,t_Reviews.c_created_at DESC ";
    }

    public void UpdateReview(Reviews.Post reviews)
    {
        if (!DoesUserExist(reviews.UserId))
        {
            throw new UserException("The specified user does not exist.");
        }
        if (!DoesReviewExist(reviews.UserId)) throw new UserException("No reviews found from this user.");
        NpgsqlCommand updateReviewCommand = new("UPDATE t_Reviews SET c_rating = @rating, c_comments = @comments WHERE c_user_id  = @userid", connection);
        updateReviewCommand.Parameters.AddWithValue("rating", reviews.Rating);
        updateReviewCommand.Parameters.AddWithValue("comments", reviews.Comments);
        updateReviewCommand.Parameters.AddWithValue("userid", reviews.UserId);
        updateReviewCommand.ExecuteNonQuery();
    }
}
