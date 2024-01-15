using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private static Random random = new Random();
        private readonly AppDbContext context;
        private readonly IConfiguration config;
        public LoginController(AppDbContext context, IConfiguration _config)
        {
            this.context = context;
            this.config = _config;
        }

        [HttpPost]
        [Route("PostValidarUsu")]
        public async Task<ActionResult<dynamic>> PostValidarUsu(Login_User ent)//string NombreUsuario, string PassUsuario
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

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_VALIDAR_USUARIO
                         @NombreUsuario={ent.NombreUsuario},
                        @PassUsuario={ent.PassUsuario}, 
                        @resp={parametroResp} OUTPUT");

                int respuesta = (int)parametroResp.Value;
                //context.Dispose();

                if (respuesta > 0)
                {
                    //creacion de token
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
                    // fin token

                    //datos usuario
                    var usuario_data = context.entidad_lst_usu
                    .FromSqlInterpolated($"Exec SP_USUARIO_SEL_06 @NombreUsuario={ent.NombreUsuario},@PassUsuario={ent.PassUsuario}")
                    .AsAsyncEnumerable();


                    response.status = 1;

                    await foreach (var usuario in usuario_data)
                    {
                        //usuario.PassUsuario = Encryptor.Decrypt(usuario.PassUsuario);
                        usuario.token = bearer_token;
                        datos_temp.Add(usuario);
                    }

                    foreach (var i in datos_temp)
                    {
                        var lst_rol_modulo = new List<entidad_lst_rol_modulo>();
                        //var lst_modulo_acceso = new List<entidad_lst_modulo_acceso>();

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
                            //lista
                            datos_modulo = lst_rol_modulo //(List<entidad_lst_rol_acceso>)accesos_rol
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
                                    //lista
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
                    //response.data = usuario_data;
                    //fin data

                    //return Ok(bearer_token);
                    //val_resp = bearer_token;
                }
                else
                {
                    //return Forbid();
                    //var item = new ItemResponse { message = "Usuario Incorrecto" };
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
                //var item = new ItemResponse { message = errorMessages.ToString() };
                //return item;
                //throw;
            }
            //return response;

            //return Ok(respuesta);

            /*var result = context.SP_VALIDAR_USUARIO
                .FromSqlInterpolated($"Exec SP_VALIDAR_USUARIO @NombreUsuario={NombreUsuario},@PassUsuario={PassUsuario}"
                .AsAsynEnumerable() );*///new SqlParameter("@id", id)
                                        //return "value";
        }

        [HttpPost]
        [Route("PostRecoverPassword")]
        public async Task<ActionResult<ItemResponse>> PostRecoverPassword(string email)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                string ramdon = RandomString(8);
                int IdUsuario = 0;

                var parametroResp = new SqlParameter("@IdUsuario", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_SEL_04 
                    @Email={email},
                    @IdUsuario={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    //response.status = (int)parametroResp.Value;

                    IdUsuario = (int)parametroResp.Value;
                    string agg = string.Format("{0}|{1}", ramdon, parametroResp.Value.ToString());
                    string url = config.GetValue<string>("UrlApp");

                    string rutaBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(agg));
                    url = url + "reset-password?code=" + rutaBase64;
                    //url = url + "/login/RecoverPassword?code=" + rutaBase64;

                    response = SendMailRecover(email, url);

                    if(response.status> 0)
                    {
                        await context.Database
                        .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_01
                         @IdUsuario={IdUsuario},
                        @CodigoCambioPassword={ramdon}
                        ");
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


        [HttpPost]
        [Route("PostCodeRecoverPassword")]
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
                    //responseCode.flgActiveLink = true;
                }
                else
                {
                    response.status = 0;
                    response.message = "El codigo es incorrecto";
                    //responseCode.flgActiveLink = false;
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


            [HttpPost]//no se usa
        [Route("PostListUserEmail")]
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

            return Ok(response); //val_resp;
            //return Ok(respuesta);
        }

        [HttpPost]//no se usa
        [Route("PostUpdateCodPassUsu")]
        public async Task<ActionResult<ItemResponse>> PostUpdateCodPassUsu(entity_cod_pass ent)
        {
            var response = new ItemResponse();

            try
            {
                await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_01
                         @IdUsuario={ent.IdUsuario},
                        @CodigoCambioPassword={ent.CodigoCambioPassword}
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

        [HttpPost]//no se usa
        [Route("PostCodeLogin_old")]
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

        [HttpPost]
        [Route("PostUpdatePassword")]
        public async Task<ActionResult<ItemResponse>> PostUpdatePassword(int IdUsuario, string PassUsuario)
        {
            //var val_resp = "";
            var response = new ItemResponse();
            //IdentityResult result =                

            try//if (result.Succeeded)
            {
                //var decrypted_text = Encryptor.Decrypt("PRL6/Sbwq8g=");
                PassUsuario = Encryptor.Encrypt(PassUsuario);

                await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_UPD_02
                         @IdUsuario={IdUsuario},
                        @PassUsuario={PassUsuario}
                        ");

                //val_resp = "Guardado";
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
                //val_resp = errorMessages.ToString();
                //throw;
            }

            return Ok(response); //val_resp;
        }






        ///*******metodos para correo*******************************
        public ItemResponse SendMailRecover(string correo, string enlace)
        {
            //var apiResponse = new ItemResponse("OK", "");
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                //SendMail mail = new SendMail();
                MailWorkIntegrity mailWorkIntegrity = new MailWorkIntegrity()
                {
                    Correo = correo,
                    keyBody = "NameFileRecoverPassword",
                    keySubject = "SubjectRecoverPassword",
                    enlace = enlace,
                    SubjectText = new string[] { }
                };

                //mail.ComposeMail   mail.SendMailAll SendMail.MailCompose
                List<Message> MailSend = ComposeMail(new List<MailWorkIntegrity> { mailWorkIntegrity },MailCompose.recoverPassword);
                var itemSend = MailSend[0];
                SendMailAll(itemSend.Address, itemSend.Subject, itemSend.Body, null, null, false);
                response.status =1;

            }
            catch (Exception ex)
            {
                response.status = 0;
                response.message = ex.Message.ToString();
            }
            return response;
        }

        public string RandomString(int length)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

       

        //--***********CLASE SendMail**********************************

        public enum MailCompose
        {
            NewSolicitud = 0,
            ChangeStatusSoli = 1,
            recoverPassword = 2,
            RiesgosExternos = 3
        }


        public List<Message> ComposeMail(List<MailWorkIntegrity> mailList, MailCompose mailCompose = MailCompose.NewSolicitud)
        {
            List<Message> list = new List<Message>();

            string subject = string.Empty;
            string keySubject = string.Empty;
            string body = string.Empty;
            string keyBody = string.Empty;
            string cliente = string.Empty;

            try
            {
                foreach (MailWorkIntegrity item in mailList)
                {
                    keySubject = item.keySubject;
                    keyBody = item.keyBody;

                    cliente = item.Usuario;
                    if (mailCompose == MailCompose.NewSolicitud)
                    {
                        subject = ComposeSubject(keySubject, item.SubjectText);
                        body = ComposeBody(keyBody, cliente, item.NroSolicitud, item.EstadoSol, item.Telefono, item.Empresa);
                    }
                    else if (mailCompose == MailCompose.ChangeStatusSoli)
                    {
                        subject = ComposeSubject(keySubject, item.SubjectText);
                        body = ComposeBodyChangeSol(keyBody, cliente, item.NroSolicitud, item.EstadoSol, item.Telefono, item.Empresa, item.DesServicio);
                    }
                    else if (mailCompose == MailCompose.recoverPassword)
                    {
                        subject = ComposeSubject(keySubject, item.SubjectText);
                        body = ComposeBodyRecoverPassword(keyBody, item.enlace);
                    }
                    else if (mailCompose == MailCompose.RiesgosExternos)
                    {
                        subject = ComposeSubject(keySubject, item.SubjectText);
                        body = ComposeBodyRiesgosExternos(keyBody, cliente, item.NombreInforme, item.Empresa);
                    }

                    list.Add(new Message
                    {
                        Address = item.Correo,
                        Subject = subject,
                        Body = body
                    });
                }
            }
            catch (Exception)
            {

            }

            return list;
        }
        private string ComposeSubject(string key, string[] value)
        {
            return string.Format(GetValueConfig(key), value);
        }
        private string ComposeBody(string key, string nombre, string nroSolicitud, string estadoSol, string telefono, string empresa)
        {
            string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));

            string readText = System.IO.File.ReadAllText(path);

            return readText
                .Replace("[username]", nombre)
                .Replace("[Empresa]", empresa)
                .Replace("[EstadoSol]", estadoSol)
                .Replace("[NroSolicitud]", nroSolicitud)
                .Replace("[Telefono]", telefono);
        }
        private string ComposeBodyChangeSol(string key, string nombre, string nroSolicitud, string estadoSol, string telefono, string empresa, string DesServ)
        {
            string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));

            string readText = System.IO.File.ReadAllText(path);

            return readText
                .Replace("[username]", nombre)
                .Replace("[Empresa]", empresa)
                .Replace("[EstadoSol]", estadoSol)
                .Replace("[NroSolicitud]", nroSolicitud)
                .Replace("[Telefono]", telefono)
                .Replace("[DesServicios]", DesServ);
        }

        private string ComposeBodyRecoverPassword(string key, string enlace)
        {
            //string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));

            //string readText = System.IO.File.ReadAllText(path);

            /*return readText
                .Replace("[enlace]", enlace);*/

            var _html = "";

            _html += "<html>";
            _html += "<head>";
            _html += "    <title></title>";
            _html += "	<meta charset='utf-8' />";
            _html += "	 <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' />";
            _html += "	 <script src='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js'></script>	";
            _html += "<style>";

            _html += ".details {";
            _html += "  color: green !important;";
            _html += "}";
            _html += "        .ctrl-sm{";
            _html += "                font-size: 9pt;";
            _html += "                padding: .25rem .5rem;";
            _html += "                line-height: 1.5;";
            _html += "                border-radius: .2rem;";
            _html += "                height: 28px;";
            _html += "        }";
            _html += "        .cls_border_th1{";
            _html += "            text-align: center;";
            _html += "            background: rgb(15, 74, 96);";
            _html += "            border: 5px solid #ffff;";
            _html += "            color: #ffff;";
            _html += "        }";
            _html += "         .cls_border_th2{";
            _html += "            text-align: center;";
            _html += "            background: rgb(146, 206, 193); /*rgb(65, 143, 126)*/";
            _html += "            border: 5px solid #ffff;";
            _html += "            color: #ffff;";
            _html += "        }";
            _html += "</style>";
            _html += "</head>";
            _html += "<body>";
            _html += "<div class='col-sm-12' style='padding: 28px;background: black;'>";
            //_html += "    <img src= 'http://mapa.suelourbano.org/eccofiles/Habitat/habitat_logo.png' style='width: 250px;color: white;'>";
            _html += "    <h3 style= 'color: white;float: right;margin: 0 !important;padding: 0;line-height: inherit;margin-top: 5px !important;' > SISTEMA</h3>";
            _html += "</div>";

            _html += " <div class='col-sm-12' style='padding:15px;'><b><a href='" + enlace + "' target='_blank'>Cambiar Contraseña</a></b></div>";
            //_html += "<br /><img  src='http://mapa.suelourbano.org/images/Logo_HFH.png' height='35px' alt='Logo' style='margin: 10px;' />";
            _html += "<span style='text-align:justify;font-size:11px;position: absolute;margin-top: 28px;'>";

            _html += "Evita imprimir, cuidemos al planeta. <br />";
            //_html += "AVISO DE PRIVACIDAD. Hábitat para la Humanidad México A.C., con domicilio en Avenida Xola número 162, Colonia Álamos, Delegación Benito Juárez en México, Distrito Federal, C.P. 03400, utilizará sus datos personales recabados para estar en aptitud de: perseguir el objeto de la Asociación, siendo éste la atención de requerimientos básicos de subsistencia en materia de vivienda, promoviendo la solidaridad, por medio del trabajo mutuo con la gente de escasos recursos para apoyarles a crear un ambiente mejor en el cual vivir y trabajar. Para mayor información acerca del tratamiento y de los derechos que puede hacer valer, usted puede acceder al aviso de privacidad completo a través de la página web http://www.habitatmexico.org,  dando clic en el apartado “Aviso de Privacidad”.";

            _html += "</span>";
            _html += "</body>";
            _html += "</html>";

            return _html;

        }

        private string ComposeBodySolicitud(string key, string nombre, string documento, string aplicativo)
        {
            string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));



            string readText = System.IO.File.ReadAllText(path);

            return readText
                .Replace("[Nombre]", nombre)
                .Replace("[Documento]", string.Format("<br /><strong>{0}</strong>", documento))
                .Replace("[Aplicativo]", string.Format("<br /><strong>{0}</strong>", aplicativo));
        }
        private string ComposeBodyRiesgosExternos(string key, string nombre, string nombreInforme, string empresa)
        {
            string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));

            string readText = System.IO.File.ReadAllText(path);

            return readText
                .Replace("[username]", nombre)
                .Replace("[Empresa]", empresa)
                .Replace("[InformeRiesgoExterno]", nombreInforme);
        }


        public void SendMailAll(string address, string subject, string body, string AddressCopy, string[] fileEntries = null, Boolean isAdjuntImage = true)
        {
            var message = new MailMessage();

            //string AddressCopy = GetValueConfig("CopyAddress");

            message.To.Add(new MailAddress(address));
            if (!string.IsNullOrEmpty(AddressCopy)) message.To.Add(new MailAddress(AddressCopy));
            message.Subject = subject;
            message.IsBodyHtml = true;

            string fileName = string.Format("{0}{1}", GetValueConfig("Templates"), "logo.png");

            AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

            if (isAdjuntImage)
            {

                LinkedResource lr = new LinkedResource(fileName, MediaTypeNames.Image.Jpeg);
                lr.ContentId = "Logo";
                av.LinkedResources.Add(lr);
            }

            message.AlternateViews.Add(av);
            message.Body = body;

            if (fileEntries != null)
            {
                foreach (string file in fileEntries)
                {
                    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(file);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                    message.Attachments.Add(data);
                }
            }

            try
            {
                string host = config.GetValue<string>("Smtp:host");
                int port = config.GetValue<int>("Smtp:port", 25);
                string fromAddress = config.GetValue<string>("Smtp:from");
                string userName = config.GetValue<string>("Smtp:userName");
                string password = config.GetValue<string>("Smtp:password");
               

                message.From = new MailAddress(fromAddress);                
                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(userName, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetValueConfig(string key)
        {

            string resp = config.GetValue<string>(key);
            return resp;//System.Configuration.ConfigurationManager.AppSettings[key];
        }

        //******************************************

        // GET: api/<LoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
