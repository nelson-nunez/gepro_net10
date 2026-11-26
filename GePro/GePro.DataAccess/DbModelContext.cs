using Microsoft.EntityFrameworkCore;
using GePro.Model.Model;
using GePro.Model.Base;
using GePro.Model.ModelUser;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;

namespace GePro.DataAccess
{
    public class DbModelContext : DbContext
    {
        public DbSet<Address> Address { get; set; } = null!;
        public DbSet<City> City { get; set; } = null!;
        public DbSet<Neighborhood> Neighborhood { get; set; } = null!;
        public DbSet<Province> Province { get; set; } = null!;
        public DbSet<Person> Person { get; set; } = null!;
        public DbSet<Customer> Customer { get; set; } = null!;
        public DbSet<Employee> Employee { get; set; } = null!;
        public DbSet<Gender> Gender { get; set; } = null!;
        public DbSet<AppUser> AppUser { get; set; } = null!;
        public DbSet<Role> Role { get; set; } = null!;

        public DbModelContext(DbContextOptions<DbModelContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>().HasKey(x => new { x.Id });
            modelBuilder.Entity<City>().HasKey(x => new { x.Id });
            modelBuilder.Entity<Neighborhood>().HasKey(x => new { x.Id });
            modelBuilder.Entity<Province>().HasKey(x => new { x.Id });
            modelBuilder.Entity<Customer>().HasKey(x => new { x.Id });
            modelBuilder.Entity<Employee>().HasKey(x => new { x.Id });
            modelBuilder.Entity<Gender>().HasKey(x => new { x.Id });

            modelBuilder.Entity<Customer>().HasIndex(x => x.PersonId).IsUnique();
            modelBuilder.Entity<Customer>().HasIndex(x => x.Code).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(x => x.PersonId).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(x => x.FileNumber).IsUnique();

            //Elimina ciclos de eliminacion en cascada
            var cascadeFKs = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            AddMyFilters(ref modelBuilder);

            modelBuilder.Seed();
        }

        private void AddMyFilters(ref ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //other automated configurations left out   
                if (entityType.ClrType != null && typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }
        }
    }

    public static class ModelBuilderExtensions
    {
        // Fecha fija: HasData con DateTime.Now cambia el modelo en cada compilación.
        private static readonly DateTime SeedDate = new(2025, 1, 1);

        public static void Seed(this ModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
            {
                SeedAppUser(modelBuilder);
                SeedRole(modelBuilder);
                
                SeedProvince(modelBuilder);
                SeedCity(modelBuilder);
                SeedNeighborhood(modelBuilder);
                SeedAddress(modelBuilder);

                SeedGender(modelBuilder);
                SeedPerson(modelBuilder);

            }
        }

        #region User

