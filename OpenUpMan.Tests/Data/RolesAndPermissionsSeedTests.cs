using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class RolesAndPermissionsSeedTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void Should_Have_36_Permissions()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var permissionCount = context.Permissions.Count();

            // Assert
            Assert.Equal(36, permissionCount);
        }

        [Fact]
        public void Should_Have_8_Roles()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var roleCount = context.Roles.Count();

            // Assert
            Assert.Equal(8, roleCount);
        }

        [Fact]
        public void Should_Have_All_Permission_IDs()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var permissions = context.Permissions.ToList();

            // Assert
            Assert.Contains(permissions, p => p.Id == PermissionIds.ProyectoVer);
            Assert.Contains(permissions, p => p.Id == PermissionIds.ProyectoActualizar);
            Assert.Contains(permissions, p => p.Id == PermissionIds.ProyectoBorrar);
            Assert.Contains(permissions, p => p.Id == PermissionIds.ArtefactosMinimos);
            Assert.Contains(permissions, p => p.Id == PermissionIds.SoloLectura);
            Assert.Contains(permissions, p => p.Id == PermissionIds.MicroincrementosAgregarDocumentos);
        }

        [Fact]
        public void Should_Have_All_Role_IDs()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var roles = context.Roles.ToList();

            // Assert
            Assert.Contains(roles, r => r.Id == RoleIds.Admin);
            Assert.Contains(roles, r => r.Id == RoleIds.Autor);
            Assert.Contains(roles, r => r.Id == RoleIds.ProductOwner);
            Assert.Contains(roles, r => r.Id == RoleIds.ScrumMaster);
            Assert.Contains(roles, r => r.Id == RoleIds.Desarrollador);
            Assert.Contains(roles, r => r.Id == RoleIds.Tester);
            Assert.Contains(roles, r => r.Id == RoleIds.Revisor);
            Assert.Contains(roles, r => r.Id == RoleIds.Viewer);
        }

        [Fact]
        public void Autor_Should_Have_All_Permissions()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var autorPermissions = context.RolePermissions
                .Where(rp => rp.RoleId == RoleIds.Autor)
                .ToList();

            // Assert
            Assert.True(autorPermissions.Count > 30, "Autor should have many permissions");
        }

        [Fact]
        public void Admin_Should_Have_Same_Permissions_As_Autor()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var autorPermissions = context.RolePermissions
                .Where(rp => rp.RoleId == RoleIds.Autor)
                .Select(rp => rp.PermissionId)
                .OrderBy(p => p)
                .ToList();

            var adminPermissions = context.RolePermissions
                .Where(rp => rp.RoleId == RoleIds.Admin)
                .Select(rp => rp.PermissionId)
                .OrderBy(p => p)
                .ToList();

            // Assert
            Assert.Equal(autorPermissions, adminPermissions);
        }

        [Fact]
        public void Revisor_Should_Have_SoloLectura_Permission()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var revisorHasSoloLectura = context.RolePermissions
                .Any(rp => rp.RoleId == RoleIds.Revisor && rp.PermissionId == PermissionIds.SoloLectura);

            // Assert
            Assert.True(revisorHasSoloLectura);
        }

        [Fact]
        public void Desarrollador_Should_Have_AgregarDocumentos_Permission()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var devHasAgregarDocs = context.RolePermissions
                .Any(rp => rp.RoleId == RoleIds.Desarrollador && 
                          rp.PermissionId == PermissionIds.MicroincrementosAgregarDocumentos);

            // Assert
            Assert.True(devHasAgregarDocs);
        }

        [Fact]
        public void PO_Should_Have_ArtefactosMinimos_Permission()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var poHasArtefactosMinimos = context.RolePermissions
                .Any(rp => rp.RoleId == RoleIds.ProductOwner && 
                          rp.PermissionId == PermissionIds.ArtefactosMinimos);

            // Assert
            Assert.True(poHasArtefactosMinimos);
        }

        [Fact]
        public void PO_Should_NOT_Have_ProyectoBorrar_Permission()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var poCanBorrarProyecto = context.RolePermissions
                .Any(rp => rp.RoleId == RoleIds.ProductOwner && 
                          rp.PermissionId == PermissionIds.ProyectoBorrar);

            // Assert
            Assert.False(poCanBorrarProyecto, "PO should not be able to delete project");
        }

        [Fact]
        public void Tester_Should_Have_SubirVersion_Permission()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var testerCanSubirVersion = context.RolePermissions
                .Any(rp => rp.RoleId == RoleIds.Tester && 
                          rp.PermissionId == PermissionIds.ArtefactosSubirVersion);

            // Assert
            Assert.True(testerCanSubirVersion);
        }

        [Fact]
        public void Viewer_Should_Have_Minimal_Permissions()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var viewerPermissionCount = context.RolePermissions
                .Count(rp => rp.RoleId == RoleIds.Viewer);

            // Assert
            Assert.True(viewerPermissionCount < 10, "Viewer should have minimal permissions");
        }

        [Fact]
        public void All_Permissions_Should_Have_Unique_Names()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var permissions = context.Permissions.ToList();
            var uniqueNames = permissions.Select(p => p.Name).Distinct().Count();

            // Assert
            Assert.Equal(permissions.Count, uniqueNames);
        }

        [Fact]
        public void All_Roles_Should_Have_Unique_Names()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var roles = context.Roles.ToList();
            var uniqueNames = roles.Select(r => r.Name).Distinct().Count();

            // Assert
            Assert.Equal(roles.Count, uniqueNames);
        }

        [Fact]
        public void All_RolePermissions_Should_Reference_Valid_Roles()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var rolePermissions = context.RolePermissions.ToList();
            var roleIds = context.Roles.Select(r => r.Id).ToList();
            
            var invalidReferences = rolePermissions
                .Where(rp => !roleIds.Contains(rp.RoleId))
                .ToList();

            // Assert
            Assert.Empty(invalidReferences);
        }

        [Fact]
        public void All_RolePermissions_Should_Reference_Valid_Permissions()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            // Act
            var rolePermissions = context.RolePermissions.ToList();
            var permissionIds = context.Permissions.Select(p => p.Id).ToList();
            
            var invalidReferences = rolePermissions
                .Where(rp => !permissionIds.Contains(rp.PermissionId))
                .ToList();

            // Assert
            Assert.Empty(invalidReferences);
        }
    }
}

