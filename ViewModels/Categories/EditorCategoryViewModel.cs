using System.ComponentModel.DataAnnotations;

namespace ApiEcomm.ViewModels.Categories
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 40 caracteres")]
        public string Title { get; set; }
        [Required(ErrorMessage = "O Slug é obrigatório")]
        public string Slug { get; set; }
    }
}
