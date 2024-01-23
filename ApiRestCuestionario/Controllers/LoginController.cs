using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using ApiRestCuestionario.Utils;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;


namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration config;
        public LoginController(AppDbContext context, IConfiguration _config)
        {
            this.context = context;
            this.config = _config;
        }

        [HttpPost("PostValidarUsu")]
        public async Task<ActionResult<dynamic>> PostValidarUsu(Login_User ent)
        {
            var response = new ItemResponse();
            string error = "";

            try
            {
                var encrypted_text = Encryptor.Encrypt(ent.PassUsuario);
                error += encrypted_text + Environment.NewLine;
                var decrypted_text = Encryptor.Decrypt("Y8zud9wauW0=");
                ent.PassUsuario = encrypted_text;

                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database.ExecuteSqlInterpolatedAsync($@"Exec SP_VALIDAR_USUARIO @NombreUsuario={ent.NombreUsuario}, @PassUsuario={ent.PassUsuario}, @resp={parametroResp} OUTPUT");

                int respuesta = (int)parametroResp.Value;

                if (respuesta > 0)
                {
                    List<entidad_lst_usuario_acceso> datos = new List<entidad_lst_usuario_acceso>();
                    List<entidad_lst_usuario> datos_temp = new List<entidad_lst_usuario>();

                    var secretkey = config.GetValue<string>("SecretKey");
                    var key = Encoding.ASCII.GetBytes(secretkey);
                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, ent.NombreUsuario));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddHours(4),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var createdToken = tokenHandler.CreateToken(tokenDescriptor);

                    string bearer_token = tokenHandler.WriteToken(createdToken);

                    var usuario_data = context.entidad_lst_usu
                    .FromSqlInterpolated($"Exec SP_USUARIO_SEL_06 @NombreUsuario={ent.NombreUsuario},@PassUsuario={ent.PassUsuario}")
                    .AsAsyncEnumerable();


                    response.status = 1;

                    await foreach (var usuario in usuario_data)
                    {
                        usuario.token = bearer_token;
                        datos_temp.Add(usuario);
                    }

                    foreach (var i in datos_temp)
                    {
                        var lst_rol_modulo = new List<entidad_lst_rol_modulo>();

                        List<entidad_lst_modulos_accesos> lst_rol_modulo_temp = new List<entidad_lst_modulos_accesos>();
                        var accesos_rol = context.entidad_lst_modulos_accesos
                            .FromSqlInterpolated($"Exec SP_ACCESO_SEL_02 @IdRol={i.IdRol}")
                            .AsAsyncEnumerable();

                        datos.Add(new entidad_lst_usuario_acceso()
                        {
                            IdUsuario = i.IdUsuario,
                            IdPais = i.IdPais,
                            Pais = i.Pais,
                            IdEmpresa = i.IdEmpresa,
                            DescripcionEmpresa = i.DescripcionEmpresa,
                            IdSucursal = i.IdSucursal,
                            DescripcionSucursal = i.DescripcionSucursal,
                            NombreUsuario = i.NombreUsuario,
                            EstadoUsuario = i.EstadoUsuario,
                            ApellidoPaterno = i.ApellidoPaterno,
                            ApellidoMaterno = i.ApellidoMaterno,
                            Nombre = i.Nombre,
                            Email = i.Email,
                            IdTipoDocIdentidad = i.IdTipoDocIdentidad,
                            DesTipoDocIdentidad = i.DesTipoDocIdentidad,
                            NumDocIdentidad = i.NumDocIdentidad,
                            Telefono = i.Telefono,
                            UsuarioCreacion = i.UsuarioCreacion,
                            UsuarioAccion = i.UsuarioAccion,
                            FechaCreacion = i.FechaCreacion,
                            FechaAccion = i.FechaAccion,
                            CodigoCambioPassword = i.CodigoCambioPassword,
                            FlgCambioPassword = i.FlgCambioPassword,
                            FechaCambioPassword = i.FechaCambioPassword,
                            IdRol = i.IdRol,
                            DesRol = i.DesRol,
                            token = i.token,
                            datos_modulo = lst_rol_modulo 
                        });

                        var v_id_modulo = 0;

                        await foreach (var acceso in accesos_rol)
                        {
                            lst_rol_modulo_temp.Add(acceso);
                        }


                        foreach (var r in lst_rol_modulo_temp)
                        {
                            if (v_id_modulo != r.IdModulo)
                            {
                                var lst_modulo_acceso = new List<entidad_lst_modulo_acceso>();
                                lst_rol_modulo.Add(new entidad_lst_rol_modulo()
                                {
                                    IdModulo = r.IdModulo,
                                    title = r.NombreModulo,
                                    icon = r.Icon_Modulo,
                                    children = lst_modulo_acceso
                                });

                                foreach (var l in lst_rol_modulo_temp)
                                {
                                    if (l.IdModulo == r.IdModulo)
                                    {
                                        lst_modulo_acceso.Add(new entidad_lst_modulo_acceso()
                                        {
                                            IdAcceso = l.IdAcceso,
                                            title = l.NombreAcceso,
                                            Descripcion = l.Descripcion,
                                            type = "basic",
                                            icon = l.Icon_Acceso,
                                            link = l.Link,

                                        });
                                    }
                                }
                                v_id_modulo = r.IdModulo;
                            }
                        }
                    }
                    return Ok(datos);
                }
                else
                {
                    response.status = 0;
                    response.message = "Usuario Incorrecto";
                    return Ok(response);
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
                return Ok(response);
         
            }

        }

        [HttpPost("PostRecoverPassword")]
        public async Task<ActionResult<ItemResponse>> PostRecoverPassword(string email)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                string ramdon = Randomizer.generateString(8);
                int IdUsuario = 0;

                var parametroResp = new SqlParameter("@IdUsuario", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_SEL_04 
                    @Email={email},
                    @IdUsuario={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    IdUsuario = (int)parametroResp.Value;
                    string agg = string.Format("{0}|{1}", ramdon, parametroResp.Value.ToString());
                    string url = config.GetValue<string>("UrlApp");

                    string rutaBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(agg));
                    url = url + "reset-password?code=" + rutaBase64;

                    if(response.status> 0)
                    {
                        await context.Database
                        .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_01 @IdUsuario={IdUsuario}, @CodigoCambioPassword={ramdon}");
                        response.status = 1;
                    }
                    else
                    {
                        response.status = 0;
                        response.message = "error al enviar el correo";
                    }
                }
                else
                {
                    response.status = 0;
                    response.message = "El correo ingresado no existe";
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


        [HttpPost("PostCodeRecoverPassword")]
        public async Task<ActionResult<ItemResponse>> PostCodeRecoverPassword(string code)
        {
            var response = new ItemResponse();

            try
            {
                string inputStr = Encoding.UTF8.GetString(Convert.FromBase64String(code));

                string[] arrayDato = inputStr.Split('|');

                string codeVerificate = arrayDato[0];
                string idUsuario = arrayDato[1];
                List<entidad_lst_codigo> datos = new List<entidad_lst_codigo>();
                var data_codigo = context.entidad_lst_codigo
                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_03 @IdUsuario={idUsuario}")
                .AsAsyncEnumerable();

                response.status = 1;
                await foreach (var dato in data_codigo)
                {
                    datos.Add(dato);
                }

                if (datos[0].CodigoCambioPassword == codeVerificate)
                {
                    response.status = Convert.ToInt32(idUsuario);
                }
                else
                {
                    response.status = 0;
                    response.message = "El codigo es incorrecto";
                }

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


        [HttpPost("PostListUserEmail")]
        public async Task<ActionResult<ItemResponse>> PostIdUserEmail(entidad_correo ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@IdUsuario", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_SEL_04 
                    @Email={ent.Email},
                    @IdUsuario={parametroResp} OUTPUT");

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

        [HttpPost("PostUpdateCodPassUsu")]
        public async Task<ActionResult<ItemResponse>> PostUpdateCodPassUsu(entity_cod_pass ent)
        {
            var response = new ItemResponse();
            try
            {
                await context.Database.ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_01 @IdUsuario={ent.IdUsuario}, @CodigoCambioPassword={ent.CodigoCambioPassword}");
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

        [HttpPost("PostCodeLogin_old")]
        public async Task<ActionResult<dynamic>> PostCodeLogin_old(entidad_dato_id_user ent)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_codigo> datos = new List<entidad_lst_codigo>();
                var data_codigo = context.entidad_lst_codigo
                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_03 @IdUsuario={ent.IdUsuario}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data_codigo)
                {
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

        [HttpPost("PostUpdatePassword")]
        public async Task<ActionResult<ItemResponse>> PostUpdatePassword(int IdUsuario, string PassUsuario)
        {
            var response = new ItemResponse();
            try
            {
                PassUsuario = Encryptor.Encrypt(PassUsuario);
                await context.Database.ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_02 @IdUsuario={IdUsuario}, @PassUsuario={PassUsuario}");
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
    }
}
