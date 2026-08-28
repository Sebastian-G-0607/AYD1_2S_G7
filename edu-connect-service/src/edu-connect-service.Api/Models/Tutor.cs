namespace edu_connect_service.Api.Models;

public class Tutor
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public required string Nombre { get; set; }

    public required string Apellido { get; set; }

    public required string CarnetId { get; set; }

    public required string NumeroIdentificacion { get; set; }

    public required string Genero { get; set; }

    public required string Direccion { get; set; }

    public required string Telefono { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public required string FotografiaUrl { get; set; }

    public required string DireccionTutoria { get; set; }

    public int AnioInicio { get; set; }

    public required string Universidad { get; set; }

    public TimeOnly? HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public ICollection<TutorDiaAtencion> DiasAtencion { get; set; } = [];

    public ICollection<TutorMateria> TutorMaterias { get; set; } = [];

    public ICollection<Sesion> Sesiones { get; set; } = [];
}
