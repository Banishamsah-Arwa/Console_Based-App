
using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;


namespace InventoryManagementSystem
{
    class Program
    {
        private static CategoryService categoryService=new CategoryService("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=123456;");
        private static UserService userService =new UserService("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=123456;");
        private static ItemService itemService =new ItemService("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=123456;");
        private static PurchaseService purchaseService=new PurchaseService("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=123456;");
        private static string currentuser;
        private static string currentpassword;
        private static string role;

        static async Task Main(string[] args)
        {


            Welcoming();
            string signInsignUp = Console.ReadLine();
            switch (signInsignUp)
            {
                case ("1"):
                    {
                        GetUsernameAndPassword(out currentuser, out currentpassword);


                        (bool isAuthenticated, bool is_admin) = await userService.LogIn(currentuser, currentpassword);
                        if (isAuthenticated == true && is_admin == true)
                        {
                            role= "admin";
                            AdminRole();
                        }
                        else if (isAuthenticated == true && is_admin == false)
                        {
                            role ="user";

                            RegularUser();
                        }
                        else
                        {
                            Console.WriteLine("You are not a user, try create new account");
                            Welcoming();
                        }
                        break;
                    }

                case ("2"):
                    {
                        GetUsernameAndPassword(out currentuser, out currentpassword);

                        do
                        {
                            Console.WriteLine("Enter your Role (Admin|User):");
                            role = Console.ReadLine().Trim().ToLower();
                            if (role != "admin" && role != "user")
                            {
                                Console.WriteLine("Invalid role. Please enter 'Admin' or 'User'.");
                            }
                        } while (role != "admin" && role != "user");
                        User user = new User { UserName = currentuser, Password = currentpassword, Role = role };
                        await userService.CreateNewAccount(user);
                        if (role=="admin")
                        {
                           await  AdminRole();
                        }
                        else
                        {
                           await  RegularUser();

                        }
                        break;
                    }
                case ("3"):
                    {
                        System.Environment.Exit(0);
                        break;
                    }

            }



        }

        private static void GetUsernameAndPassword(out string username, out string password)
        {
            do
            {
                Console.WriteLine("Enter your Username:");
                username = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Username cannot be empty.");
                }
            } while (string.IsNullOrEmpty(username));

