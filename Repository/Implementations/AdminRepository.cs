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
public class AdminRepository : IAdminRepository
{
    private readonly NpgsqlConnection connection;
    public AdminRepository(IConfiguration configuration)
    {
        connection = new(configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING"));

    }

    public int TotalUsers()
    {
        int totalUsers = 0;


        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using (NpgsqlCommand TotalUsersCommand = new("SELECT COUNT(c_user_id) FROM t_users", connection))
        {
            using (NpgsqlDataReader reader = TotalUsersCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    totalUsers = reader.GetInt32(0);
                }
            }
        }

        return totalUsers;
    }


    public int TotalProperties()
    {
        int totalProperties = 0;
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }
        using (NpgsqlCommand TotalPropertiesCommand = new("SELECT COUNT(c_property_id) FROM t_properties", connection))
        {
            using (NpgsqlDataReader reader = TotalPropertiesCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    totalProperties = reader.GetInt32(0);
                }
            }
        }
        return totalProperties;
    }
}