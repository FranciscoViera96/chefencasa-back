using ChefEnCasa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Infrastructure.Configurations;

public partial class ChefEnCasaDbContext : DbContext
{
    public ChefEnCasaDbContext() { }

    public ChefEnCasaDbContext(DbContextOptions<ChefEnCasaDbContext> options) : base(options) { }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<PerfilSalud> PerfilesSalud { get; set; }
    public virtual DbSet<Suscripcion> Suscripciones { get; set; }
    public virtual DbSet<Verificacion> Verificaciones { get; set; } 

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }
    public virtual DbSet<Almacen> Almacenes { get; set; }

    public virtual DbSet<Receta> Recetas { get; set; }
    public virtual DbSet<RecetaIngrediente> RecetaIngredientes { get; set; }

    public virtual DbSet<PerfilAlergia> PerfilAlergias { get; set; }

    public virtual DbSet<ListaCompra> ListasCompras { get; set; }
    public virtual DbSet<ListaCompraDetalle> ListaCompraDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==========================================
        // 1. TABLA MAESTRA: USUARIOS
        // ==========================================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK_USUARIOS");
            entity.ToTable("USUARIOS");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(150).IsUnicode(false).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsUnicode(false).IsRequired();
            entity.Property(e => e.TelefonoPrefijo).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.TelefonoNumero).HasMaxLength(20).IsUnicode(false);

            // Campos nuevos
            entity.Property(e => e.EmailVerificado).HasDefaultValue(false);
            entity.Property(e => e.TelefonoVerificado).HasDefaultValue(false);
            entity.Property(e => e.ResetToken).HasMaxLength(100).IsUnicode(false);

            entity.Property(e => e.Puntos).HasDefaultValue(0);
            entity.Property(e => e.PoliciesAccepted).HasDefaultValue(false);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.FechaUltimaSesion).HasColumnType("datetime");
        });

        // ==========================================
        // 2. PERFIL DE SALUD Y ALERGIAS
        // ==========================================
        modelBuilder.Entity<PerfilSalud>(entity =>
        {
            entity.HasKey(e => e.PerfilSaludId).HasName("PK_PERFILESSALUD");
            entity.ToTable("PERFILESSALUD");

            // Configuración de decimales
            entity.Property(e => e.Peso).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Altura).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IMC).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TMB).HasColumnType("decimal(8, 2)");

            // Booleanos por defecto en falso
            entity.Property(e => e.EsVegetariano).HasDefaultValue(false);
            entity.Property(e => e.EsVegano).HasDefaultValue(false);
            entity.Property(e => e.EsCeliaco).HasDefaultValue(false);
            entity.Property(e => e.IntoleranteLactosa).HasDefaultValue(false);

            entity.HasOne(d => d.Usuario)
                .WithOne(p => p.PerfilSalud)
                .HasForeignKey<PerfilSalud>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PERFILSALUD_USUARIO");
        });

        modelBuilder.Entity<PerfilAlergia>(entity =>
        {
            entity.HasKey(e => e.PerfilAlergiaId);
            entity.ToTable("PERFIL_ALERGIAS");

            // Evitar que el usuario agregue "Tomate" dos veces a sus alergias
            entity.HasIndex(e => new { e.PerfilSaludId, e.IngredienteId }).IsUnique();

            entity.HasOne(d => d.PerfilSalud)
                .WithMany(p => p.Alergias)
                .HasForeignKey(d => d.PerfilSaludId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Ingrediente)
                .WithMany() // Ingrediente no necesita saber quién es alérgico a él
                .HasForeignKey(d => d.IngredienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        // ==========================================
        // 3. SUSCRIPCIONES
        // ==========================================
        modelBuilder.Entity<Suscripcion>(entity =>
        {
            entity.HasKey(e => e.SuscripcionId).HasName("PK_SUSCRIPCIONES");
            entity.ToTable("SUSCRIPCIONES");

            entity.Property(e => e.EstadoPremium).HasDefaultValue(false);
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaFin).HasColumnType("datetime");

            entity.HasOne(d => d.Usuario)
                .WithOne(p => p.Suscripcion)
                .HasForeignKey<Suscripcion>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SUSCRIPCION_USUARIO");
        });

        // ==========================================
        // 4. VERIFICACIONES (OTP)
        // ==========================================
        modelBuilder.Entity<Verificacion>(entity =>
        {
            entity.HasKey(e => e.VerificacionId).HasName("PK_VERIFICACIONES");
            entity.ToTable("VERIFICACIONES");

            entity.Property(e => e.Token).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.Tipo).HasMaxLength(20).IsUnicode(false).IsRequired();
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");

            // Si decides activar la relación de llave foránea en el futuro:
            // entity.HasOne(d => d.Usuario).WithMany().HasForeignKey(d => d.UsuarioId).HasConstraintName("FK_VERIFICACION_USUARIO");
        });

        // ALMACEN E INGREDIENTE
        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IngredienteId);
            entity.ToTable("INGREDIENTES"); // Arreglamos tu TOC de mayúsculas
            entity.HasIndex(e => e.NombreEspanol).IsUnique(); 

            // Nuevos campos
            entity.Property(e => e.Categoria).HasMaxLength(100).IsUnicode(false);
            // DiasVidaUtilEstimada no necesita configuración extra por ser int?
        });

        modelBuilder.Entity<Almacen>(entity =>
        {
            entity.HasKey(e => e.AlmacenId);
            entity.ToTable("ALMACENES");

            // LA VERSIÓN CORREGIDA PARA SOPORTAR LOTES FEFO
            entity.HasIndex(e => new { e.UsuarioId, e.IngredienteId, e.FechaCaducidad }).IsUnique();
        });

        // ==========================================
        // 8. RECETAS
        // ==========================================
        modelBuilder.Entity<Receta>(entity =>
        {
            entity.HasKey(e => e.RecetaId).HasName("PK_RECETAS");
            entity.ToTable("RECETAS");

            entity.HasIndex(e => e.Titulo);

            entity.Property(e => e.Titulo).HasMaxLength(200).IsUnicode(false).IsRequired();
            entity.Property(e => e.Resumen).IsUnicode(true); // Soporta emojis
            entity.Property(e => e.Instrucciones).IsUnicode(true);
            entity.Property(e => e.ImagenUrl).HasMaxLength(500).IsUnicode(false);

            // --- NUEVOS CAMPOS: DIETAS Y ALERGIAS ---
            entity.Property(e => e.EsVegetariano).HasDefaultValue(false);
            entity.Property(e => e.EsVegano).HasDefaultValue(false);
            entity.Property(e => e.EsSinGluten).HasDefaultValue(false);
            entity.Property(e => e.EsSinLacteos).HasDefaultValue(false);

            // --- NUEVOS CAMPOS: NUTRICIÓN ---
            entity.Property(e => e.Calorias).HasDefaultValue(0);
            entity.Property(e => e.Carbohidratos).HasColumnType("decimal(18, 2)").HasDefaultValue(0);
            entity.Property(e => e.Proteinas).HasColumnType("decimal(18, 2)").HasDefaultValue(0);
            entity.Property(e => e.Grasas).HasColumnType("decimal(18, 2)").HasDefaultValue(0);
        });

        // ==========================================
        // 9. RECETA_INGREDIENTES
        // ==========================================
        modelBuilder.Entity<RecetaIngrediente>(entity =>
        {
            entity.HasKey(e => e.RecetaIngredienteId).HasName("PK_RECETA_INGREDIENTES");
            entity.ToTable("RECETA_INGREDIENTES");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.UnidadMedida).HasMaxLength(50).IsUnicode(false).IsRequired();
            entity.Property(e => e.CantidadEnGramosOMl).HasColumnType("decimal(10, 2)").IsRequired();

            entity.HasOne(d => d.Receta)
                .WithMany(p => p.Ingredientes)
                .HasForeignKey(d => d.RecetaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RECETA_INGREDIENTES_RECETA");

            entity.HasOne(d => d.Ingrediente)
                .WithMany() // Una receta_ingrediente tiene un ingrediente, pero no necesitamos la lista inversa en el catálogo
                .HasForeignKey(d => d.IngredienteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RECETA_INGREDIENTES_INGREDIENTE");
        });

        // ==========================================
        // 10. RECETAS FAVORITAS
        // ==========================================
        modelBuilder.Entity<RecetaFavorita>(entity =>
        {
            entity.HasKey(e => e.RecetaFavoritaId);
            entity.ToTable("RECETAS_FAVORITAS");

            // Índice único para no duplicar favoritos (el usuario no puede guardar la misma receta 2 veces)
            entity.HasIndex(e => new { e.UsuarioId, e.RecetaId }).IsUnique();

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Receta)
                .WithMany()
                .HasForeignKey(d => d.RecetaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ==========================================
        // 11. LISTAS DE COMPRAS
        // ==========================================
        modelBuilder.Entity<ListaCompra>(entity =>
        {
            entity.HasKey(e => e.ListaCompraId);
            entity.ToTable("LISTAS_COMPRAS");

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ListaCompraDetalle>(entity =>
        {
            entity.HasKey(e => e.ListaCompraDetalleId);
            entity.ToTable("LISTA_COMPRA_DETALLES");

            entity.HasOne(d => d.ListaCompra)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.ListaCompraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Ingrediente)
                .WithMany()
                .HasForeignKey(d => d.IngredienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}