            do
            {
                Console.WriteLine("Enter Your Password:");
                password = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Password cannot be empty.");
                }
            } while (string.IsNullOrEmpty(password));
        }


        private static void Welcoming()
        {
            Console.WriteLine("Welcome to the Inventory Management System!");

            Console.WriteLine("1.Log In.");
            Console.WriteLine("2.Sign Up.");
            Console.WriteLine("3.Exit");
        }

        private static async Task AdminRole()
        {
            while (true)
            {
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Manage Categories");
                Console.WriteLine("2. Manage Items");
                Console.WriteLine("3. View Purchase History");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ManageCategories();
                        break;
                    case "2":
                        await ManageItems();
                        break;
                    case "3":
                        await ViewPurchaseHistory();
                        break;
                    case "4":
                        Main(new string[0]);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 1 to 4.");
                        break;
                }
            }
        }

        private static async Task ViewPurchaseHistory()
        {
            purchaseService.DisplayAllPurchases();
        }

        private static async Task RegularUser()
        {
            while (true)
            {
                Console.WriteLine("Regular User Menu:");
                Console.WriteLine("1. Make a Purchase");
                Console.WriteLine("2. View Items");
                Console.WriteLine("3. View Categories");
                Console.WriteLine("4. View Purchase History");
                Console.WriteLine("5. Search for certain item");
                Console.WriteLine("6. Search for certain category items ");

                Console.WriteLine("7. Exit");
                Console.Write("Enter your choice: ");

                string choice;
                 choice= Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await MakePurchase();
                        break;
                    case "2":
                        await DisplayItems();
                        break;
                    case "3":
                        await DisplayCategories();
                        break;
                    case "4":
                        await ViewPurchaseHistoryForUser();
                        break;
                    case "5":
                        await SearchItemAsync();
                        break;
                    case "6":
                        SearchCategory();
                        break;
                    case "7":
                        currentuser = null;
                        currentpassword = null;
                        Welcoming();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 1 to 5.");
                        break;
                }
            }
        }

        private static async void SearchCategory()
        {
            string categoryname;
            do
            {
                Console.WriteLine("Enter your item name you want to search for :");
                categoryname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(categoryname))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryname));
            var items = await itemService.GetItemByCategoryName(categoryname);
            Console.WriteLine("Name\tDescription\tCategory name\tQuantity\tPrice ");
            foreach (var item in items)
            {
                Console.WriteLine($"{item.Name}\t{item.Description}\t{item.Category_name}\t{item.Quantity}\t{item.Price}");
            }
            return;
        }

        private static async Task SearchItemAsync()
        {
            string itemname;
            do
            {
                Console.WriteLine("Enter your item name you want to search for :");
                itemname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemname))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(itemname));

            var item = await itemService.GetItemByName(itemname);
            if (item != null)
            {
                Console.WriteLine("Name\tDescription\tCategory name\tQuantity\tPrice ");
                Console.WriteLine($"{item.Name}\t{item.Description}\t{item.Category_name}\t{item.Quantity}\t{item.Price}");
            }
            else
            {
                Console.WriteLine("Item not found.");
            }

            return;

        }

        private static async Task ViewPurchaseHistoryForUser()
        {
            purchaseService.DisplayPurchaseHistory(currentuser);

        }

        private static async Task MakePurchase()
        {
            string itemname;
            do
            {
                Console.WriteLine("Enter your item name:");
                itemname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemname))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(itemname));

            int quantity;
            do
            {
                Console.WriteLine("Enter your item Quantity:");

                string quantityString = Console.ReadLine();
                if (!int.TryParse(quantityString, out quantity) || quantity < 0)
                {
                    Console.WriteLine("Invalid quantity. Please enter a non-negative number.");
                }
            } while (quantity < 0);

              purchaseService.AddItemToPurchase(itemname, currentuser, quantity);
            return;

        }

        private static async Task ManageCategories()
        {
            while (true)
            {
                Console.WriteLine("Manage Categories:");
                Console.WriteLine("1. Add Category");
                Console.WriteLine("2. Display All Categories");
                Console.WriteLine("3. Update Category");
                Console.WriteLine("4. Delete Category");
                Console.WriteLine("5. Back to Main Menu");
                Console.WriteLine("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddCategory();
                        break;
                    case "2":
                        await DisplayCategories();
                        break;
                    case "3":
                        await UpdateCategory();
                        break;
                    case "4":
                        await DeleteCategory();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 1 to 5.");
                        break;
                }
            }
        }

        private static async Task DeleteCategory()
        {
            string categoryname;
            do
            {
                Console.WriteLine("Enter your category name tobe deleted:");
                categoryname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(categoryname))
                {
                    Console.WriteLine("category name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryname));

           await  categoryService.DeleteCategory(categoryname);
           await ManageCategories();


        }

        private static async Task UpdateCategory()
        {
            string categoryOldName;
            string categoryname;
            string description;
            do
            {
                Console.WriteLine("Enter your category old name:");
                categoryOldName = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(categoryOldName))
                {
                    Console.WriteLine("category name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryOldName));

            do
            {
                Console.WriteLine("Enter your category new name:");
                categoryname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(categoryname))
                {
                    Console.WriteLine("category name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryname));

            Console.WriteLine("Enter your category new description:");
            description = Console.ReadLine();
            Category category = new Category { Name = categoryname, Description = description };
           await  categoryService.UpdateCategory(categoryOldName,category);
          await ManageCategories()  ;
        }

        private static async Task DisplayCategories()
        {
            await categoryService.GetAllCategories();
            await ManageCategories();
        }

        private static async Task AddCategory()
        {
            string categoryname;
            string description;

            do
            {
                Console.WriteLine("Enter your category name:");
                 categoryname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(categoryname))
                {
                    Console.WriteLine("category name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryname));
            
                Console.WriteLine("Enter your category description:");
                description = Console.ReadLine();
            Category category = new Category {Name = categoryname, Description = description };
           await categoryService.AddCategory(category);
            Console.WriteLine("Added sucessfully");

            await ManageCategories();
        }

        private static async Task ManageItems()
        {
            while (true)
            {
                Console.WriteLine("Manage Items:");
                Console.WriteLine("1. Add Item");
                Console.WriteLine("2. Update Item");
                Console.WriteLine("3. Delete Item");
                Console.WriteLine("4. Display All Items");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddItem();
                        break;
                    case "2":
                        await UpdateItem();
                        break;
                    case "3":
                        await DeleteItem();
                        break;
                    case "4":
                        await DisplayItems();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 1 to 5.");
                        break;
                }
            }
        }

        private static async Task DisplayItems()
        {
            var items = await itemService.GetAllItems();
            Console.WriteLine("Name\tDescription\tCategory name\tQuantity\tPrice ");
            foreach (var item in items)
            {
                Console.WriteLine($"{item.Name}\t{item.Description}\t{item.Category_name}\t{item.Quantity}\t{item.Price}");
            }
            if (role == "admin")
                await ManageItems();
            else
                await RegularUser();

        }

        private static async Task DeleteItem()
        {
            string itemName;
            do
            {
                Console.WriteLine("Enter your item name:");
                itemName = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemName))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(itemName));
           await  itemService.DeleteItem(itemName);
            await ManageItems();

        }

        private static async Task UpdateItem()
        {
            string itemName;
            do
            {
                Console.WriteLine("Enter your item old name:");
                itemName = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemName))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(itemName));
            string itemname;
            do
            {
                Console.WriteLine("Enter your item name:");
                itemname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemname))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(itemname));

            string description;
            Console.WriteLine("Enter your item Description:");
            description = Console.ReadLine();

            string categoryName;
            do
            {
                Console.WriteLine("Enter your item category name:");
                categoryName = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemname))
                {
                    Console.WriteLine("category name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryName));

            int price;
            do
            {
                Console.WriteLine("Enter your item Price:");

                string priceString = Console.ReadLine();
                if (!int.TryParse(priceString, out price) || price < 0)
                {
                    Console.WriteLine("Invalid price. Please enter a non-negative number.");
                }
            } while (price < 0);

            int quantity;
            do
            {
                Console.WriteLine("Enter your item Quantity:");

                string quantityString = Console.ReadLine();
                if (!int.TryParse(quantityString, out quantity) || quantity < 0)
                {
                    Console.WriteLine("Invalid quantity. Please enter a non-negative number.");
                }
            } while (quantity < 0);

            Item item = new Item
            {
                Name = itemname,
                Description = description,
                Price = price,
                Quantity = quantity,
                Category_name = categoryName,
            };


            await itemService.UpdateItem(itemName, item);
            await ManageItems();


        }

        private static async Task AddItem()
        {
            string itemname;
            do
            {
                Console.WriteLine("Enter your item name:");
                itemname = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemname))
                {
                    Console.WriteLine("item name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(itemname));

            string description;
            Console.WriteLine("Enter your item Description:");
            description = Console.ReadLine();

            string categoryName;
            do
            {
                Console.WriteLine("Enter your item category name:");
                categoryName = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(itemname))
                {
                    Console.WriteLine("category name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(categoryName));

            int price;
            do
            {
                Console.WriteLine("Enter your item Price:");

                string priceString = Console.ReadLine();
                if (!int.TryParse(priceString, out price) || price < 0)
                {
                    Console.WriteLine("Invalid price. Please enter a non-negative number.");
                }
            } while (price < 0);

            int quantity;
            do
            {
                Console.WriteLine("Enter your item Quantity:");

                string quantityString = Console.ReadLine();
                if (!int.TryParse(quantityString, out quantity) || quantity < 0)
                {
                    Console.WriteLine("Invalid quantity. Please enter a non-negative number.");
                }
            } while (quantity < 0);
            Item item = new Item {
            Name = itemname,
            Description = description,
            Price = price,
            Quantity = quantity,
            Category_name=categoryName,
            };
            await itemService.AddItem( item);
            await ManageItems();

        }
        

    }

}
