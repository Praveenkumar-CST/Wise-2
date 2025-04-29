using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WiseHRServer.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } // Matches Supabase userId

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } // "user", "admin"

        public OnboardingData Onboarding { get; set; }
    }

    public class OnboardingData
    {
        // Basic Information
        [Required]
        public string EmployeeName { get; set; } // As per Govt. ID *
        [Required]
        public string EmployeeNo { get; set; } // *
        [Required]
        public string FirstName { get; set; } // *
        public string MiddleName { get; set; } // Optional
        [Required]
        public string LastName { get; set; } // *
        [Required]
        public string FathersName { get; set; } // *
        [Required]
        public string MothersName { get; set; } // *
        [Required]
        public string DateOfJoining { get; set; } // DD-MM-YYYY *
        public string DateOfRelieving { get; set; } // Filled by People Team, Optional
        [Required]
        public string DateOfBirth { get; set; } // DD-MM-YYYY *
        public string TypeOfEmployment { get; set; } // Optional (e.g., Full-Time, Part-Time)
        public string Level { get; set; } // Optional (e.g., L1, L2)
        public string Designation { get; set; } // Optional (e.g., Software Engineer)
        [Required]
        public string JoiningLocation { get; set; } // *
        [Required]
        public string Gender { get; set; } // *
        [Required]
        public string MaritalStatus { get; set; } // *
        [Required]
        public string BloodGroup { get; set; } // *
        [Required]
        public string Nationality { get; set; } // *
        public string Allergies { get; set; } // Optional
        public string Medications { get; set; } // Optional
        [Required]
        public bool IsPhysicallyChallenged { get; set; } // *

        // Family Details (if married)
        public int NumberOfSons { get; set; } // Optional
        public int NumberOfDaughters { get; set; } // Optional

        // Address Details
        [Required]
        public Address CurrentAddress { get; set; } // *
        [Required]
        public Address PermanentAddress { get; set; } // *
        [Required]
        public string MobileNo { get; set; } // Encrypted *
        [Required]
        public string PersonalEmailId { get; set; } // *

        // Passport Details
        public PassportDetails Passport { get; set; } // Optional (not all employees have passports)

        // Bank Details
        [Required]
        public BankDetails Bank { get; set; } // *

        // Emergency Contacts
        [Required]
        public EmergencyContact EmergencyContact1 { get; set; } // *
        [Required]
        public EmergencyContact EmergencyContact2 { get; set; } // *

        // Nominee Details
        [Required]
        public NomineeDetails Nominee { get; set; } // *

        // Educational Qualifications
        [Required]
        public List<Education> Education { get; set; } // *

        // Work Experience
        public List<WorkExperience> WorkExperience { get; set; } // Optional

        // Metadata
        public string? Status { get; set; } // "draft", "submitted" (set in controller, not required)
        public bool AgreementAccepted { get; set; } // Default false
        public DateTime LastUpdated { get; set; } // Set in controller
    }

    public class Address
    {
        [Required]
        public string Line1 { get; set; } // *
        public string Line2 { get; set; } // Optional
        public string Line3 { get; set; } // Optional
        [Required]
        public string City { get; set; } // *
        [Required]
        public string State { get; set; } // *
        [Required]
        public string ZipCode { get; set; } // *
    }

    public class PassportDetails
    {
        [Required]
        public string FullName { get; set; } // *
        [Required]
        public string PassportNo { get; set; } // Encrypted *
        [Required]
        public string Nationality { get; set; } // *
        [Required]
        public string DateOfIssue { get; set; } // *
        [Required]
        public string DateOfExpiry { get; set; } // *
        [Required]
        public string PlaceOfIssue { get; set; } // *
    }

    public class BankDetails
    {
        [Required]
        public string BankName { get; set; } // *
        [Required]
        public string AccountNumber { get; set; } // Encrypted *
        [Required]
        public string IFSC { get; set; } // Encrypted *
        [Required]
        public string BranchDetailsState { get; set; } // *
        [Required]
        public string PAN { get; set; } // Encrypted *
        [Required]
        public string AadharNo { get; set; } // Encrypted *
    }

    public class EmergencyContact
    {
        [Required]
        public string Name { get; set; } // *
        [Required]
        public string Relationship { get; set; } // *
        [Required]
        public Address Address { get; set; } // *
        [Required]
        public string MobileNo { get; set; } // Encrypted *
    }

    public class NomineeDetails
    {
        [Required]
        public string Name { get; set; } // *
        [Required]
        public string Relationship { get; set; } // *
        [Required]
        public Address Address { get; set; } // *
        [Required]
        public string MobileNo { get; set; } // Encrypted *
    }


}
