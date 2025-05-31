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
    /// Controlador para la gestión de permisos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CursoController : ControllerBase
    {
        private readonly ICursoBusiness _FormBusiness;
        private readonly ILogger<CursoController> _logger;

        public CursoController(ICursoBusiness FormBusiness, ILogger<CursoController> logger)
        {
            _FormBusiness = FormBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CursoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var Forms = await _FormBusiness.GetAllFormsAsync();
                return Ok(Forms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CursoDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormById(int id)
        {
            try
            {
                var Form = await _FormBusiness.GetFormByIdAsync(id);
                return Ok(Form);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CursoDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateForm([FromBody] CursoDTO FormDto)
        {
            try
            {
                var createdForm = await _FormBusiness.CreateFormAsync(FormDto);
                return CreatedAtAction(nameof(GetFormById), new { id = createdForm.Id }, createdForm);
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
        /// <param name="id">ID del permiso</param>
        /// <param name="FormDto">Datos actualizados del permiso</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] CursoDTO FormDto)
        {
            if (FormDto == null)
            {
                _logger.LogWarning("El cuerpo de la solicitud está vacío al intentar actualizar permiso.");
                return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });
            }

            if (id != FormDto.Id)
            {
                _logger.LogWarning("ID del permiso no coincide. ID en ruta: {RouteId}, ID en objeto: {ObjectId}", id, FormDto.Id);
                return BadRequest(new { message = "El ID del permiso no coincide con el del objeto." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo de permiso inválido al actualizar permiso con ID: {FormId}", id);
                return BadRequest(ModelState);
            }

            try
            {
                await _FormBusiness.UpdateFormAsync(FormDto);
                _logger.LogInformation("Permiso actualizado exitosamente con ID: {FormId}", id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar permiso con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error externo al actualizar permiso con ID: {FormId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar permiso con ID: {FormId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
            }
        }


        /// <summary>
        /// Elimina un permiso por su ID
        /// </summary>
        /// <param name="id">ID del permiso a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteForm(int id)
        {
            try
            {
                await _FormBusiness.DeleteFormAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado para eliminación con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar permiso con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// <summary>
        /// Desactiva un formulario (eliminación lógica)
        /// </summary>
        /// <param name="id">ID del formulario a desactivar</param>
        /// <returns>NoContent si fue exitoso</returns>
        [HttpDelete("{id}/disable")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DisableForm(int id)
        {
            try
            {
                await _FormBusiness.DisableFormAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Formulario no encontrado para desactivación con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al desactivar formulario con ID: {FormId}", id);
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
        public async Task<IActionResult> PartialUpdateForm(int id, [FromBody] CursoDTO formDto)
        {
            if (id != formDto.Id)
            {
                return BadRequest(new { message = "El ID del permiso no coincide con el del objeto." });
            }

            try
            {
                await _FormBusiness.PartialUpdateFormAsync(formDto);
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