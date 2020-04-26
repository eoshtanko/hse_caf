using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VkNet.Model;
using VkNet.Utils;
using Newtonsoft.Json.Linq;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;
using VkNet.Model.Keyboard;
using VkNet.Enums.SafetyEnums;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace myFirstAzureWebApp
{
    [Route("api/[controller]")]
        [ApiController]
        public class CallbackController : ControllerBase
        {
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private readonly IConfiguration _configuration;

        private readonly IVkApi _vkApi;
        public CallbackController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
        }

        [HttpPost]
            public IActionResult Callback([FromBody] Updates updates)
            {
             KeyboardBuilder key = new KeyboardBuilder();
             key.AddButton("1", "a");
            key.AddButton("2", "b");
            key.AddButton("3", "c");
            key.AddLine();
            key.AddButton("4", "c");
            key.AddButton("5", "d");
            key.AddButton("6", "e");
            key.AddLine();
            key.AddButton("7", "f");
            key.AddButton("8", "g");
            key.AddButton("9", "h");
            key.AddLine();
            key.AddButton("10", "i");
            key.AddButton("11", "j");
            key.AddButton("12", "k");
            key.AddLine();
            key.AddButton("13", "l");
            key.AddButton("14", "m");
            key.AddButton("15", "n");
            key.AddLine();
            key.AddButton("16", "o");
            key.AddLine();
            key.AddButton("Список", "r", KeyboardButtonColor.Positive);
            MessageKeyboard keyboard = key.Build();



          



            /*
            var photos = _vkApi.Photo.Get(new PhotoGetParams
            {
                AlbumId = PhotoAlbumType.Id(100),
                OwnerId = _vkApi.UserId.Value
            });
            */

            // Тип события
            switch (updates.Type)
                {
                    // Ключ-подтверждение
                    case "confirmation":
                        {
                            return Ok(_configuration["Config:Confirmation"]);
                        }

                // Новое сообщение
                case "message_new":
                    {
                        // Десериализация
                        var msg = Message.FromJson(new VkResponse(updates.Object));
     
                        // Отправим в ответ полученный от пользователя текст

                        if(msg.Text == "1")
                        {

                            async Task<string> UploadFile(string serverUrl, string file, string fileExtension)
                            {
                                // Получение массива байтов из файла
                                var data = GetBytes(file);

                                // Создание запроса на загрузку файла на сервер
                                using (var client = new HttpClient())
                                {
                                    var requestContent = new MultipartFormDataContent();
                                    var content = new ByteArrayContent(data);
                                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                    requestContent.Add(content, "file", $"file.{fileExtension}");

                                    var response = client.PostAsync(serverUrl, requestContent).Result;
                                    return Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync());
                                }
                            }


                            byte[] GetBytes(string fileUrl)
                            {
                                using (var webClient = new WebClient())
                                {
                                    return webClient.DownloadData(fileUrl);
                                }
                            }


                            async void SendMessageWithImage()
                           {
                                //var userId = 12345678; //Получатель сообщения!!!!!!!!!!!!!!!!1

                                // Получить адрес сервера для загрузки картинок в сообщении
                                var uploadServer = _vkApi.Photo.GetMessagesUploadServer(msg.PeerId.Value);

                                // Загрузить картинку на сервер VK.
                                var response = await UploadFile(uploadServer.UploadUrl,
                                    "https://www.gstatic.com/webp/gallery/1.jpg", "jpg");

                                // Сохранить загруженный файл
                                var attachment = _vkApi.Photo.SaveMessagesPhoto(response);

                                //Отправить сообщение с нашим вложением
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    PeerId = msg.PeerId.Value,
                                    //UserId = msg.UserId.Value,
                                    Message = "Message", //Сообщение
                                    Attachments = attachment, //Вложение
                                    RandomId = new Random().Next(999999) //Уникальный идентификатор
                                });
                            }


                            SendMessageWithImage();
                        }       
                        else if (msg.Text == "Список")
                        {
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId.Value,
                                //UserId = msg.UserId.Value,
                                Keyboard = keyboard,
                                Message = "Выберите корпус: \n1. Большая Ордынка, 47/7\n2. Б.Переяславская ул., д.50\n3." +
                                " Б.Трехсвятительский пер., 3\n4. Усачева, 6\n5. М.Гнездниковский пер., д.4\n6. М.Ордынка," +
                                " 17\n7. М.Пионерская ул., 12/4\n8. М.Трехсвятительский пер., 8/2, стр.1\n9. Мясницкая, " +
                                "11\n10. Мясницкая, 20\n11. Покровский бульвар, д. 11\n12. Потаповский пер., д.16," +
                                " стр.10\n13. 1-й Саратовский пр-д, д. 5, корп. 2\n14. Старая Басманная, 21/4 стр. " +
                                "1, стр. 5, стр. 6\n15. Таллинская ул., д.34\n16. Трифоновская ул., д.57",
                            });
                        }
                        else
                        {
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = msg.PeerId.Value,
                                Keyboard = keyboard,
                                Message = "Выберите корпус: \n1. Большая Ордынка, 47/7\n2. Б.Переяславская ул., д.50\n3." +
                                " Б.Трехсвятительский пер., 3\n4. Усачева, 6\n5. М.Гнездниковский пер., д.4\n6. М.Ордынка," +
                                " 17\n7. М.Пионерская ул., 12/4\n8. М.Трехсвятительский пер., 8/2, стр.1\n9. Мясницкая, " +
                                "11\n10. Мясницкая, 20\n11. Покровский бульвар, д. 11\n12. Потаповский пер., д.16," +
                                " стр.10\n13. 1-й Саратовский пр-д, д. 5, корп. 2\n14. Старая Басманная, 21/4 стр. " +
                                "1, стр. 5, стр. 6\n15. Таллинская ул., д.34\n16. Трифоновская ул., д.57",
                            });
                        }
                        /*
                        _vkApi.Messages.Send(new MessagesSendParams
                        {
                            RandomId = new DateTime().Millisecond,
                            PeerId = msg.PeerId.Value,
                            Keyboard = keyboard,
                            Message = " Выберите корпус: \n1. Армянский пер., д.4, стр.2\n2. М.Гнездниковский пер., д.4" +
"\n3. Мясницкая, 11\n4. Мясницкая, 20\n5. Большая Ордынка, 47/7\n6. М.Ордынка, 17	\n7. Б.Переяславская ул., д.50\n8. М.Пионерская ул., 12/4" +
"\n9. Покровский бульвар, д. 11\n10. Потаповский пер., д.16, стр.10\n11. 1-й Саратовский пр-д, д. 5, корп. 2\n12. Старая Басманная, 21/4 стр. 1, стр. 5, стр. 6" +
"\n13. Таллинская ул., д.34\n14. Б.Трехсвятительский пер., 3\n15. М.Трехсвятительский пер., 8/2, стр.1\n16. Трифоновская ул., д.57" +
"\n17. Усачева, 6\n18. Хитровский пер., 2/8, стр.5\n19. Шаболовка, 26, стр. 4\n20. Шаболовка, 28/11, стр.2",
                        }) ;
                        */
                        break;
                    }
            }
                return Ok("ok");
            }

        private object GetBytes(string file)
        {
            throw new NotImplementedException();
        }
    }



       [Serializable]
        public class Updates
        {
            /// <summary>
            /// Тип события
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }

            /// <summary>
            /// Объект, инициировавший событие
            /// Структура объекта зависит от типа уведомления
            /// </summary>
            [JsonProperty("object")]
            public JObject Object { get; set; }

            /// <summary>
            /// ID сообщества, в котором произошло событие
            /// </summary>
            [JsonProperty("group_id")]
            public long GroupId { get; set; }

            /// <summary>
            /// Секретный ключ. Передается с каждым уведомлением от сервера
            /// </summary>
            [JsonProperty("secret")]
            public string Secret { get; set; }
        }
}
