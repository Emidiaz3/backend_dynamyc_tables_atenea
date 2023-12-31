﻿using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext context;
        //private readonly IConfiguration config;
        public UsuarioController(AppDbContext context)// IConfiguration _config
        {
            this.context = context;
            //this.config = _config;
        }


        //[HttpGet("{NombreUsuario,PassUsuario}")]
        //[AllowAnonymous]


        [HttpGet]
        [Route("GetListUsuarios")]
        public async Task<ActionResult<dynamic>> GetListUsuarios(int IdUsuarioAccion)//para listar todos los usuarios se manda sin datos los parametros
        {
            var response = new ItemResponse();

            try
            {
                string filtro = "and T_MAE_USUARIO.UsuarioAccion=" + IdUsuarioAccion;
                List<entidad_lst_tb_usuario> datos = new List<entidad_lst_tb_usuario>();
                var rol_usuario_data = context.entidad_lst_tb_usuario
                                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_07 @filtro={filtro}")
                                .AsAsyncEnumerable();

                await foreach (var dato in rol_usuario_data)
                {
                    dato.PassUsuario = Encryptor.Decrypt(dato.PassUsuario);
                    datos.Add(dato);
                }
                return Ok(datos);
                /*if (datos.Count() > 0)
                {
                    return Ok(datos);
                }
                else
                {
                    response.status = 0;
                    response.message = "Sin Registros";
                    return Ok(response);
                }*/
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
            //return NotFound();
        }


        /*[HttpGet]
        [Route("GetListEdit_usuario")]
        public async Task<ActionResult<dynamic>> GetListEdit_usuario(int IdUsuario)//para listar todos los usuarios se manda sin datos los parametros
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_edit_usuario> datos = new List<entidad_lst_edit_usuario>();
                var rol_usuario_data = context.entidad_lst_edit_usuario
                                .FromSqlInterpolated($"Exec SP_USUARIO_SEL_08 @IdUsuario={IdUsuario}")
                                .AsAsyncEnumerable();

                await foreach (var dato in rol_usuario_data)
                {
                    //return rol_usuario;
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
            //return NotFound();
        }*/
        //*******insert**********************

        //[HttpGet("{NombreUsuario,PassUsuario}")]
        [HttpPost]
        [Route("PostInsertUsu")]
        public async Task<ActionResult<ItemResponse>> PostInsertUsu(entidad_guardar_usuario ent)
        {
            //var val_resp = "";                          
            var response = new ItemResponse();
            try
            {
                var encrypted_text = Encryptor.Encrypt(ent.PassUsuario);

                //var decrypted_text = Encryptor.Decrypt("PRL6/Sbwq8g=");
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

                //val_resp = "Guardado";
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
                //val_resp = errorMessages.ToString();
                //throw;
            }
            //return val_resp;

            return Ok(response);
        }

        [HttpPost]
        [Route("PostUpdatePassUsu")]
        public async Task<ActionResult<ItemResponse>> PostUpdatePassUsu(entity_update_pass ent)
        {
            //var val_resp = "";
            var response = new ItemResponse();
            //IdentityResult result =                

            try//if (result.Succeeded)
            {
                //var decrypted_text = Encryptor.Decrypt("PRL6/Sbwq8g=");
                ent.PassUsuarioOld = Encryptor.Encrypt(ent.PassUsuarioOld);//SP_USUARIO_UPD_02
                ent.PassUsuarioNew = Encryptor.Encrypt(ent.PassUsuarioNew);

                await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_PASSWORD_UPD_01
                         @IdUsuario={ent.IdUsuario},
                        @PassUsuarioOld={ent.PassUsuarioOld},
                        @PassUsuarioNew={ent.PassUsuarioNew}
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

        [HttpGet]
        [Route("GetListPerfil")]
        public  async Task<ActionResult<dynamic>> GetListPerfil(int IdUsuario)
        {
            var response = new ItemResponse();   

            try
            {
                List<entidad_lst_perfil> datos = new List<entidad_lst_perfil>();
                var data_perfil = context.entidad_lst_perfil
                .FromSqlInterpolated($"Exec SP_PERFIL_SEL_01 @IdUsuario={IdUsuario}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data_perfil)
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
        [Route("updatePerfilImage")]
        public async Task<ActionResult> updatePerfilImage([FromForm] FormFilecs form)
        {
            try
            {
                int idUser = Int32.Parse(JsonConvert.DeserializeObject<string>(form.userId));
                string filePath = "";
                //Se junta las direcciones de guardado en un string
                List<string> joinToPathDocument = new List<string>();
                foreach (IFormFile document in form.file)
                {
                    //Si no existe que cree la carpeta DocumentsAnswers
                    //Direccion total seria : CuestionarioRepo\Encuestas_Back2\Encuestas_Back\ApiRestCuestionario\bin\Debug\netcoreapp3.1\DocumentsAnswers
                    //El numero significa el id del formularios
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsPerfilImage");
                    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "DocumentsPerfilImage\\" + idUser.ToString() + "\\", document.FileName);
                    joinToPathDocument.Add(filePath);
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsPerfilImage\\" + idUser.ToString());
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(fileStream);
                    }
                }
                var user = new t_mae_usuario { IdUsuario = idUser, FotoPerfil = filePath };
                context.t_mae_usuario.Attach(user).Property(x => x.FotoPerfil).IsModified = true;
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = "Subida de imagen correcta", data = filePath });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetPerfilImage")]
        public ActionResult GetPerfilImage([FromBody] JsonElement form)
        {
            try
            {
                string filePath = JsonConvert.DeserializeObject<String>(form.GetProperty("path").ToString());
                byte[] archivoBytes = System.IO.File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(archivoBytes);
                //Se junta las direcciones de guardado en un string

                return StatusCode(200, new ItemResp { status = 200, message = "Se obtuvo correctamente la imagen", data = base64 });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("PostUpdatePerfil")]
        public async Task<ActionResult<ItemResponse>> PostUpdatePerfil(entidad_actualizar_perfil ent)
        {
            //var val_resp = "";                          
            var response = new ItemResponse();
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                string ApellidoPaterno = "", ApellidoMaterno = "", Nombre = "";

                string[] datos_nom = ent.Nombres.Split(" ");

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
                    /*else if (datos_nom.Length == 3)
                    {
                        Nombre = datos_nom[0];
                        ApellidoPaterno = datos_nom[1];
                        ApellidoMaterno = datos_nom[2];                       
                    }*/
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
                /* @Fecha_Nac={ent.Fecha_Nac},
                        @Genero={ent.Genero},*/
                /* @IdTipoDocIdentidad={ent.IdTipoDocIdentidad},
                        @NumDocIdentidad={ent.NumDocIdentidad},*/
                //val_resp = "Guardado";
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
                //val_resp = errorMessages.ToString();
                //throw;
            }
            //return val_resp;

            return Ok(response);
        }


        //--********* no se utiliza

        [HttpPost]// no se usa
        [Route("PostSelUsuario")]
        public async Task<ActionResult<entidad_usuario>> PostSelUsuario(string NombreUsuario, string PassUsuario)
        {
            string error = "";
            var encrypted_text = Encryptor.Encrypt(PassUsuario);
            error += encrypted_text + Environment.NewLine;
            //var decrypted_text = Encryptor.Decrypt("Ji5OhtwJIS8=");
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

        //*************************************
        // GET: api/<UsuarioController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // GET api/<UsuarioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsuarioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
