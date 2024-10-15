using MySqlConnector;
using SampleApi.Models;
using System.Security.AccessControl;
using System.Text.Json;

namespace SampleApi.Services;

public class DbRepo(ILogger<DbRepo> logger, MySqlDataSource db) : IDbRepo
{
    private readonly ILogger<DbRepo> logger = logger;
    private readonly MySqlDataSource db = db;

    #region Helper
    private static object? GetObjectValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else return obj;
    }

    private static string? GetStringValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else return obj.ToString();
    }

    private static byte[]? GetByteArrayValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else return (byte[])obj;
    }

    private static DateTime? GetDateTimeValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else return Convert.ToDateTime(obj);
    }

    private static double? GetDoubleValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else
        {
            return obj.ToString() == null ? null : double.Parse(obj.ToString());
        }
    }
    private static int? GetIntValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else
        {
            return obj.ToString() == null ? null : int.Parse(obj.ToString());
        }
    }

    private static long? GetLongValue(object obj)
    {
        if (obj == DBNull.Value) return null;
        else
        {
            return obj.ToString() == null ? null : long.Parse(obj.ToString());
        }
    }

    /// <summary>
    /// Already handled if value is NULL or empty or whitespace
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static Dictionary<string, int> DeserializeDict(string value)
    {
        try
        {
            return string.IsNullOrWhiteSpace(value) ? [] : JsonSerializer.Deserialize<Dictionary<string, int>>(value);
        }
        catch
        {
            return [];
        }
    }

    private static List<T> DeserializeList<T>(string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return new();

            var t = JsonSerializer.Deserialize<List<T>>(value);
            var generic = (List<T>)Convert.ChangeType(t, typeof(List<T>));

            return generic;
        }
        catch
        {
            return new();
        }
    }
    #endregion

    public async Task<ResponseBase> AddContactAsync(ContactBase c)
    {
        try
        {
            ResponseBase resp = new() { IsSuccess = false };
            string query =
                "INSERT INTO contact (firstname, lastname, num) VALUES " +
                "(@a, @b, @c);";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(query, connection);

            cmd.Parameters.AddWithValue("@a", c.FirstName);
            cmd.Parameters.AddWithValue("@b", c.LastName);
            cmd.Parameters.AddWithValue("@c", c.PhoneNumber);

            await cmd.ExecuteNonQueryAsync();
            resp = new()
            {
                IsSuccess = true,
                Message = $"Contact '{c.FirstName} {c.LastName}' added"
            };

            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(AddContactAsync)}: {ex.Message}");
            return new()
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ResponseBase> EditContactAsync(int id, ContactBase c)
    {
        try
        {
            ResponseBase resp = new() { IsSuccess = false };
            string query =
                "UPDATE contact SET firstname = @a, lastname = @b, num = @c WHERE id = @id;";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(query, connection);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@a", c.FirstName);
            cmd.Parameters.AddWithValue("@b", c.LastName);
            cmd.Parameters.AddWithValue("@c", c.PhoneNumber);

            await cmd.ExecuteNonQueryAsync();
            resp = new()
            {
                IsSuccess = true,
                Message = $"Contact '{id}' updated"
            };


            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(EditContactAsync)}: {ex.Message}");
            return new()
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ResponseBase> DeleteContactAsync(int id)
    {
        try
        {
            ResponseBase resp = new() { IsSuccess = false };
            string query =
                "DELETE FROM contact WHERE id = @a;";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(query, connection);

            cmd.Parameters.AddWithValue("@a", id);

            await cmd.ExecuteNonQueryAsync();
            resp = new()
            {
                IsSuccess = true,
                Message = $"Contact '{id}' deleted"
            };


            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(DeleteContactAsync)}: {ex.Message}");
            return new()
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<int> CountAllContactAsync()
    {
        try
        {
            int data = 0;
            string sql = "SELECT COUNT(id) FROM contact;";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(sql, connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data = GetIntValue(reader[0]).Value;
            }

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(CountAllContactAsync)}: {ex.Message}");
            return 0;
        }
    }

    public async Task<Contact> GetContactAsync(int id)
    {
        try
        {
            Contact data = null;
            string sql = "SELECT * FROM contact WHERE id = @id;";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data = new()
                {
                    Id = id,
                    FirstName = GetStringValue(reader["firstname"]),
                    LastName = GetStringValue(reader["lastname"]),
                    PhoneNumber = GetStringValue(reader["num"]),
                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value,
                    UpdateTime = GetDateTimeValue(reader["update_time"]).Value
                };
            }


            return data;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(GetContactAsync)}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Contact>> ListAllContactAsync()
    {
        try
        {
            List<Contact> data = [];
            string sql = "SELECT * FROM contact;";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(sql, connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.Add(new()
                {
                    Id = GetIntValue(reader["id"]).Value,
                    FirstName = GetStringValue(reader["firstname"]),
                    LastName = GetStringValue(reader["lastname"]),
                    PhoneNumber = GetStringValue(reader["num"]),
                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value,
                    UpdateTime = GetDateTimeValue(reader["update_time"]).Value
                });
            }

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(ListAllContactAsync)}: {ex.Message}");
            return [];
        }
    }

    public async Task<List<Contact>> SearchContactAsync(string keyword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return [];

            List<Contact> data = [];

            string sql = $"SELECT * FROM contact WHERE firstname LIKE '%{keyword}%' OR lastname LIKE '%{keyword}%' OR num LIKE '%{keyword}%';";

            await using MySqlConnection connection = await db.OpenConnectionAsync();
            await using MySqlCommand cmd = new(sql, connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.Add(new()
                {
                    Id = GetIntValue(reader["id"]).Value,
                    FirstName = GetStringValue(reader["firstname"]),
                    LastName = GetStringValue(reader["lastname"]),
                    PhoneNumber = GetStringValue(reader["num"]),
                    CreatedTime = GetDateTimeValue(reader["created_time"]).Value,
                    UpdateTime = GetDateTimeValue(reader["update_time"]).Value
                });
            }

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(SearchContactAsync)}: {ex.Message}");
            return [];
        }
    }

}
