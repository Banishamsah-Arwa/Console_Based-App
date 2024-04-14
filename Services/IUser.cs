using InventoryManagementSystem.Model;


namespace InventoryManagementSystem.Services
{
    public interface IUser
    {
       Task  CreateNewAccount(User user);
       Task <(bool,bool)> LogIn(string name , string password );

    }
}
