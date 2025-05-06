using ChainVote.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ChainVote.SeedData
{
    public static class UserDataSeeder
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            if (userManager.Users.Any()) return;

            var courses = new[] { "BSCS", "BSIT", "BSCpE", "BSDS", "BSEMC" }; // CICT courses
            var sections = new[] { "A", "B", "C", "D" };
            var admissionYears = new Dictionary<string, string>
            {
                { "22", "4" }, // 2022 => 4th year
                { "23", "3" }, // 2023 => 3rd year
                { "24", "2" }, // 2024 => 2nd year
                { "25", "1" }  // 2025 => 1st year
            };

            var firstNames = new[] { "Juan", "Maria", "Pedro", "Ana", "Jose", "Carla", "Luis", "Isabel", "Ramon", "Grace" };
            var lastNames = new[] { "Santos", "Dela Cruz", "Garcia", "Reyes", "Bautista", "Mendoza", "Lopez", "Torres", "Aquino", "Gonzales" };

            var random = new Random();

            for (int i = 0; i < 30; i++)
            {
                // Pick a random year code (2022-2025)
                var yearCode = admissionYears.Keys.ElementAt(random.Next(admissionYears.Count));
                var yearLevel = admissionYears[yearCode];

                // Generate a student ID in the format: yy-xxxx
                var numberPart = random.Next(1000, 9999).ToString();
                var studentId = $"{yearCode}-{numberPart}";

                // Pick random names
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var email = $"{firstName.ToLower()}{lastName.ToLower()}{yearCode}@bpsu.edu.ph";

                // Generate a user object
                var user = new ApplicationUser
                {
                    UserName = studentId,
                    StudentId = studentId,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Course = courses[random.Next(courses.Length)],
                    Section = sections[random.Next(sections.Length)],
                    YearLevel = yearLevel
                };

                // Create the user
                var result = await userManager.CreateAsync(user, "Test@123");
                if (result.Succeeded)
                {
                    // Assign the "Voter" role
                    await userManager.AddToRoleAsync(user, "Voter");
                }
            }
        }

    }
}
