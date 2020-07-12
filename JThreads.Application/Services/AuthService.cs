using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using JThreads.Data.Dto;
using JThreads.Data.Dto.Auth;
using JThreads.Data.Entity;
using Microsoft.AspNetCore.Hosting;
using Google.Apis.Auth.OAuth2.Flows;
using JThreads.Data.Enums;
using System.Text.Json;

namespace JThreads.Application.Services
{
    public class AuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _currentEnvironment;
        private readonly IHttpClientFactory _clientFactory;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, IHttpContextAccessor contextAccessor, IHostingEnvironment currentEnvironment,
            IHttpClientFactory clientFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _currentEnvironment = currentEnvironment;
            _clientFactory = clientFactory;
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginDto.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(applicationUser, loginDto.Password, false);

            if (!result.Succeeded)
            {
                new Response()
                    .WithError("*", "Invalid username or password");
            }

            if (applicationUser.AuthType != AuthType.JThreads)
            {
                return new Response()
                    .WithError("*", "Account already authenticated with " + applicationUser.AuthType.ToString());
            }

            var token = GenerateJwt(applicationUser).ToString();
            GenerateCookie(token);

            return new DataResponse<TokenDto>()
                .WithData(new TokenDto() { Token = token });
        }

        public async Task<Response> Register(RegisterDto registerDto)
        {
            if (!registerDto.Password.Equals(registerDto.ConfirmPassword))
            {
                new Response().WithError("password", "Passwords must match");
            }

            var user = new ApplicationUser
            {
                //hack for now - force users to activate email before logging in?
                UserName = registerDto.Email,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                AuthType = AuthType.JThreads
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new Response()
                    .WithErrors(result.Errors
                        .Select(error => new Response.Error()
                        {
                            Key = error.Code == "DuplicateUserName" ? "email" : "*",
                            Msg = error.Code == "DuplicateUserName" ? "Email already in use" : error.Description
                        }).ToList());
            }

            //await _signInManager.SignInAsync(user, false);

            return new Response().WithSuccess();
            //response = new DataResponse<object>() { Data = GenerateJwt(user), Success = true };

        }

        public async Task<Response> GoogleLogin(OAuthLoginDto loginDto)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = _configuration["GoogleClientId"],
                    ClientSecret = _configuration["GoogleClientSecret"]
                },
                Scopes = new[] { "email", "profile" },
            });
            try
            {
                //Exchange / validate code with Google
                var googleToken = await flow.ExchangeCodeForTokenAsync("user", loginDto.Code,
                    _configuration["GoogleRedirectUri"], CancellationToken.None);
                var payload = (await GoogleJsonWebSignature.ValidateAsync(googleToken.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings()));
                var appUser = await _userManager.FindByNameAsync(payload.Email);

                //Check if this user has signed in before (if we have a AspNetUser record for them)
                if (appUser != null)
                {
                    if (appUser.AuthType != AuthType.Google)
                    {
                        return new Response().WithError("*", "Account already authenticated with " + appUser.AuthType.ToString());
                    }
                    if (appUser.AvatarUrl != payload.Picture)
                    {
                        appUser.AvatarUrl = payload.Picture;
                        await _userManager.UpdateAsync(appUser);


                    }
                    //await _signInManager.SignInAsync(appUser, false);
                }

                //No existing sign ins for this user, let's create a record for them
                else
                {
                    appUser = new ApplicationUser
                    {
                        //hack for now - force users to activate email before logging in?
                        UserName = payload.Email,
                        Email = payload.Email,
                        DisplayName = payload.GivenName,
                        AuthType = AuthType.Google,
                        AvatarUrl = payload.Picture
                    };
                    var result = await _userManager.CreateAsync(appUser);
                    if (!result.Succeeded)
                    {
                        return new Response()
                            .WithErrors(result.Errors
                                .Select(error => new Response.Error()
                                {
                                    Key = error.Code == "DuplicateUserName" ? "email" : "*",
                                    Msg = error.Code == "DuplicateUserName" ? "Email already in use" : error.Description
                                }).ToList());
                    }
                }

                //Create JWT and cookie for user and return success
                var token = GenerateJwt(appUser).ToString();
                GenerateCookie(token);

                return new DataResponse<TokenDto>()
                    .WithData(new TokenDto()
                    {
                        Token = token
                    });
            }
            catch (Exception e)
            {
                return new Response()
                    .WithError("*", "Error authenticating with Google: " + e.Message);
            }
        }

        public async Task<Response> ExchangeFacebookCodeForTokenAsync(string code)
        {
            var client = _clientFactory.CreateClient();

            var tokenResponse = await client.GetAsync($"https://graph.facebook.com/v7.0/oauth/access_token?" +
                $"client_id={_configuration["FacebookAppId"]}" +
                $"&redirect_uri={_configuration["FacebookRedirectUri"]}" +
                $"&client_secret={_configuration["FacebookAppSecret"]}" +
                $"&code={code}");

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return new Response()
                    .WithError("*", "Unable to authenticate with Facebook");
            }
            var tokenResponseStream = await tokenResponse.Content.ReadAsStreamAsync();
            var tokenResponseModel = await JsonSerializer.DeserializeAsync
                <FacebookTokenResponse>(tokenResponseStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

            var userResponse = await client.GetAsync($"https://graph.facebook.com/me" +
                "?access_token=" + tokenResponseModel.access_token + "&fields=email,name,picture");

            if (!userResponse.IsSuccessStatusCode)
            {
                return new Response()
                    .WithError("*", "Unable to authenticate with Facebook");
            }

            var userResponseStream = await userResponse.Content.ReadAsStreamAsync();
            var user = await JsonSerializer.DeserializeAsync
                <FacebookUserDto>(userResponseStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

            var appUser = await _userManager.FindByNameAsync(user.Email);

            //Check if this user has signed in before (if we have a AspNetUser record for them)
            if (appUser != null)
            {
                if (appUser.AuthType != AuthType.Facebook)
                {
                    return new Response().WithError("*", "Account already authenticated with " + appUser.AuthType.ToString());
                }
                //await _signInManager.SignInAsync(appUser, false);

                if (appUser.AvatarUrl != user.Picture?.Data?.Url)
                {
                    appUser.AvatarUrl = user.Picture?.Data?.Url;
                    await _userManager.UpdateAsync(appUser);
                }
            }

            //No existing sign ins for this user, let's create a record for them
            else
            {
                appUser = new ApplicationUser
                {
                    //hack for now - force users to activate email before logging in?
                    UserName = user.Email,
                    Email = user.Email,
                    DisplayName = user.Name.Split(" ")[0],
                    AuthType = AuthType.Facebook,
                    AvatarUrl = user.Picture?.Data?.Url ?? ""
                };
                var result = await _userManager.CreateAsync(appUser);
                if (!result.Succeeded)
                {
                    return new Response()
                        .WithErrors(result.Errors
                            .Select(error => new Response.Error()
                            {
                                Key = error.Code == "DuplicateUserName" ? "email" : "*",
                                Msg = error.Code == "DuplicateUserName" ? "Email already in use" : error.Description
                            }).ToList());
                }
            }

            //Create JWT and cookie for user and return success
            var token = GenerateJwt(appUser).ToString();
            GenerateCookie(token);

            return new DataResponse<TokenDto>()
                .WithData(new TokenDto()
                {
                    Token = token
                });
        }

        public async Task<Response> FacebookLogin(OAuthLoginDto loginDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://graph.facebook.com/" + loginDto.UserId +
                "?access_token=" + loginDto.AccessToken + "&fields=email,name");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new Response()
                    .WithError("*", "Unable to authenticate with Facebook");
            }

            var responseStream = await response.Content.ReadAsStreamAsync();

            var user = await JsonSerializer.DeserializeAsync
                <FacebookUserDto>(responseStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

            var appUser = await _userManager.FindByNameAsync(user.Email);

            //Check if this user has signed in before (if we have a AspNetUser record for them)
            if (appUser != null)
            {
                if (appUser.AuthType != AuthType.Facebook)
                {
                    return new Response().WithError("*", "Account already authenticated with " + appUser.AuthType.ToString());
                }
                //await _signInManager.SignInAsync(appUser, false);
            }

            //No existing sign ins for this user, let's create a record for them
            else
            {
                appUser = new ApplicationUser
                {
                    //hack for now - force users to activate email before logging in?
                    UserName = user.Email,
                    Email = user.Email,
                    DisplayName = user.Name.Split(" ")[0],
                    AuthType = AuthType.Facebook
                };
                var result = await _userManager.CreateAsync(appUser);
                if (!result.Succeeded)
                {
                    return new Response()
                        .WithErrors(result.Errors
                            .Select(error => new Response.Error()
                            {
                                Key = error.Code == "DuplicateUserName" ? "email" : "*",
                                Msg = error.Code == "DuplicateUserName" ? "Email already in use" : error.Description
                            }).ToList());
                }
            }

            //Create JWT and cookie for user and return success
            var token = GenerateJwt(appUser).ToString();
            GenerateCookie(token);

            return new DataResponse<TokenDto>()
                .WithData(new TokenDto()
                {
                    Token = token
                });
        }

        private object GenerateJwt(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email),
                new Claim("avatarUrl", user.AvatarUrl ?? ""),
                new Claim("displayName", user.DisplayName ?? user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                null,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void GenerateCookie(string token)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                SameSite = SameSiteMode.Strict
            };
            if (_currentEnvironment.IsProduction())
            {
                cookieOptions.Domain = "jrdn.tech";
                cookieOptions.Secure = true;
            }
            _contextAccessor.HttpContext.Response.Cookies.Append("token", token, cookieOptions);
        }
    }
}