        private static void SeedAppUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                Name = "admin",
                // Hash PBKDF2 de la contraseña inicial "admin" (cambiar en el primer ingreso).
                PasswordHash = "AQAAAAIAAYagAAAAEJeFXFjbZCF9ZWRpv7cbxW/XYfVyQNABFmvZqBz5eyHBXtpxB2IyWcCU2neqeQAQlg==",
                RoleId = 1,
                CreatedBy = "System",
                Created = SeedDate,
            });
        }
        private static void SeedRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Permissions = "FullAccess",
                CreatedBy = "System",
                Created = SeedDate,
            });
        }

        #endregion

        #region Address
    
        private static void SeedProvince(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Province>().HasData(
                new Province { Id = 1, Name = "Buenos Aires", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 2, Name = "Córdoba", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 3, Name = "Catamarca", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 4, Name = "Chaco", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 5, Name = "Chubut", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 6, Name = "Corrientes", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 7, Name = "Entre Ríos", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 8, Name = "Formosa", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 9, Name = "Jujuy", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 10, Name = "La Pampa", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 11, Name = "La Rioja", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 12, Name = "Mendoza", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 13, Name = "Misiones", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 14, Name = "Neuquén", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 15, Name = "Río Negro", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 16, Name = "Salta", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 17, Name = "San Juan", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 18, Name = "San Luis", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 19, Name = "Santa Cruz", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 20, Name = "Santa Fe", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 21, Name = "Santiago del Estero", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 22, Name = "Tierra del Fuego", CreatedBy = "System", Created = SeedDate },
                new Province { Id = 23, Name = "Tucumán", CreatedBy = "System", Created = SeedDate }
            );
        }
        private static void SeedCity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                // CHACO
                new City { Id = 1, Name = "Resistencia", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 2, Name = "Presidencia Roque Sáenz Peña", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 3, Name = "Barranqueras", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 4, Name = "Villa Ángela", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 5, Name = "Fontana", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 6, Name = "Charata", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 7, Name = "Quitilipi", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 8, Name = "General San Martín", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 9, Name = "Las Breñas", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 10, Name = "Castelli", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 11, Name = "Corzuela", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 12, Name = "Machagai", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 13, Name = "La Leonesa", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 14, Name = "San Bernardo", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 15, Name = "Las Palmas", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 16, Name = "General Pinedo", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 17, Name = "Puerto Tirol", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 18, Name = "Margarita Belén", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 19, Name = "Tres Isletas", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 20, Name = "La Escondida", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 21, Name = "Puerto Vilelas", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 22, Name = "Puerto Bermejo", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 23, Name = "Hermoso Campo", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 24, Name = "Villa Berthet", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 25, Name = "Colonias Unidas", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 26, Name = "General Vedia", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 27, Name = "Misión Nueva Pompeya", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 28, Name = "Miraflores", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 29, Name = "Napenay", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 30, Name = "Gancedo", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 31, Name = "Samuhú", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 32, Name = "Pampa del Infierno", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 33, Name = "Campo Largo", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 34, Name = "Fuerte Esperanza", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 35, Name = "Avia Terai", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 36, Name = "La Verde", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 37, Name = "Colonia Elisa", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 38, Name = "Capitán Solari", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 39, Name = "La Tigra", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 40, Name = "Enrique Urien", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 41, Name = "Los Frentones", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 42, Name = "Pampa del Indio", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 43, Name = "Puerto Eva Perón", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                new City { Id = 44, Name = "Ciervo Petiso", ProvinceId = 4, CreatedBy = "System", Created = SeedDate },
                //FORMOSA
                new City { Id = 45, Name = "Formosa", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 46, Name = "Clorinda", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 47, Name = "Pirané", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 48, Name = "El Colorado", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 49, Name = "Laguna Blanca", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 50, Name = "Ingeniero Juárez", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 51, Name = "General Manuel Belgrano", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 52, Name = "Villa Dos Trece", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 53, Name = "Ibarreta", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 54, Name = "Las Lomitas", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 55, Name = "Comandante Fontana", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 56, Name = "San Francisco de Laishí", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 57, Name = "Misión Tacaaglé", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 58, Name = "Herradura", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 59, Name = "Estanislao del Campo", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 60, Name = "Buena Vista", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 61, Name = "Laguna Naick Neck", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 62, Name = "Gran Guardia", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 63, Name = "Tres Lagunas", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 64, Name = "Riacho He Hé", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 65, Name = "Laguna Yema", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 66, Name = "Mayor Vicente Villafañe", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 67, Name = "Subteniente Perín", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 68, Name = "Misión San Martín", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 69, Name = "El Espinillo", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 70, Name = "Siete Palmas", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 71, Name = "Palo Santo", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 72, Name = "Villa Escolar", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 73, Name = "Loma Monte Lindo", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 74, Name = "General Lucio V. Mansilla", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 75, Name = "Colonia Pastoril", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 76, Name = "Fortín Lugones", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 77, Name = "Pozo del Tigre", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 78, Name = "Las Cañitas", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 79, Name = "El Potrillo", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 80, Name = "Palma Sola", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 81, Name = "San Hilario", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 82, Name = "Colonia Ituzaingó", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                new City { Id = 83, Name = "General Güemes", ProvinceId = 8, CreatedBy = "System", Created = SeedDate },
                // CORRIENTES
                new City { Id = 84, Name = "Corrientes", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 85, Name = "Goya", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 86, Name = "Paso de los Libres", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 87, Name = "Mercedes", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 88, Name = "Bella Vista", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 89, Name = "Santo Tomé", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 90, Name = "Esquina", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 91, Name = "Monte Caseros", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 92, Name = "Curuzú Cuatiá", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 93, Name = "Ituzaingó", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 94, Name = "Mocoretá", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 95, Name = "Saladas", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 96, Name = "Sauce", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 97, Name = "San Luis del Palmar", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 98, Name = "Empedrado", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 99, Name = "Santa Lucía", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 100, Name = "Concepción", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 101, Name = "San Roque", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 102, Name = "Paso de la Patria", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 103, Name = "Alvear", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 104, Name = "Riachuelo", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 105, Name = "San Miguel", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 106, Name = "Santa Rosa", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 107, Name = "San Lorenzo", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 108, Name = "Colonia Carlos Pellegrini", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 109, Name = "San Cosme", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 110, Name = "Colonia Libertad", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 111, Name = "Loreto", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 112, Name = "San Carlos", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 113, Name = "Yapeyú", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 114, Name = "Bonpland", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 115, Name = "Berón de Astrada", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 116, Name = "Juan Pujol", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 117, Name = "Gobernador Virasoro", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 118, Name = "Itatí", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 119, Name = "Chavarría", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 120, Name = "Tapebicuá", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 121, Name = "Parada Pucheta", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 122, Name = "Perugorría", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 123, Name = "Felipe Yofre", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 124, Name = "Ramón Lista", ProvinceId = 3, CreatedBy = "System", Created = SeedDate },
                new City { Id = 125, Name = "Villa Olivari", ProvinceId = 3, CreatedBy = "System", Created = SeedDate }
            );
        }
        private static void SeedNeighborhood(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Neighborhood>().HasData(
            new Neighborhood
            {
                Id = 1,
                Name = "Centro",
                CityId = 1,
                CreatedBy = "System",
                Created = SeedDate,
            },
            new Neighborhood
            {
                Id = 2,
                Name = "Nueva Córdoba",
                CityId = 2,
                CreatedBy = "System",
                Created = SeedDate,
            });
        }
        private static void SeedAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().HasData(
            new Address
            {
                Id = 1,
                ProvinceId = 1,
                CityId = 1,
                NeighborhoodId = 1,
                StreetNumber = "1234",
                NeighborhoodName = "Centro",
                CreatedBy = "System",
                Created = SeedDate,
            });

            modelBuilder.Entity<Address>().HasData(
            new Address
            {
                Id = 2,
                ProvinceId = 1,
                CityId = 1,
                NeighborhoodId = 1,
                StreetNumber = "1234",
                NeighborhoodName = "Centro 222",
                CreatedBy = "System",
                Created = SeedDate,
            });
        }

        #endregion

        #region Person

        private static void SeedGender(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>().HasData(
            new Gender
            {
                Id = 1,
                Name = "Masculino",
                CreatedBy = "System",
                Created = SeedDate,
            },
            new Gender
            {
                Id = 2,
                Name = "Femenino",
                CreatedBy = "System",
                Created = SeedDate,
            },
            new Gender
            {
                Id = 3,
                Name = "Otro",
                CreatedBy = "System",
                Created = SeedDate,
            });
        }

        public static void SeedPerson(ModelBuilder modelBuilder)
        {
            // Insertar datos en la tabla Person
            modelBuilder.Entity<Person>().HasData(
                new Person
                {
                    Id = 1,
                    Name = "Juan",
                    LastName = "Pérez",
                    DNI = "20123456789",
                    BirthDate = new DateTime(1985, 5, 15),
                    Phone = "1234567890",
                    Email = "juan.perez@example.com",
                    GenderId = 1,
                    AddressId = 1,
                    CreatedBy = "System",
                    Created = SeedDate
                },
                new Person
                {
                    Id = 2,
                    Name = "María",
                    LastName = "González",
                    DNI = "20987654321",
                    BirthDate = new DateTime(1978, 11, 30),
                    Phone = "0987654321",
                    Email = "maria.gonzalez@example.com",
                    GenderId = 2,
                    AddressId = 2,
                    CreatedBy = "System",
                    Created = SeedDate
                }
            );

            // Insertar datos en la tabla Customer
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    PersonId = 1,
                    CreatedBy = "System",
                    Created = SeedDate
                }
            );
        }

        #endregion
    }
}
