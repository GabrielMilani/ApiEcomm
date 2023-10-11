using ApiEcomm.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiEcomm.ViewModels.Products
{
    public class EditProductViewModel
    {
        [Required(ErrorMessage = "O titulo e obrigatório")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 40 caracteres")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "O preço e obrigatório")]
        public decimal Price { get; set; }
        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "A categoria e obrigatório")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
