using Npgsql;

namespace Infrastructure.Data;

public static class Database
{
    private static readonly string ConnectionString = "Host = localhost; Port = 5432; Username = postgres; Database = Exam_Dapper; Password = 1234554321";

    public static NpgsqlConnection Connection() => new(ConnectionString);

}
