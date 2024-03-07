namespace Perritos.Entities
{
    public class Genero
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List <Perrito> Perritos { get; set; }
    } 
}
