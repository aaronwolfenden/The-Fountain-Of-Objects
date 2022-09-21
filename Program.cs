﻿DialogueHelper.Write("What size map do you wish to play on? (Small, Medium, Large) - ", ConsoleColor.Cyan);
TheFountainOfObjectsGame game = Console.ReadLine()?.ToLower() switch
{
    "small" => CreateSmallGame(),
    "medium" => CreateMediumGame(),
    "large" => CreateLargeGame()
};

game.Run();


// Creates a small game map
TheFountainOfObjectsGame CreateSmallGame()
{
    Map map = new Map(4, 4);
    Location startLocation = new Location(0, 0);
    Player player = new Player(startLocation);

    map.SetRoomType(startLocation, RoomType.CavernEntrance);
    map.SetRoomType(new Location(0, 2), RoomType.FountainRoom);

    return new TheFountainOfObjectsGame(map, player);
}

// Creates a medium game map
TheFountainOfObjectsGame CreateMediumGame()
{
    Map map = new Map(6, 6);
    Location startLocation = new Location(0, 0);
    Player player = new Player(startLocation);

    map.SetRoomType(startLocation, RoomType.CavernEntrance);
    map.SetRoomType(new Location(5, 4), RoomType.FountainRoom);

    return new TheFountainOfObjectsGame(map, player);
}

// Creates a large game map
TheFountainOfObjectsGame CreateLargeGame()
{
    Map map = new Map(8, 8);
    Location startLocation = new Location(0, 0);
    Player player = new Player(startLocation);

    map.SetRoomType(startLocation, RoomType.CavernEntrance);
    map.SetRoomType(new Location(7, 4), RoomType.FountainRoom);

    return new TheFountainOfObjectsGame(map, player);
}

public class TheFountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    private readonly ISense[] _senses;
    public bool isFountainActivated { get; set; }

    public TheFountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;

        // New senses can be added to this list as the game is expanded upon, allowing for scalability
        _senses = new ISense[]
        {
            new CavernEntranceSense(),
            new FountainSense()
        };
    }

    // The main game loop
    public void Run()
    {
        while (!IsWon)
        {
            DisplayGameStatus();
            ICommand playerCommand = GetCommand();
            playerCommand.Execute(this);

            if (IsWon)
            {
                DialogueHelper.WriteLine("The Fountain Of Objects has been reactivated, and you have escaped with your life!", ConsoleColor.Magenta);
                DialogueHelper.WriteLine("You Win!", ConsoleColor.Magenta);
            }
        }
    }


    // Display the current status of the game, including the players location & any senses that may be triggered
    private void DisplayGameStatus()
    {
        DialogueHelper.WriteLine("---------------------------------------", ConsoleColor.White);
        DialogueHelper.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})", ConsoleColor.White);

        foreach (ISense sense in _senses)
        {
            if (sense.CanSense(this)) sense.Display(this);
        }
    }

    // Returns a ICommand object to get the players input
    private ICommand GetCommand()
    {
        // Keep looping until a valid command is entered
        while (true)
        {
            DialogueHelper.Write("What would you like to do? - ", ConsoleColor.Cyan);
            string? command = Console.ReadLine();

            if (command?.ToLower() == "move north") return new MoveCommand(Direction.North);
            if (command?.ToLower() == "move south") return new MoveCommand(Direction.South);
            if (command?.ToLower() == "move west") return new MoveCommand(Direction.West);
            if (command?.ToLower() == "move east") return new MoveCommand(Direction.East);
            if (command?.ToLower() == "enable fountain") return new ActivateCommand();

            DialogueHelper.WriteLine($"{command} is not a valid not a valid command.", ConsoleColor.Red);
        }
    }

    // Checks whether the player has won or not dependent on if the fountain is activated and if the player is at the cave entrance
    public bool IsWon => isFountainActivated && Map.GetRoomType(Player.Location) == RoomType.CavernEntrance;

}

// A class containing all information relevant to the player
public class Player
{
    public Location Location { get; set; }

    public Player(Location location)
    {
        Location = location;
    }
}

