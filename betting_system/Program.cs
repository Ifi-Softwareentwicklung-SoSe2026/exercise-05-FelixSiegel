using wm;

Console.WriteLine("Hi");

var manager = new TurnierManager();

manager.initializeTurnier();
manager.printGames();
manager.saveAllData("test.json");
