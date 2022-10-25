using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExperiments;

class Program
{
    #region SettingMenu

    private const string BACKMENU = "Назад";
    private const string SHOPMENU = "Товары";
    private const string ABOUTMENU = "Справка";
    private const string ADRESSMENU = "Адрес производителя";
    private const string DELIVERYMENU = "Доставка";
    private const string SUPPORTMENU = "Техническая поддержка";

    private const string ADRESS = "г.Иваново Ул.Волжская строение 13";
    private const string DELIVERY = "доставка осуществляеться по всей россии в периоды от 6 утра до 10 вечера ";
    private const string SUPPORT = "если требуется помощь специалиста обращаться на электронную почту 7rotkev@gmail.com или по номеру телефона +79030146515";


    private static ReplyKeyboardMarkup MainMenu = new(
        new List<KeyboardButton>()
            {
               new KeyboardButton(SHOPMENU),
               new KeyboardButton(ABOUTMENU)
            });

    private static ReplyKeyboardMarkup AboutMenu = new(
       new List<KeyboardButton>()
           {
               new KeyboardButton(ADRESSMENU),
               new KeyboardButton(DELIVERYMENU),
               new KeyboardButton(SUPPORTMENU),
               new KeyboardButton(BACKMENU)
           });


    #endregion


    #region SettingImage

    private static Dictionary<Telegram.Bot.Types.InputFiles.InputOnlineFile, string> Shop = new()
    {
        {
            new Telegram.Bot.Types.InputFiles.InputOnlineFile(
                    new Uri(@"https://upload.wikimedia.org/wikipedia/commons/thumb/0/05/%D0%A7%D0%B0%D0%BF%D0%B5%D0%BB%D1%8C%D0%BD%D0%B8%D0%BA.%D0%A0%D1%8F%D0%B7%D0%B0%D0%BD%D1%81%D0%BA%D0%B0%D1%8F_%D0%B3%D1%83%D0%B1%D0%B5%D1%80%D0%BD%D0%B8%D1%8F.jpg/800px-%D0%A7%D0%B0%D0%BF%D0%B5%D0%BB%D1%8C%D0%BD%D0%B8%D0%BA.%D0%A0%D1%8F%D0%B7%D0%B0%D0%BD%D1%81%D0%BA%D0%B0%D1%8F_%D0%B3%D1%83%D0%B1%D0%B5%D1%80%D0%BD%D0%B8%D1%8F.jpg")),
            "Чапельник старинного образца, для изысканых ценителей антиквариата"
        },
        {
            new Telegram.Bot.Types.InputFiles.InputOnlineFile(
                new Uri(@"https://dmhod.ru/image/cache/catalog/category/posuda/siton/img_1028_copy_1-500x500.jpg")),
            "Чапельник варианта модерн, для тех кому нечего доказывать"

        },
        {
            new Telegram.Bot.Types.InputFiles.InputOnlineFile(
                new Uri(@"https://cdn3.static1-sima-land.com/items/4243393/0/1600.jpg?v=1584621559")),
            "Чапельник расписной, для любителей широкой русской души"

        },
        {
            new Telegram.Bot.Types.InputFiles.InputOnlineFile(
                new Uri(@"https://www.extrememarket.ru/upload/iblock/17b/17bcb11fc8f7847845ccf4f417d29d6f.jpg")),
            "Чапельник для кастрюль, и этим все сказано "

        },
        {
            new Telegram.Bot.Types.InputFiles.InputOnlineFile(
                new Uri(@"https://alecom74.ru/upload/iblock/d95/d95fea35ee7ded9ab3fbe7809d77be75.jpeg")),
            "Чапельник стандартный и точка"

        }
    };

    private static Telegram.Bot.Types.InputFiles.InputOnlineFile Factory = new(
        new Uri("https://img04.urban3p.ru/up/o/2045/gallery/122457.jpg"));

    #endregion


    #region BotApi

    static ITelegramBotClient bot = new TelegramBotClient("5721253138:AAFou7n8clzBnRWX-_X4HmqcvyOzsLn0248");
    static void Main(string[] args)
    {
        Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        Console.ReadLine();
    }
    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            await HandleMessage(update.Message, botClient);
        }
    }

    #endregion


    static async Task HandleMessage(Message message, ITelegramBotClient botClient)
    {
        switch (message.Text)
        {
            case "/start":
                await botClient.SendTextMessageAsync(message.Chat, "Menu:", replyMarkup: MainMenu);
                break;
            case SHOPMENU:
                OpenShop(message.Chat, botClient);
                break;
            case ABOUTMENU:
                await botClient.SendTextMessageAsync(message.Chat, "About:", replyMarkup: AboutMenu);
                break;
            case BACKMENU:
                await botClient.SendTextMessageAsync(message.Chat, "Menu:", replyMarkup: MainMenu);
                break;
            case ADRESSMENU:
                await botClient.SendPhotoAsync(message.Chat, caption: ADRESS, photo: Factory);
                break;
            case DELIVERYMENU:
                await botClient.SendTextMessageAsync(message.Chat, DELIVERY);
                break;
            case SUPPORTMENU:
                await botClient.SendTextMessageAsync(message.Chat, SUPPORT);
                break;
            default:
                await botClient.SendTextMessageAsync(message.Chat, "Ничего непонятно, попробуйте снова");
                break;
        }
    }

    static void OpenShop(Chat chat, ITelegramBotClient botClient)
    {
        foreach (var item in Shop)
        {
            botClient.SendPhotoAsync(chat, photo: item.Key, caption: item.Value);
        }
    }
}