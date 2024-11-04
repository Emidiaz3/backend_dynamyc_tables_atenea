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
using DotLiquid;


namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration config;
        private readonly IGmailSender emailSender;


        public LoginController(AppDbContext context, IConfiguration config, IGmailSender gmailSender)
        {
            this.context = context;
            this.config = config;
            this.emailSender = gmailSender;
        }

        [HttpPost("PostValidarUsu")]
        public async Task<ActionResult<dynamic>> PostValidarUsu(Login_User ent)
        {
            var response = new ItemResponse();

            try
            {
                // Encriptar la contraseña
                var encrypted_text = Encryptor.Encrypt(ent.PassUsuario);
                ent.PassUsuario = encrypted_text;

                // Parámetro de respuesta para el procedimiento almacenado
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                // Ejecutar el procedimiento almacenado para validar usuario
                await context.Database.ExecuteSqlInterpolatedAsync($@"Exec SP_VALIDAR_USUARIO @NombreUsuario={ent.NombreUsuario}, @PassUsuario={ent.PassUsuario}, @resp={parametroResp} OUTPUT");

                int respuesta = (int)parametroResp.Value;

                if (respuesta > 0)
                {
                    List<entidad_lst_usuario_acceso> datos = new List<entidad_lst_usuario_acceso>();
                    List<entidad_lst_usuario> datos_temp = new List<entidad_lst_usuario>();

                    string secretkey = config.GetValue<string>("SecretKey")!;
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

                    // Recuperar datos del procedimiento almacenado
                    var usuario_data = await context.entidad_lst_usu
                        .FromSqlRaw("Exec SP_USUARIO_SEL_06 @NombreUsuario = {0}, @PassUsuario = {1}", ent.NombreUsuario, ent.PassUsuario)
                        .AsNoTracking()
                        .ToListAsync();

                    response.status = 1;

                    foreach (var usuario in usuario_data)
                    {
                        usuario.token = bearer_token;
                        datos_temp.Add(usuario);
                    }

                    foreach (var i in datos_temp)
                    {
                        var lst_rol_modulo = new List<entidad_lst_rol_modulo>();
                        var lst_rol_modulo_temp = await context.entidad_lst_modulos_accesos
                            .FromSqlRaw("Exec SP_ACCESO_SEL_02 @IdRol = {0}", i.IdRol)
                            .AsNoTracking()
                            .ToListAsync();

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
                            EstadoUsuario = i.EstadoUsuario ? 1 : 0,
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
                            FlgCambioPassword = i.FlgCambioPassword == true ? 1 : 0,
                            FechaCambioPassword = i.FechaCambioPassword,
                            IdRol = i.IdRol,
                            DesRol = i.DesRol,
                            token = i.token,
                            FotoPerfil = i.FotoPerfil,
                            datos_modulo = lst_rol_modulo
                        });

                        var v_id_modulo = 0;

                        foreach (var acceso in lst_rol_modulo_temp)
                        {
                            if (v_id_modulo != acceso.IdModulo)
                            {
                                var lst_modulo_acceso = new List<entidad_lst_modulo_acceso>();
                                lst_rol_modulo.Add(new entidad_lst_rol_modulo()
                                {
                                    IdModulo = acceso.IdModulo,
                                    title = acceso.NombreModulo,
                                    icon = acceso.Icon_Modulo,
                                    children = lst_modulo_acceso
                                });

                                foreach (var l in lst_rol_modulo_temp.Where(x => x.IdModulo == acceso.IdModulo))
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
                                v_id_modulo = acceso.IdModulo;
                            }
                        }

                        // Guardar o actualizar token FCM
                        if (!string.IsNullOrEmpty(ent.FcmToken))
                        {
                            var tokenEntry = await context.T_MAE_USUARIO_TOKEN
                                .FirstOrDefaultAsync(t => t.IdUsuario == i.IdUsuario && t.Activo);
                            if (tokenEntry != null)
                            {
                                if (tokenEntry.Token != ent.FcmToken)
                                {
                                    tokenEntry.Token = ent.FcmToken;
                                    tokenEntry.FechaCreacion = DateTime.Now;
                                }
                            }
                            else
                            {
                                context.T_MAE_USUARIO_TOKEN.Add(new T_MAE_USUARIO_TOKEN
                                {
                                    IdUsuario = i.IdUsuario,
                                    Token = ent.FcmToken,
                                    Activo = true
                                });
                            }
                        }
                    }

                    // Guardar cambios en la base de datos solo para T_MAE_USUARIO_TOKEN
                    await context.SaveChangesAsync();

                    return Ok(datos);
                }
                else
                {
                    response.status = 0;
                    response.message = "Usuario o contraseña incorrectos";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.status = 0;
                response.message = ex.Message;
                return Ok(response);
            }
        }




        [HttpPost("PostRecoverPassword")]
        public async Task<ActionResult<ItemResponse>> PostRecoverPassword(string email)
        {
            var response = new ItemResponse
            {
                status = 0
            };
            try
            {
                string randomGeneratedString = Randomizer.generateString(8);

                var parametroResp = new SqlParameter("@IdUsuario", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_SEL_04 
                    @Email={email},
                    @IdUsuario={parametroResp} OUTPUT");

                Console.WriteLine("value");
                Console.WriteLine(parametroResp.Value);
                if (parametroResp.Value != DBNull.Value)
                {
                    int IdUsuario = (int)parametroResp.Value;
                    string agg = string.Format("{0}|{1}", randomGeneratedString, parametroResp.Value.ToString());
                    string applicationUrl = config.GetValue<string>("UrlApp")!;

                    string rutaBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(agg));

                    string recoveryUrl = $"{applicationUrl}/reset-password?code={rutaBase64}";
                    TemplateEntity? template = await context.Template.FirstOrDefaultAsync((e) => e.Id  == 1);
                    Template html = Template.Parse(template?.Body);
                    string renderedTemplate = html.Render(Hash.FromAnonymousObject(new { link = recoveryUrl, header= $"{applicationUrl}/assets/images/logo-grupoatenea.png" }));
                    await emailSender.SendEmailAsync(email, "Recuperación de contraseña | Cuestionario", renderedTemplate);
                    response.status = 1;
                    if(response.status> 0)
                    {
                        await context.Database
                        .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_01 @IdUsuario={IdUsuario}, @CodigoCambioPassword={randomGeneratedString}");
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
                
                List<entidad_lst_codigo> entities = await context.entidad_lst_codigo
                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_03 @IdUsuario={idUsuario}")
                .ToListAsync();
                entidad_lst_codigo? data = entities.FirstOrDefault();
                response.status = 1;
                
                
                if (data !=null && !string.IsNullOrEmpty(data?.CodigoCambioPassword) && data?.CodigoCambioPassword == codeVerificate)
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
