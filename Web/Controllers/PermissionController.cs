using Business;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionBusiness _PermissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(PermissionBusiness PermissionBusiness, ILogger<PermissionController> logger)
        {
            _PermissionBusiness = PermissionBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var Permissions = await _PermissionBusiness.GetAllPermissionAsync();
                return Ok(Permissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var Permission = await _PermissionBusiness.GetPermissionByIdAsync(id);
                return Ok(Permission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDto PermissionDto)
        {
            try
            {
                var createdPermission = await _PermissionBusiness.CreatePermissionAsync(PermissionDto);
                return CreatedAtAction(nameof(GetPermissionById), new { id = createdPermission.Id }, createdPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un permiso existente
        /// </summary>
        /// <param name="id">ID del permiso a actualizar</param>
        /// <param name="permissionDto">Datos actualizados del permiso</param>
        /// <returns>Permiso actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionDto permissionDto)
        {
            try
            {
                var updatedPermission = await _PermissionBusiness.UpdatePermissionAsync(id, permissionDto);
                return Ok(updatedPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar permiso con ID: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar permiso con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un permiso por su ID
        /// </summary>
        /// <param name="id">ID del permiso a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                await _PermissionBusiness.DeletePermissionAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar permiso con ID: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar permiso con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        ///<summary>
        /// <summary>
        /// Desactiva un permission (eliminación lógica)
        /// </summary>
        /// <param name="id">ID del permission a desactivar</param>
        /// <returns>NoContent si fue exitoso</returns>
        [HttpDelete("{id}/disable")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Disablepermission(int id)
        {
            try
            {
                await _PermissionBusiness.DisableFormAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permission no encontrado para desactivación con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al desactivar Permission con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un permiso existente.
        /// </summary>
        /// <param name="id">ID del permiso a actualizar.</param>
        /// <param name="formDto">Datos parciales del permiso.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateForm(int id, [FromBody] PermissionDto permissionDto)
        {
            if (id != permissionDto.Id)
            {
                return BadRequest(new { message = "El ID del permiso no coincide con el del objeto." });
            }

            try
            {
                await _PermissionBusiness.PartialUpdateFormAsync(permissionDto);
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