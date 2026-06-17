using wm;

var manager = new TurnierManager();

if (args.Length == 0) {
    manager.initializeTurnier();
    manager.printGames();
    manager.saveAllData("test.json");
    manager.loadAllData("test.json");
} else {
    CommandHandler.Handle(manager, args);
}

