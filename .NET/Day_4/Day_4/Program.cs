using Day_4;
using System.Runtime.CompilerServices;

var games = new List<Games>

{

    new Games { Title = "The Legend of Zelda: Breath of the Wild", Genre = "Action-adventure", ReleaseYear = 2017, Rating = 9.5, Price = 59 },

    new Games { Title = "God of War", Genre = "Action-adventure", ReleaseYear = 2018, Rating = 9.3, Price = 49 },

    new Games { Title = "Red Dead Redemption 2", Genre = "Action-adventure", ReleaseYear = 2018, Rating = 9.7, Price = 69 },

    new Games { Title = "The Witcher 3: Wild Hunt", Genre = "RPG", ReleaseYear = 2015, Rating = 9.4, Price = 39 },

    new Games { Title = "Minecraft", Genre = "Sandbox", ReleaseYear = 2011, Rating = 9.0, Price = 26 },

    new Games { Title = "Fortnite", Genre = "Battle Royale", ReleaseYear = 2017, Rating = 8.5, Price = 0 },

    new Games { Title = "Among Us", Genre = "Party", ReleaseYear = 2018, Rating = 8.0, Price = 5 },

    new Games { Title = "Cyberpunk 2077", Genre = "RPG", ReleaseYear = 2020, Rating = 7.5, Price = 59 },

    new Games { Title = "Hades", Genre = "Roguelike", ReleaseYear = 2020, Rating = 9.2, Price = 24 },

    new Games { Title = "Animal Crossing: New Horizons", Genre = "Simulation", ReleaseYear = 2020, Rating = 9.1, Price = 59 }

};

//List<string> allGames = new List<string>();

//foreach (var game in games)
//{
//    allGames.Add(game.Title);
//}
//foreach (var title in allGames)
//{
//    Console.WriteLine(title);
//}

//var allGames = games.Select(n => n.Title);
//foreach (var title in allGames)
//{
//    Console.WriteLine(title);
//}

//var Genregames = games.Where(g => g.Genre == "RPG");

//foreach (var item in Genregames)
//{
//    Console.WriteLine(item.Title);
//}

//var modelgames = games.Any(g => g.ReleaseYear >= 2020);
//Console.WriteLine(modelgames);

//var sortbygames = games.OrderByDescending(g => g.ReleaseYear);

//foreach (var item in sortbygames)
//{
//    Console.WriteLine($"{item.Title} - {item.ReleaseYear}");
//}

//var avgprice = games.Average(g => g.Price);
//Console.WriteLine($"Average Price: {avgprice}");

//var maxValue = games.Max(g => g.Rating);
//var first = games.First(g => g.Rating == maxValue);

//Console.WriteLine($"Highest Rated Game: {first.Title} - {first.Rating}");


//var groupByGenre = games.GroupBy(g => g.Genre);

//foreach (var group in groupByGenre)
//{
//    Console.WriteLine($"Genre: {group.Key}");
//    foreach (var game in group)
//    {
//        Console.WriteLine($" - {game.Title}");
//    }
//}


var multiConditions = games.Where(g => g.Genre == "RPG" && g.ReleaseYear > 2015)
                            .OrderBy(g => g.ReleaseYear)
                            .Select(g => $"{ g.Title} -- {g.ReleaseYear } -- {g.Rating}");

foreach (var item in multiConditions)
{
    Console.WriteLine(item);
}