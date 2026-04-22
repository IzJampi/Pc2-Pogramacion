Pc2_Pogramacion

Aplicación web desarrollada en ASP.NET Core MVC para la gestión de cursos y matrículas.

Tecnologías
-.NET 8 (ASP.NET Core MVC)
-Entity Framework Core
-SQLite
-Identity (usuarios y roles)
-Redis (cache)
-Docker + Render
-GitHub

Funcionalidades
-Listado de cursos con filtros
-Registro e inicio de sesión
-Inscripción a cursos
-Gestión de matrículas
-Rol "Coordinador" con acceso adicional
-Uso de sesión para recordar último curso

Validaciones en servidor
-Evita inscripción duplicada
-Control de cupo máximo
-Validación de cruce de horarios
-Validación de horario correcto

Cache y sesiones
-Cache de cursos con Redis (fallback a memoria si falla)
-Sesión para guardar último curso visitado

GitHub
-Uso de ramas para desarrollo
-Merges para integrar cambios

Despliegue
-Aplicación desplegada en Render usando Docker
-Variables configuradas:
-ConnectionStrings__DefaultConnection
-Redis__ConnectionString

Acceso
URL: (https://pc2-pogramacion.onrender.com/)

Usuario inicial
Email: admin@uni.com
Password: Admin123!
