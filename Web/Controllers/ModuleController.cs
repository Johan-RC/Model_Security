using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de módulos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusiness _ModuleBusiness;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(ModuleBusiness ModuleBusiness, ILogger<ModuleController> logger)
        {
            _ModuleBusiness = ModuleBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllModules()
        {
            try
            {
                var Modules = await _ModuleBusiness.GetAllModuleAsync();
                return Ok(Modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModuleById(int id)
        {
            try
            {
                var Module = await _ModuleBusiness.GetModuleByIdAsync(id);
                return Ok(Module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto ModuleDto)
        {
            try
            {
                var createdModule = await _ModuleBusiness.CreateModuleAsync(ModuleDto);
                return CreatedAtAction(nameof(GetModuleById), new { id = createdModule.Id }, createdModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un módulo existente
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateModule([FromBody] ModuleDto ModuleDto)
        {
            try
            {
                var updated = await _ModuleBusiness.UpdateModuleAsync(ModuleDto);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado al actualizar con ID: {ModuleId}", ModuleDto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un módulo por ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                await _ModuleBusiness.DeleteModuleAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminar módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado para eliminar con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// <summary>
        /// Desactiva un module (eliminación lógica)
        /// </summary>
        /// <param name="id">ID del module a desactivar</param>
        /// <returns>NoContent si fue exitoso</returns>
        [HttpDelete("{id}/disable")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DisableModule(int id)
        {
            try
            {
                await _ModuleBusiness.DisableFormAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "module no encontrado para desactivación con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al desactivar module con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza parcialmente un permiso existente.
        /// </summary>
        /// <param name="id">ID del permiso a actualizar.</param>
        /// <param name="moduleDto">Datos parciales del permiso.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateForm(int id, [FromBody] ModuleDto moduleDto)
        {
            if (id != moduleDto.Id)
            {
                return BadRequest(new { message = "El ID del permiso no coincide con el del objeto." });
            }

            try
            {
                await _ModuleBusiness.PartialUpdateFormAsync(moduleDto);
                return NoContent(); // 204: Actualizado correctamente sin contenido de respuesta
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el permiso con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado para actualización parcial. ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar permiso parcialmente con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}