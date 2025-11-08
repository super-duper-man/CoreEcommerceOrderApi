using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record AppUserDto(
            int Id,
            [Required] string Name,
            [Required] string Phone,
            [Required] string Address,
            [Required, EmailAddress] string Email,
            [Required] string password,
            [Required] string role
        );
}
