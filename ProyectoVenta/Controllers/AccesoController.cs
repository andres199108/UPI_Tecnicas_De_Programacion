﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoVenta.Datos;
using ProyectoVenta.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ProyectoVenta.Controllers
{
    //[Authorize]
    //public class AutocompleteController : Controller
    public class AccesoController : Controller
        {
        DA_Usuario _daUsuario = new DA_Usuario();

        public IActionResult Index()
            {
            return View();
            }


        [HttpPost]
        public async Task<IActionResult> Index(string correo, string clave)
            {

            Usuario ouser = new Usuario();
            ouser = _daUsuario.Listar().Where(u => u.Correo == correo && u.Clave == clave).FirstOrDefault();

            if (ouser == null)
                {
                ViewData["mensaje"] = "Usuario no encontrado";
                return View();
                }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ouser.Correo),
                new Claim("NombreCompleto", ouser.NombreCompleto),
                new Claim(ClaimTypes.Role, "Administrador"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            //SESIONES
            //HttpContext.Session.SetString("correo", correo);

            return RedirectToAction("Index", "Home");
            }

        public async Task<IActionResult> Salir()
            {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Acceso");
            }
        }
    }




//        DA_Autocomplete _daAutocomplete = new DA_Autocomplete();

//        [HttpGet]
//        public JsonResult ListaAutocomplete(string search)
//        {
//            List<Autocomplete> autocomplete = _daAutocomplete.Listar()
//                .Where(x => x.label.Contains(search, System.StringComparison.OrdinalIgnoreCase))
//                .ToList();

//            return Json(autocomplete);
//        }

//        [HttpPost]
//        public JsonResult GuardarAutocomplete([FromBody] Autocomplete obj)
//        {
//            string operacion = Request.Headers["operacion"];
//            bool respuesta;

//            if (operacion == "crear")
//            {
//                respuesta = _daAutocomplete.Guardar(obj);
//            }
//            else
//            {
//                respuesta = _daAutocomplete.Editar(obj);
//            }

//            return Json(new { respuesta = respuesta });
//        }

//        [HttpDelete]
//        public JsonResult EliminarAutocomplete(int value)
//        {
//            bool respuesta = _daAutocomplete.Eliminar(value);
//            return Json(new { respuesta = respuesta });
//        }
//    }
//}

