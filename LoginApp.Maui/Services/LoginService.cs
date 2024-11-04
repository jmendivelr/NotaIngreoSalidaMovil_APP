using LoginApp.Maui.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace LoginApp.Maui.Services
{
    public class LoginResponse
    {
        public string Status { get; set; }
        public UserResult Result { get; set; }
    }

    public class UserResult
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string TPD { get; set; }
        public string serie_doc { get; set; }
    }
    public class LoginService : ILoginService
    {
        public async Task<User> Login(string email, string password)
        {
            try
            {
                var client = new HttpClient();
                var data = new
                {
                    correo = email,
                    clave = password
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                string url = "http://192.168.1.3:8022/api/Usuario/LoginUsuario";
                HttpResponseMessage response = await client.PostAsync(url, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                        if (loginResponse == null || loginResponse.Status != "ok" || loginResponse.Result == null)
                        {
                            return null;
                        }

                        // Crear un objeto User asignando los valores desde UserResult
                        User user = new User
                        {
                            Id = loginResponse.Result.Id,
                            FullName = loginResponse.Result.FullName,
                            Email = loginResponse.Result.Email,
                            Password = loginResponse.Result.Password,
                            token = loginResponse.Result.Token,
                            TPD = loginResponse.Result.TPD,
                            serie_doc = loginResponse.Result.serie_doc,
                        };

                        if (user.Id == 0)
                        {
                            return null;
                        }

                        return user;
                    }
                    catch (JsonException)
                    {
                        await Shell.Current.DisplayAlert("Error", "Error al conectar con Servidor, verificar conexión.", "Ok");
                        // Manejar excepción de deserialización JSON0
                        return null;
                    }
                }

                
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Error al conectar con Servidor, verificar conexión.", "Ok");
                // Manejar excepción de deserialización JSON0
                return null;
                //throw;
            }
            return null;
        }
    }
}
