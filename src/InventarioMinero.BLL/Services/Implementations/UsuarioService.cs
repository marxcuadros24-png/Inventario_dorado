using BCrypt.Net;
using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class UsuarioService : IUsuarioService
{
    private readonly InventarioDbContext _db;

    public UsuarioService(InventarioDbContext db) => _db = db;

    public async Task<(bool Exito, Usuario? Usuario, string Mensaje)> AutenticarAsync(
        string nombreUsuario, string password)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Empleado).ThenInclude(e => e.Cargo)
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario is null)
            return (false, null, "Usuario no encontrado o inactivo.");

        if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            return (false, null, "Contraseña incorrecta.");

        await RegistrarAccesoAsync(usuario.UsuarioId);
        return (true, usuario, "Autenticación exitosa.");
    }

    public async Task<IEnumerable<Usuario>> GetTodosAsync()
        => await _db.Usuarios
            .Include(u => u.Empleado).ThenInclude(e => e.Cargo)
            .Include(u => u.Rol)
            .AsNoTracking().ToListAsync();

    public async Task<Usuario?> GetPorIdAsync(int id)
        => await _db.Usuarios
            .Include(u => u.Empleado).ThenInclude(e => e.Cargo)
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.UsuarioId == id);

    public async Task<(bool Exito, string Mensaje)> CrearAsync(Usuario usuario, string passwordPlano)
    {
        if (await _db.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
            return (false, "El nombre de usuario ya existe.");

        if (await _db.Usuarios.AnyAsync(u => u.EmpleadoId == usuario.EmpleadoId))
            return (false, "Este empleado ya tiene una cuenta de usuario.");

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlano, workFactor: 12);
        await _db.Usuarios.AddAsync(usuario);
        await _db.SaveChangesAsync();
        return (true, "Usuario creado correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarAsync(Usuario usuario)
    {
        var existe = await _db.Usuarios
            .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario && u.UsuarioId != usuario.UsuarioId);
        if (existe) return (false, "El nombre de usuario ya está en uso.");

        _db.Usuarios.Update(usuario);
        await _db.SaveChangesAsync();
        return (true, "Usuario actualizado.");
    }

    public async Task<(bool Exito, string Mensaje)> CambiarPasswordAsync(
        int usuarioId, string passwordActual, string passwordNuevo)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return (false, "Usuario no encontrado.");
        if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.PasswordHash))
            return (false, "La contraseña actual es incorrecta.");

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordNuevo, workFactor: 12);
        await _db.SaveChangesAsync();
        return (true, "Contraseña actualizada correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int usuarioId)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return (false, "Usuario no encontrado.");
        usuario.Activo = !usuario.Activo;
        await _db.SaveChangesAsync();
        return (true, usuario.Activo ? "Usuario activado." : "Usuario desactivado.");
    }

    public async Task RegistrarAccesoAsync(int usuarioId)
    {
        var u = await _db.Usuarios.FindAsync(usuarioId);
        if (u is not null)
        {
            u.UltimoAcceso = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }
}
