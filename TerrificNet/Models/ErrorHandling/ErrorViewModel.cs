namespace TerrificNet.Models.ErrorHandling
{
    public class ErrorViewModel
    {
        public string ErrorMessage { get; set; }
        public string Details { get; set; }
        public string TemplateId { get; set; }
        public string Before { get; set; }
        public string Node { get; set; }
        public string After { get; set; }
        public string Text { get; set; }
        public ErrorRange Range { get; set; }
    }
}