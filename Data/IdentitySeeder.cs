using Microsoft.AspNetCore.Identity;
using MachineShopManager.Models;

namespace MachineShopManager.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        const string roleName = "Operator";

        // Cria a role Operator caso ela ainda não exista
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(
                    $"Não foi possível criar a role inicial de operador: {errors}");
            }
        }

        const string email = "operador@machineshop.com";
        const string password = "Operador123";

        var user = await userManager.FindByEmailAsync(email);

        // Cria o operador caso ainda não exista
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description)
                );

                throw new Exception(
                    $"Não foi possível criar o operador inicial: {errors}"
                );
            }
        }

        // Garante que o usuário pertence à role Operator
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            var assignmentResult = await userManager.AddToRoleAsync(user, roleName);

            if (!assignmentResult.Succeeded)
            {
                var errors = string.Join(", ", assignmentResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(
                    $"Não foi possível atribuir a role Operator ao usuário inicial: {errors}");
            }
        }
    }
}
