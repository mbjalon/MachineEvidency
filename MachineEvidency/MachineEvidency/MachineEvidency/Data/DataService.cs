using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MachineEvidency.AuthService;
using SQLite;

namespace MachineEvidency.Data;

public class DataService
{
    private readonly SQLiteAsyncConnection _connection;

    public DataService(string dbFilePath)
    {
        _connection = new SQLiteAsyncConnection(dbFilePath);
        _connection.CreateTableAsync<BigMachine>().Wait();
        _connection.CreateTableAsync<SmallMachine>().Wait();
        _connection.CreateTableAsync<User>().Wait();
    }

    public async Task<List<Machine>?> GetAllMachinesAsync(int type, int status)
    {
        try
        {
            if (type == 0) 
            {
                var smallMachines = await _connection.Table<SmallMachine>().Where(m => m.Status == status).ToListAsync();
                return smallMachines.Cast<Machine>().ToList();
            }

            var bigMachines = await _connection.Table<BigMachine>().Where(m => m.Status == status).ToListAsync();
            return bigMachines.Cast<Machine>().ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving data from database: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AddMachineAsync(Machine machine, int type)
    {
        try
        {
            if (type == 0)
                await _connection.InsertAsync(machine as SmallMachine);
            else
                await _connection.InsertAsync(machine as BigMachine);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding machine to database: {e.Message}");
            return false;
        }
    }
    
    public async Task UpdateMachineAsync(Machine machine, string id, int type)
    {
        try
        {
            if (type == 0)
            {
                await _connection.ExecuteAsync("DELETE FROM SmallMachine WHERE ID = ?", id);
                await _connection.InsertAsync(machine);
            }
            else
            {
                await _connection.ExecuteAsync("DELETE FROM BigMachine WHERE ID = ?", id);
                await _connection.InsertAsync(machine);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating machine in database: {ex.Message}");
        }
    }

    public async Task DeleteAsync(int type, string id)
    {
        try
        {
            if (type == 0)
                await _connection.ExecuteAsync("DELETE FROM SmallMachine WHERE ID = ?", id);
            else 
                await _connection.ExecuteAsync("DELETE FROM BigMachine WHERE ID = ?", id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing machine from database: {ex.Message}");
        }
    }

    public async Task StatusChangeAsync(int type, string id, int status)
    {
        try
        {
            var machine = await GetMachineByIdAsync(type, id);
            if (machine != null) 
                machine.Status = status;
            else
                return;

            if (type == 0)
                await UpdateMachineAsync(machine, id, type);
            else
                await UpdateMachineAsync(machine, id, type);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing machine from database: {ex.Message}");
        }
    }
    
    public async Task AddUserAsync(User user)
    {
        try
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _connection.InsertAsync(user);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding user to database: {e.Message}");
        }
    }
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        try
        {
            return await _connection.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user from database: {ex.Message}");
            return null;
        }
    }
    
    private async Task<Machine?> GetMachineByIdAsync(int type, string id)
    {
        try
        {
            if (type == 0)
                return await _connection.Table<SmallMachine>().FirstOrDefaultAsync(m => m.ID == id);

            return await _connection.Table<BigMachine>().FirstOrDefaultAsync(m => m.ID == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving machine from database: {ex.Message}");
            return null;
        }
    }
}