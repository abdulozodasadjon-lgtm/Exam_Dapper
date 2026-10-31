using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class CustomerService
{
    public async Task<bool> AddCustomer (Customer customer)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "insert into customers (fullname) values (@fullname)";
        var res = await conn.ExecuteAsync(query, new { fullname = customer.FullName });
        return res > 0;
    }

    public async Task<bool> UpdateCustomer (Customer customer)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "update customers set fullname = @fullname where id = @id";
        var res = await conn.ExecuteAsync(query, new { fullname = customer.FullName, id = customer.Id });
        return res > 0;
    }

    public async Task<bool> DeleteCustomer (int id)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "delete from customers where id = @id";
        var res = await conn.ExecuteAsync(query, new { id });
        return res > 0;
    }

    public async Task<Customer?> GetCustomer (int id)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "select id, fullname from customers where id = @id";
        var res = await conn.QueryFirstAsync<Customer>(query, new { id });
        return res;
    }

    public async Task<List<Customer>> GetAllCustomers ()
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "select id, fullname from customers";
        var res = await conn.QueryAsync<Customer>(query);
        return res.ToList();
    }
}
