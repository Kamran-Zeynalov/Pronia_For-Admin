

using System.ComponentModel.DataAnnotations;

namespace P230_Pronia.Entities
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Необходимо ввести имя в поле.")]
        public string Name { get; set; }
        public List<PlantCategory> PlantCategories { get; set; }
    }
}