// A record to store locations as
public record Location(int Row, int Column);

// Contains all information regarding to the map (Room information, map size etc.
public class Map
{
    private readonly RoomType[,] _rooms;
    public int Rows { get; set; }
    public int Columns { get; set; }

    public Map(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _rooms = new RoomType[Rows, Columns];
    }

    // Set the room type to a given room
    public void SetRoomType(Location location, RoomType room) => _rooms[location.Row, location.Column] = room;


    // Get the room type at the given location
    public RoomType GetRoomType(Location location) => _rooms[location.Row, location.Column];

    // Checks to see if the room exists on the map
    public bool RoomExists(Location location) => (location.Row < _rooms.GetLength(0) && location.Row >= 0 && location.Column < _rooms.GetLength(1) && location.Column >= 0);


}
// An interface for commands which further commands such as movement will be based off
public interface ICommand
{
    
    public void Execute(TheFountainOfObjectsGame game) { }
}


// A move command class based off the ICommand interface
public class MoveCommand : ICommand
{
    public Direction Direction { get; }

    public MoveCommand(Direction direction)
    {
        Direction = direction;
    }

    // Executes the movement command
    public void Execute(TheFountainOfObjectsGame game)
    {
        
        Location currentPlayerLocation = game.Player.Location;
        Location newLocation = Direction switch
        {
            Direction.North => new Location(currentPlayerLocation.Row - 1, currentPlayerLocation.Column),
            Direction.South => new Location(currentPlayerLocation.Row + 1, currentPlayerLocation.Column),
            Direction.East => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column + 1),
            Direction.West => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column - 1)
        };

        if (game.Map.RoomExists(newLocation))
        {
            game.Player.Location = newLocation;
        }
        else
        {
            DialogueHelper.WriteLine("There is a wall here.", ConsoleColor.White);
        }

    }

}
// An activation command for the fountain based off ICommand
public class ActivateCommand : ICommand
{
    public void Execute(TheFountainOfObjectsGame game)
    {
        // Checks to see if the fountain is in the same room as player and responds accordingly
        if (game.Map.GetRoomType(game.Player.Location) == RoomType.FountainRoom) game.isFountainActivated = true;
        else DialogueHelper.WriteLine("The fountain is not in this room.", ConsoleColor.White);
    }
}

// Interface for sensing which individual senses will be based off
public interface ISense
{
    // Check to see if anything can be sensed
    public bool CanSense(TheFountainOfObjectsGame game);

    // Displays the appropriate message if CanSense returns true
    public void Display(TheFountainOfObjectsGame game);
}

// A sense for checking if the player is at the cavern entrance
public class CavernEntranceSense : ISense
{
    // Check if the cavern entrance can be sensed
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.CavernEntrance;
    // Displays a message if it can be sensed
    public void Display(TheFountainOfObjectsGame game) => DialogueHelper.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.",
                                                            ConsoleColor.Yellow);

}
// A sense for checking if the player can sense the fountain 
public class FountainSense : ISense
{
    // Checks if the fountain is in the player's room
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.FountainRoom;

    // Displays an appropriate message if the fountain is on or off
    public void Display(TheFountainOfObjectsGame game)
    {
        if (!game.isFountainActivated) DialogueHelper.WriteLine("You hear water dripping in this room. The Fountain Of Objects is here!", ConsoleColor.Blue);
        else DialogueHelper.WriteLine("You hear the rushing waters from The Fountain Of Objects. It has been reactivated!", ConsoleColor.Blue);
    }
}

// Static helper class which can be referenced for highlighting relevant messages within the game
public static class DialogueHelper
{
    // WriteLine method replacement for Console.WriteLine()
    public static void WriteLine(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
    }

    // Write method replacement for Console.Write()
    public static void Write(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
    }
}




// Enumeration to store the different types of room, allows for expanding upon
public enum RoomType { Empty, CavernEntrance, FountainRoom, Pit}

// Public enumeration to store a direction as it will be used in multiple instances
public enum Direction { North, South, East, West }