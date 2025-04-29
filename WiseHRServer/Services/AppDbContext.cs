using Microsoft.EntityFrameworkCore;
using WiseHRServer.Models;

namespace WiseHRServer.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BankingInformation> BankingInformation { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }


        //Employee Details Operations

        public async Task<bool> RegisterEmployeeDetailsAsync(EmployeeDetails request)
        {
            Console.WriteLine("Inserting Info");

            await EmployeeDetails.AddAsync(request);
            await SaveChangesAsync();

            Console.WriteLine("Inserted");
            return true;
        }
        public async Task<List<EmployeeDetails>> GetAllEmployeesAsync()
        {
            return await EmployeeDetails.ToListAsync();  // Fetch all employee details from the database
        }

        public async Task<EmployeeDetails> GetEmployeeDetailsAsync(string employeeID)
        {
            Console.WriteLine("Getting Info");

            EmployeeDetails employeeDetails = await EmployeeDetails.FirstOrDefaultAsync(e => e.EmployeeID == employeeID);

            Console.WriteLine("Got Info");
            return employeeDetails;
        }

        public async Task<bool> DeleteEmployeeDetailsAsync(string employeeId)
        {
            var employeeDetails = await EmployeeDetails.FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employeeDetails != null)
            {
                EmployeeDetails.Remove(employeeDetails);
                await SaveChangesAsync();
                Console.WriteLine("Deleted EmployeeDetails for EmployeeID: " + employeeId);
                return true;
            }
            else
            {
                Console.WriteLine("No EmployeeDetails found for EmployeeID: " + employeeId);
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeDetailsAsync(EmployeeDetails request)
        {
            Console.WriteLine("Updating Info");

            // Find existing record
            var existingEmployee = await EmployeeDetails.FirstOrDefaultAsync(e => e.EmployeeID == request.EmployeeID);

            if (existingEmployee == null)
            {
                Console.WriteLine("Employee not found.");
                return false;
            }

            // Preserve properties
            request.Id = existingEmployee.Id;
            request.EmployeeID = existingEmployee.EmployeeID;
            request.CreatedAt = existingEmployee.CreatedAt;


            var properties = typeof(EmployeeDetails).GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == "Id" || prop.Name == "EmployeeID" || prop.Name == "CreatedAt")
                    continue;

                var newValue = prop.GetValue(request);
                prop.SetValue(existingEmployee, newValue);
            }

            var changes = await SaveChangesAsync();

            if (changes > 0)
            {
                Console.WriteLine("Updated successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("No changes made.");
                return false;
            }
        }

        // Get employee by email
        public async Task<EmployeeDetails?> GetEmployeeDetailsByEmailAsync(string email)
        {
            Console.WriteLine($"Fetching EmployeeDetails for Email: {email}");
            return await EmployeeDetails.FirstOrDefaultAsync(e => e.CurrentEmail == email);
        }




        public async Task<bool> RegisterBankingProfileAsync(BankingInformation bankingInformation)
        {
            Console.WriteLine("Inserting BankingInfo");
            await BankingInformation.AddAsync(bankingInformation);
            await SaveChangesAsync();
            Console.WriteLine("Inserted BankingInfo");
            return true;
        }
        public async Task<bool> UpdateBankingInformationAsync(BankingInformation request)
        {
            Console.WriteLine("Updating BankingInfo");

            // Find existing record
            var existingBankingInfo = await BankingInformation
                                                       .FirstOrDefaultAsync(b => b.EmployeeID == request.EmployeeID);

            if (existingBankingInfo == null)
            {
                Console.WriteLine("Employee not found.");
                return false;
            }

            // Preserve properties
            request.Id = existingBankingInfo.Id;
            request.EmployeeID = existingBankingInfo.EmployeeID;
            request.CreatedAt = existingBankingInfo.CreatedAt;

            // Get all properties
            var properties = typeof(BankingInformation).GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name == "Id" || prop.Name == "EmployeeID" || prop.Name == "CreatedAt")
                    continue;

                var newValue = prop.GetValue(request);
                prop.SetValue(existingBankingInfo, newValue);
            }

            // Save changes to database
            var changes = await SaveChangesAsync();

            if (changes > 0)
            {
                Console.WriteLine("Updated successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("No changes made.");
                return false;
            }
        }
        public async Task<List<BankingInformation>> GetAllBankinginfoAsync()
        {
            return await BankingInformation.ToListAsync();  // Fetch all employee details from the database
        }

        public async Task<BankingInformation> GetBankingInformationAsync(string employeeId)
        {
            Console.WriteLine("Getting BankingInfo");
            return await BankingInformation.FirstOrDefaultAsync(b => b.EmployeeID == employeeId);
        }

        public async Task<bool> DeleteBankingInformationAsync(string employeeId)
        {
            Console.WriteLine("Deleting BankingInfo");

            var bankingInfo = await BankingInformation
                                      .FirstOrDefaultAsync(b => b.EmployeeID == employeeId);

            if (bankingInfo == null)
            {
                return false; // No record found
            }

            BankingInformation.Remove(bankingInfo);
            await SaveChangesAsync();

            return true; //Record Found
        }  
        //Experience Operations

        public async Task<bool> RegisterExperienceAsync(Experience request)
        {
            Console.WriteLine("Inserting Info");

            await Experience.AddAsync(request);
            await SaveChangesAsync();

            Console.WriteLine("Inserted");
            return true;
        }

        public async Task<List<Experience>> GetAllExperienceAsync()
        {
            return await Experience.ToListAsync();  // Fetch all experience from the database
        }

        public async Task<Experience> GetExperienceAsync(string employeeID)
        {
            Console.WriteLine("Getting Info");

            Experience experience = await Experience.FirstOrDefaultAsync(e => e.EmployeeID == employeeID);

            Console.WriteLine("Got Info");
            return experience;
        }

        public async Task<bool> DeleteExperienceAsync(string employeeId)
        {
            var experience = await Experience.FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (experience != null)
            {
                Experience.Remove(experience);
                await SaveChangesAsync();
                Console.WriteLine("Deleted Experience for EmployeeID: " + employeeId);
                return true;
            }
            else
            {
                Console.WriteLine("No experience found for EmployeeID: " + employeeId);
                return false;
            }
        }

        public async Task<bool> UpdateExperienceAsync(Experience request)
        {
            Console.WriteLine("Updating Info");

            // Find existing record
            var existingExperience = await Experience.FirstOrDefaultAsync(e => e.EmployeeID == request.EmployeeID);

            if (existingExperience == null)
            {
                Console.WriteLine("Employee not found.");
                return false;
            }

            // Preserve fields
            request.Id = existingExperience.Id;
            request.EmployeeID = existingExperience.EmployeeID;


            var properties = typeof(Experience).GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == "Id" || prop.Name == "EmployeeID" || prop.Name == "CreatedAt")
                    continue;

                var newValue = prop.GetValue(request);
                prop.SetValue(existingExperience, newValue);
            }

            // Save changes to database
            var changes = await SaveChangesAsync();

            if (changes > 0)
            {
                Console.WriteLine("Updated successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("No changes made.");
                return false;
            }
        }

    }
}