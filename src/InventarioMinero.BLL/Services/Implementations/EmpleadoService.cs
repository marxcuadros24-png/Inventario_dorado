using InventarioMinero.BLL.Services.Interfaces;
using InventarioMinero.DAL.Context;
using InventarioMinero.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioMinero.BLL.Services.Implementations;

public class EmpleadoService : IEmpleadoService
{
    private readonly InventarioDbContext _db;

    public EmpleadoService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Empleado>> GetTodosAsync(bool soloActivos = true)
    {
        var query = _db.Empleados
            .Include(e => e.Cargo)
            .AsNoTracking();

        if (soloActivos)
            query = query.Where(e => e.Activo);

        return await query.OrderBy(e => e.Apellidos).ThenBy(e => e.Nombres).ToListAsync();
    }

    public async Task<Empleado?> GetPorIdAsync(int id)
    {
        return await _db.Empleados
            .Include(e => e.Cargo)
            .Include(e => e.Asignaciones)
            .Include(e => e.EntregasEPP)
            .FirstOrDefaultAsync(e => e.EmpleadoId == id);
    }

    public async Task<Empleado?> GetPorDNIAsync(string dni)
    {
        return await _db.Empleados
            .Include(e => e.Cargo)
            .FirstOrDefaultAsync(e => e.DNI.Trim() == dni.Trim());
    }

    public async Task<(bool Exito, string Mensaje, Empleado? Empleado)> CrearAsync(Empleado empleado)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(empleado.Nombres) || string.IsNullOrWhiteSpace(empleado.Apellidos))
                return (false, "El nombre y apellidos son obligatorios.", null);

            if (string.IsNullOrWhiteSpace(empleado.DNI) || empleado.DNI.Trim().Length != 8)
                return (false, "El DNI debe tener 8 dígitos numéricos válidos.", null);

            var existente = await GetPorDNIAsync(empleado.DNI);
            if (existente != null)
                return (false, $"Ya existe un empleado registrado con el DNI {empleado.DNI} ({existente.NombreCompleto}).", null);

            if (string.IsNullOrWhiteSpace(empleado.Codigo))
                empleado.Codigo = await GenerarCodigoAsync();

            empleado.FechaCreacion = DateTime.Now;
            if (empleado.FechaIngreso == default)
                empleado.FechaIngreso = DateTime.Now;

            await _db.Empleados.AddAsync(empleado);
            await _db.SaveChangesAsync();

            return (true, $"Empleado '{empleado.NombreCompleto}' registrado exitosamente.", empleado);
        }
        catch (Exception ex)
        {
            return (false, $"Error al registrar empleado: {ex.Message}", null);
        }
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarAsync(Empleado empleado)
    {
        try
        {
            var existente = await _db.Empleados.FindAsync(empleado.EmpleadoId);
            if (existente == null)
                return (false, "El empleado no existe.");

            var porDni = await GetPorDNIAsync(empleado.DNI);
            if (porDni != null && porDni.EmpleadoId != empleado.EmpleadoId)
                return (false, $"El DNI {empleado.DNI} ya está asignado a otro trabajador ({porDni.NombreCompleto}).");

            existente.Nombres = empleado.Nombres.Trim();
            existente.Apellidos = empleado.Apellidos.Trim();
            existente.DNI = empleado.DNI.Trim();
            existente.CargoId = empleado.CargoId;
            existente.Area = empleado.Area;
            existente.Telefono = empleado.Telefono;
            existente.Email = empleado.Email;
            existente.FechaIngreso = empleado.FechaIngreso;
            existente.Activo = empleado.Activo;

            _db.Empleados.Update(existente);
            await _db.SaveChangesAsync();

            return (true, "Empleado actualizado exitosamente.");
        }
        catch (Exception ex)
        {
            return (false, $"Error al actualizar empleado: {ex.Message}");
        }
    }

    public async Task<(bool Exito, string Mensaje)> ActivarDesactivarAsync(int empleadoId)
    {
        try
        {
            var emp = await _db.Empleados.FindAsync(empleadoId);
            if (emp == null)
                return (false, "El empleado no existe.");

            emp.Activo = !emp.Activo;
            _db.Empleados.Update(emp);
            await _db.SaveChangesAsync();

            var estadoStr = emp.Activo ? "activado" : "desactivado";
            return (true, $"Empleado {emp.NombreCompleto} {estadoStr} correctamente.");
        }
        catch (Exception ex)
        {
            return (false, $"Error al cambiar estado del empleado: {ex.Message}");
        }
    }

    public async Task<string> GenerarCodigoAsync()
    {
        var count = await _db.Empleados.CountAsync();
        return $"EMP-{(count + 1):D3}";
    }
}
