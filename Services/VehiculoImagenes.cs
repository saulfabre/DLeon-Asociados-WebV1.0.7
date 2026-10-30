using Microsoft.AspNetCore.Components.Forms;

namespace DLeon_Asociados_Web.Services;

public static class VehiculoImagenes
{
    public const long MaxFileSize = 5 * 1024 * 1024;

    public static void ValidarArchivo(IBrowserFile archivo)
    {
        ArgumentNullException.ThrowIfNull(archivo);

        var extension = Path.GetExtension(archivo.Name).ToLowerInvariant();
        var tipoEsperado = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => null
        };

        if (tipoEsperado is null || !string.Equals(archivo.ContentType, tipoEsperado, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Selecciona una imagen JPG, PNG o WebP.");
        }

        if (archivo.Size <= 0 || archivo.Size > MaxFileSize)
        {
            throw new InvalidOperationException("La imagen debe pesar menos de 5 MB.");
        }
    }

    public static async Task<string> GuardarAsync(
        IBrowserFile archivo,
        string webRootPath,
        CancellationToken cancellationToken = default)
    {
        ValidarArchivo(archivo);

        var extension = Path.GetExtension(archivo.Name).ToLowerInvariant();
        var directorioUploads = Path.Combine(webRootPath, "images", "vehiculos", "uploads");
        Directory.CreateDirectory(directorioUploads);

        var nombre = $"vehiculo-{Guid.NewGuid():N}{extension}";
        var ruta = Path.Combine(directorioUploads, nombre);

        await using var origen = archivo.OpenReadStream(MaxFileSize, cancellationToken);
        await using var destino = new FileStream(
            ruta,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);
        await origen.CopyToAsync(destino, cancellationToken);

        return $"/images/vehiculos/uploads/{nombre}";
    }

    public static void EliminarSubida(string? imagen, string webRootPath)
    {
        var rutaRelativa = imagen?.Trim().Replace('\\', '/').TrimStart('~', '/') ?? string.Empty;
        if (!rutaRelativa.StartsWith("images/vehiculos/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var raizWeb = Path.GetFullPath(webRootPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        var rutaCompleta = Path.GetFullPath(Path.Combine(
            raizWeb,
            rutaRelativa.Replace('/', Path.DirectorySeparatorChar)));

        if (rutaCompleta.StartsWith(raizWeb, StringComparison.OrdinalIgnoreCase) && File.Exists(rutaCompleta))
        {
            File.Delete(rutaCompleta);
        }
    }

    public static string ObtenerUrl(string? imagen, string webRootPath)
    {
        if (Uri.TryCreate(imagen, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return uri.ToString();
        }

        var ruta = imagen?.Trim().Replace('\\', '/').TrimStart('~', '/') ?? string.Empty;
        var nombre = Path.GetFileName(ruta);
        var candidatas = new[]
        {
            ruta,
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/vehiculos/uploads/{nombre}",
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/vehiculos/{nombre}",
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/imagesSaul/public/{nombre}",
            string.IsNullOrWhiteSpace(nombre) ? string.Empty : $"images/{nombre}"
        };

        var raizWeb = Path.GetFullPath(webRootPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        foreach (var candidata in candidatas.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct())
        {
            var rutaCompleta = Path.GetFullPath(Path.Combine(
                raizWeb,
                candidata.Replace('/', Path.DirectorySeparatorChar)));

            if (rutaCompleta.StartsWith(raizWeb, StringComparison.OrdinalIgnoreCase) &&
                File.Exists(rutaCompleta))
            {
                return "/" + candidata.TrimStart('/');
            }
        }

        return "/images/vehiculos/toyota-hilux.jpg";
    }
}
