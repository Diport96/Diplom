﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MqttWebApp.Data;
using MqttWebApp.Models.JwtSecurity;
using Newtonsoft.Json;

namespace MqttWebApp.Controllers.API
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthentificationController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task Post()
        {
            var username = Request.Form["Username"];
            var password = Request.Form["Password"];

            var identity = await _userManager.FindByNameAsync(username);
            if (identity == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username.");
                return;
            }

            var loginCornfirm = await _signInManager.CheckPasswordSignInAsync(identity, password, false);
            if (!loginCornfirm.Succeeded)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username or password.");
                return;
            }

            var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, identity.UserName),
            new Claim(ClaimTypes.NameIdentifier, username)
        };

            var jwt = new JwtSecurityToken(
                   issuer: TokenAuthOptions.ISSUER,
                   audience: TokenAuthOptions.AUDIENCE,
                   claims: claims,
                   expires: DateTime.Now.AddMinutes(TokenAuthOptions.LIFETIME),
                   signingCredentials: new SigningCredentials(TokenAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.UserName
            };

            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost("SubmitDeviceData")]
        public async Task SubmitDeviceData()
        {
            var connectionString = Request.Form["connectionString"];
            var user = User.Identity.Name;
            if (user == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("User is not exists");
                return;
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("DevicesData");

            using (var collectionNames = database.ListCollectionNames())
            {
                while (await collectionNames.MoveNextAsync())
                {
                    var currentCollectionsNames = collectionNames.Current;
                    foreach (var collectionName in currentCollectionsNames)
                    {
                        var collection = database.GetCollection<BsonDocument>(collectionName);
                        var cursor = await collection.FindAsync(new BsonDocument());
                        await cursor.ForEachAsync((x) =>
                        {
                            BsonDocument doc = new BsonDocument(x);
                            doc.Add("UserName", user);
                            // Next
                        }
                        );
                    }
                }
            }
        }

        [HttpGet(nameof(GetConnectionString))]
        public string GetConnectionString()
        {
            return Startup.MongoDbConnectionString;
        }
    }
}