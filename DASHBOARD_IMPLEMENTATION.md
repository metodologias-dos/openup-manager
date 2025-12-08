# Dashboard - Implementación Completa

## Resumen de Cambios

### 1. **Servicios y Repositorios**

#### Nuevos Métodos en Repositorios:
- **ArtifactRepository**: 
  - `CountMandatoryByProjectIdAsync()` - Cuenta artefactos obligatorios
  - `CountMandatoryWithVersionsByProjectIdAsync()` - Cuenta artefactos obligatorios con al menos una versión

- **IterationRepository**:
  - `GetActiveIterationsByProjectIdAsync()` - Obtiene todas las iteraciones activas del proyecto

- **MicroincrementRepository**:
  - `CountByIterationIdAsync()` - Cuenta microincrementos por iteración

#### Servicio de Dashboard:
- **IDashboardService / DashboardService**: Servicio especializado que agrega datos de múltiples repositorios
- **DTOs creados**:
  - `DashboardData` - Contenedor principal
  - `PhaseStatusDto` - Estado de fases
  - `ActiveIterationDto` - Información de iteraciones activas
  - `ArtifactProgressDto` - Progreso de artefactos
  - `ProjectStatisticsDto` - Estadísticas del proyecto

### 2. **Métricas Implementadas**

#### Artefactos:
- Total de artefactos obligatorios
- Artefactos obligatorios registrados (con al menos una versión)
- Porcentaje de completitud

#### Iteraciones:
- Total de iteraciones en el proyecto
- Iteración activa (solo una por proyecto)
- Duración de cada iteración
- Microincrementos por iteración

#### Estadísticas Calculadas:
- Total de microincrementos en el proyecto
- Promedio de microincrementos por iteración
- Duración promedio de iteraciones
- Cantidad de fases completadas

### 3. **Diseño del Dashboard**

#### Estructura Visual (3 columnas):

**Fila Superior** (4 tarjetas de métricas clave):
1. Artefactos obligatorios con porcentaje
2. Total de iteraciones
3. Total de microincrementos
4. Promedio de microincrementos por iteración

**Columna Izquierda**: Estado de Fases
- Timeline visual con estados (Pendiente, En Progreso, Completada)
- Indicadores de color y iconos
- Duración promedio por iteración (destacado)

**Columna Central** (principal): Iteración Activa
- Enfoque en la única iteración activa del proyecto
- Badge "ACTIVA" prominente
- Objetivo de la iteración
- Métricas: período y duración
- Destacado de microincrementos registrados
- Estado vacío cuando no hay iteración activa

**Columna Derecha**: Resumen
- Fases completadas
- Iteraciones activas (contador)
- Barra de progreso general

### 4. **Características de Diseño**

✓ Sin emojis - Solo iconos y símbolos profesionales
✓ Textos formales en español
✓ Fuentes más grandes para mejor legibilidad
✓ Énfasis en la iteración activa única
✓ Big numbers para métricas importantes
✓ Diseño responsivo con cards

### 5. **Integración**

- Servicio registrado en DI container (`Program.cs`)
- DashboardWindow actualizado para recibir servicio e ID de proyecto
- ProjectView actualizado para abrir dashboard con datos reales

## Cálculos Importantes

### Porcentaje de Completitud:
```
completitud = (artefactos_obligatorios_con_versiones / total_artefactos_obligatorios) * 100
```

### Promedio de Microincrementos:
```
promedio = total_microincrementos / total_iteraciones
```

### Duración Promedio:
```
promedio_días = suma(duración_iteraciones) / cantidad_iteraciones_con_fechas
```

## Notas Técnicas

- Solo puede haber **una iteración activa** por proyecto
- Los artefactos se cuentan como "registrados" si tienen al menos una versión
- Las fases se ordenan por `OrderIndex`
- El servicio maneja errores y retorna `null` si no hay datos

## Archivos Modificados

### Backend:
- `OpenUpMan.Data/Artifact/IArtifactRepository.cs`
- `OpenUpMan.Data/Artifact/ArtifactRepository.cs`
- `OpenUpMan.Data/Iteration/IIterationRepository.cs`
- `OpenUpMan.Data/Iteration/IterationRepository.cs`
- `OpenUpMan.Data/Microincrement/IMicroincrementRepository.cs`
- `OpenUpMan.Data/Microincrement/MicroincrementRepository.cs`
- `OpenUpMan.Services/DataBase/Dashboard/IDashboardService.cs` (nuevo)
- `OpenUpMan.Services/DataBase/Dashboard/DashboardService.cs` (nuevo)
- `OpenUpMan.Services/DataBase/Dashboard/DashboardDtos.cs` (nuevo)

### Frontend:
- `OpenUpMan.UI/ViewModels/Dashboard/DashboardViewModel.cs`
- `OpenUpMan.UI/Views/Dashboard/DashboardView.axaml`
- `OpenUpMan.UI/Views/Dashboard/DashboardWindow.axaml.cs`
- `OpenUpMan.UI/Views/Project/ProjectView.axaml.cs`
- `OpenUpMan.UI/Program.cs`

