using angular.Server.Model;
using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using ApiRestCuestionario.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;


namespace ApiRestCuestionario.Controllers
{
    public class AvatarDto
    {
        public int UserId { get; set; }
        public required IFormFile File { get; set; }

    }
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly StaticFolder staticFolder;
        public UsuarioController(AppDbContext context, StaticFolder staticFolder)
        {
            this.context = context;
            this.staticFolder = staticFolder;
        }



        [HttpGet("GetListUsuarios")]
        public async Task<ActionResult<dynamic>> GetListUsuarios(int IdUsuarioAccion)//para listar todos los usuarios se manda sin datos los parametros
        {
            var response = new ItemResponse();

            try
            {
                string filtro = $"and T_MAE_USUARIO.UsuarioAccion={IdUsuarioAccion}";
                List<entidad_lst_tb_usuario> datos = [];
                var rol_usuario_data = context.entidad_lst_tb_usuario
                                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_07 @filtro={filtro}")
                                .AsAsyncEnumerable();

                await foreach (var dato in rol_usuario_data)
                {
                    dato.PassUsuario = Encryptor.Decrypt(dato.PassUsuario!);
                    datos.Add(dato);
                }
                return Ok(datos);
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }


     
        [HttpPost("PostInsertUsu")]
        public async Task<ActionResult<ItemResponse>> PostInsertUsu(entidad_guardar_usuario ent)
        {
            var response = new ItemResponse();
            try
            {
                var encrypted_text = Encryptor.Encrypt(ent.PassUsuario!);

                ent.PassUsuario = encrypted_text;

                await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_GUARDAR_USUARIO
                         @IdUsuario={ent.IdUsuario},
                        @IdPais={ent.IdPais},
                        @IdEmpresa={ent.IdEmpresa},
                        @IdSucursal={ent.IdSucursal},
                        @NombreUsuario={ent.NombreUsuario},
                        @PassUsuario={ent.PassUsuario},
                        @EstadoUsuario={ent.EstadoUsuario},
                        @ApellidoPaterno={ent.ApellidoPaterno},
                        @ApellidoMaterno={ent.ApellidoMaterno},
                        @Nombre={ent.Nombre},
                        @Email={ent.Email},
                        @IdTipoDocIdentidad={ent.IdTipoDocIdentidad},
                        @NumDocIdentidad={ent.NumDocIdentidad},
                        @Telefono={ent.Telefono},
                        @UsuarioAccion={ent.UsuarioAccion},
                        @IdRol={ent.IdRol}
                        ");

                response.status = 1;
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
              
            }
            return Ok(response);
        }

        [HttpPost("PostUpdatePassUsu")]
        public async Task<ActionResult<ItemResponse>> PostUpdatePassUsu(entity_update_pass ent)
        {
            var response = new ItemResponse();

            try
            {
                ent.PassUsuarioOld = Encryptor.Encrypt(ent.PassUsuarioOld!);
                ent.PassUsuarioNew = Encryptor.Encrypt(ent.PassUsuarioNew!);

                await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_PASSWORD_UPD_01
                         @IdUsuario={ent.IdUsuario},
                        @PassUsuarioOld={ent.PassUsuarioOld},
                        @PassUsuarioNew={ent.PassUsuarioNew}
                        ");

                response.status = 1;
            }
            //else
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response); 
        }

