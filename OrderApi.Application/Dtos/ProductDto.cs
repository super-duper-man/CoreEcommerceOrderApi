using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record ProductDto(
            int Id,
            [Required] string Name,
            [Required, Range(1, int.MaxValue)] string ProductQuantity,
            [Required, DataType(DataType.Currency)] decimal Price
        );
}
