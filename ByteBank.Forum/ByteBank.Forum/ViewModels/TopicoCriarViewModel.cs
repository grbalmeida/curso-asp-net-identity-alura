using System.ComponentModel.DataAnnotations;

namespace ByteBank.Forum.ViewModels
{
    public class TopicoCriarViewModel
    {
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Display(Name = "Conteúdo")]
        public string Conteudo { get; set; }
    }
}