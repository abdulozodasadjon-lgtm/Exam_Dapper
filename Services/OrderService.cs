using System.Net.Security;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class OrderService
{
    public async Task<bool> AddOrder (Orders order)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "insert into orders (customer_id, amount, created_at) values (@customer_id, @amount, @created_at)";
        var res = await conn.ExecuteAsync(query, new { customer_id = order.Customer_Id, amount = order.Amount, created_at = order.Created_At });
        return res > 0;
    }

    public async Task<bool> UpdateOrder (Orders order)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "update orders set customer_id = @customer_id, amount = @amount, created_at = @created_at where id = @id";
        var res = await conn.ExecuteAsync(query, new { customer_id = order.Customer_Id, amount = order.Amount, created_at = order.Created_At, id = order.Id });
        return res > 0;
    }

    public async Task<bool> DeleteOrder (int id)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "delete from orders where id = @id";
        var res = await conn.ExecuteAsync(query, new { id });
        return res > 0;
    }

    public async Task<Orders> GetOrder (int id)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "select id, customer_id, amount, created_at from orders where id = @id";
        var res = await conn.QueryFirstAsync<Orders>(query, new { id });
        return res;
    }

    public async Task<List<OrderSummary>> GetAllOrders (int customerId)
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = """
            select o.id as "OrderId", c.fullname as "CustomerName",
                   o.amount as "Amount", o.created_at as "CreatedAt"
            from orders o
            inner join customers c on c.id = o.customer_id
            where c.id = @CustomerId
            """;
        var res = await conn.QueryAsync<OrderSummary>(query, new { CustomerId = customerId });
        return res.ToList();
    }

    public async Task <List<OrderSummary>> GetOrdersInSpecialFormat()
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = """
            select o.id as "OrderId", c.fullname as "CustomerName",
                   o.amount as "Amount", o.created_at as "CreatedAt"
            from orders o
            inner join customers c on c.id = o.customer_id
            """;
        var res = await conn.QueryAsync<OrderSummary>(query);
        return res.ToList();
    }

    public async Task<decimal> GetSumOfOrders()
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = "select coalesce(sum(amount), 0) from orders";
        var res = await conn.QueryFirstAsync<decimal>(query);
        return res;
    }

    public async Task<List<CountOFOrderswithCustomer>> GetOrdersWithCountOfOrders()
    {
        using var conn = Database.Connection();
        conn.Open();
        var query = """
            select c.fullname as "Fullname", count(o.id) as "OrderCount"
            from customers c
            left join orders o on c.id = o.customer_id
            group by c.id, c.fullname
            order by c.id
            """;
        var res = await conn.QueryAsync<CountOFOrderswithCustomer>(query);
        return res.ToList();
    }

    public async Task<List<CustomerOrderTotal>> GetCustomersByOrderAmount()
    {
        using var con = Database.Connection();
        con.Open();
        var query = """
            select c.fullname as "CustomerName", sum(o.amount) as "TotalAmount"
            from customers c
            inner join orders o on c.id = o.customer_id
            group by c.id, c.fullname
            order by sum(o.amount) desc
            limit 3
            """;
        var customers = await con.QueryAsync<CustomerOrderTotal>(query);
        return customers.ToList();
    }
}
