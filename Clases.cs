
public class Comisaria
{
    public string Id { get; set; }
    public string Tipo { get; set; }
    public string Fono { get; set; }
    public string Nombre { get; set; }
    //public string Comuna { get; set; }
    //public string fonoCua { get; set; }
    //public string Hacia { get; set; }
    public string corX { get; set; }
    public string corY { get; set; }
    public string Direccion { get; set; }
}

public class Cuadrante
{
    public string Id { get; set; }
    public string Tipo { get; set;}
    public string Fono { get; set; }
    public string Comuna { get; set; }
}

public class Ubicacion
{
    public string Calle { get; set; }
    public string Numero { get; set; }
    public string Comuna { get; set; }
}

public class AceptarTerminos
{
    public string valTerminos  { get; set; }
}