        [HttpGet("GetListPerfil")]
        public ActionResult GetListPerfil(int IdUsuario)
        {
            var response = new ItemResponse();   

            try
            {
                entidad_lst_perfil? data_perfil =  context.entidad_lst_perfil
                .FromSqlInterpolated($"Exec SP_PERFIL_SEL_01 @IdUsuario={IdUsuario}").ToList().FirstOrDefault();

                response.status = 1;
                response.data = data_perfil;

                return Ok(response);
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }
        [HttpPost("updatePerfilImage")]
        public async Task<ActionResult> updatePerfilImage([FromForm] AvatarDto avatar)
        {
            try
            {
                int userId = avatar.UserId;
                IFormFile file = avatar.File!;
                string basePath = Path.Combine(staticFolder.Path, "Profile");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                string userPath = Path.Combine(basePath, userId.ToString());

                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }

                string filePath = Path.Combine(userPath, file.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath); // Asegúrate de que ningún otro proceso esté accediendo a este archivo
                }
                // Usar using para asegurar que el FileStream se cierre correctamente
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                string savePath = $"{userId}/{file.FileName}";
                var user = new Usuario { IdUsuario = userId, FotoPerfil = savePath };
                context.Usuarios.Attach(user).Property(x => x.FotoPerfil).IsModified = true;
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = "Subida de imagen correcta", data = savePath });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpDelete("deleteProfileImage/{userId}")]
        public async Task<IActionResult> DeleteProfileImage(int userId)
        {
            // Encuentra al usuario en la base de datos
            var user = await context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Verifica si el usuario tiene una imagen de perfil
            if (string.IsNullOrEmpty(user.FotoPerfil))
            {
                return NotFound(new { message = "No se encontró imagen de perfil para este usuario." });
            }

            // Construye la ruta completa al archivo de la imagen de perfil
            string basePath = Path.Combine(staticFolder.Path, "Profile");
            string filePath = Path.Combine(basePath, user.FotoPerfil);

            // Elimina el archivo de imagen si existe
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                // Elimina la referencia de la imagen de perfil en la base de datos
                user.FotoPerfil = null;
                await context.SaveChangesAsync();

                return Ok(new { message = "Imagen eliminada correctamente." });
            }
            else
            {
                return NotFound(new { message = "Archivo de imagen no encontrado." });
            }
        }


        [HttpPost("PostUpdatePerfil")]
        public async Task<ActionResult<ItemResponse>> PostUpdatePerfil(entidad_actualizar_perfil ent)
        {
            var response = new ItemResponse();
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                string ApellidoPaterno = "", ApellidoMaterno = "", Nombre = "";

                string[] datos_nom = ent.Nombres!.Split(" ");

                if (datos_nom.Length == 1)
                {
                    Nombre = datos_nom[0];
                }
                if (datos_nom.Length == 2)
                {
                    Nombre = datos_nom[0];
                    ApellidoPaterno = datos_nom[1];
                }
                if (datos_nom.Length == 3)
                {
                    Nombre = datos_nom[0];
                    ApellidoPaterno = datos_nom[1];
                    ApellidoMaterno = datos_nom[2];
                }
                else if (datos_nom.Length > 3)
                {
                    if (datos_nom.Length >=5 && (datos_nom[1].ToLower() == "de" || datos_nom[1].ToLower() == "del") )
                    {
                        Nombre = datos_nom[0] + " " + datos_nom[1] + " " + datos_nom[2];
                        ApellidoPaterno = datos_nom[3];
                        if (datos_nom.Length == 5)
                        {
                            ApellidoMaterno = datos_nom[4];
                        }
                        else if (datos_nom.Length == 6)
                        {
                            ApellidoMaterno = datos_nom[4] + " " + datos_nom[5];
                        }                        
                    }
                    
                    else if (datos_nom.Length == 4)
                    {
                        if(datos_nom[2].ToLower() == "de" || datos_nom[2].ToLower() == "del")
                        {
                            Nombre = datos_nom[0] ;
                            ApellidoPaterno = datos_nom[1];
                            ApellidoMaterno = datos_nom[2] + " " + datos_nom[3];
                        }
                        else
                        {
                            Nombre = datos_nom[0] + " " + datos_nom[1];
                            ApellidoPaterno = datos_nom[2];
                            ApellidoMaterno = datos_nom[3];
                        }                       

                    }else if (datos_nom.Length >= 5)
                    {
                        if (datos_nom.Length == 5 && (datos_nom[3].ToLower() == "de" || datos_nom[3].ToLower() == "del"))
                        {
                            Nombre = datos_nom[0] + " " + datos_nom[1];
                            ApellidoPaterno = datos_nom[2];
                            ApellidoMaterno = datos_nom[3] + " " + datos_nom[4];
                        }
                    }
                }                

                await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_03
                         @IdUsuario={ent.IdUsuario},  
                        @ApellidoPaterno={ApellidoPaterno},
                        @ApellidoMaterno={ApellidoMaterno},
                        @Nombre={Nombre},                       
                        @Email={ent.Email},                       
                        @Telefono={ent.Telefono},
                        @UsuarioAccion={ent.UsuarioAccion},
                        @resp={parametroResp} OUTPUT");
              
                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
 
            }

            return Ok(response);
        }



        [HttpPost("PostSelUsuario")]
        public async Task<ActionResult<entidad_usuario>> PostSelUsuario(string NombreUsuario, string PassUsuario)
        {
            string error = "";
            var encrypted_text = Encryptor.Encrypt(PassUsuario);
            error += encrypted_text + Environment.NewLine;
            PassUsuario = encrypted_text;

            var usuario_data = context.entidad_usuarios
                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_01 @NombreUsuario={NombreUsuario},@PassUsuario={PassUsuario}")
                .AsAsyncEnumerable();

            await foreach (var usuario in usuario_data)
            {
                return usuario;
            }

            return NotFound();
        }
    }
}
